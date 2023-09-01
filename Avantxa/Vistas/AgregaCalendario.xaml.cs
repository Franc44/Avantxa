using Avantxa.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebApiAvantxa.Security;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Avantxa;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa.Vistas
{
    public partial class AgregaCalendario : ContentPage
    {
        List<Usuario.Usuar> departamentos = new List<Usuario.Usuar>();
        TablaUsuario usuario = new TablaUsuario();
        DateTime Completa = new DateTime();
        private DateTime Fecha = DateTime.Today;
        readonly int calid;
        private byte[] userid = null;

        public AgregaCalendario(int id)
        {
            InitializeComponent();
            calid = id;
            Load();
        }
        async void Volver(Object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void Load()
        {
            //Obtener datos usuario almacenamiento local
            usuario = await App.Database.GetItemsAsync();

            string jwt = usuario.Token;
            int iduh = (int)usuario.UHid;

            if (usuario.Tipo == "A")
            {
                SelDepto.IsVisible = true;
                departamentos = await Constants.ObtenerUsuariosUH(jwt,iduh);
                if (departamentos.Count == 0 || departamentos == null)
                {
                    var p1 = new MessageBox("Error", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else
                {
                    departamentos.Add(new Usuario.Usuar { Usua = RSA.Encryption("T"), Departamento = "Todos", Estatus = 0, Nombre = new byte[64], Tipo = "", Token = "", UHid = 0});
                    departamentos.Add(new Usuario.Usuar { Usua = RSA.Encryption(usuario.Usuario), Departamento = "Para mí", Estatus = 0, Nombre = new byte[64], Tipo = "", Token = "", UHid = 0 });

                    foreach (var item in departamentos)
                    {
                        Deptos.Items.Add(item.Departamento);
                    }

                }
            }

            if (calid > 0)
            {
                Variables.IsVisible = false;
                Constantes.IsVisible = true;
                GEnviar.IsVisible = false;

                var Calendar = await Constants.ObtenerCalendarioId(usuario.Token, calid);

                if(Calendar == null)
                {
                    var p1 = new MessageBox("Enviar", "Ha ocurrido un error al cargar el registro.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    ////await DisplayAlert("Enviar", "Ha ocurrido un error al cargar el registro.", "Ok");
                    await Navigation.PopAsync();
                }
                else
                {
                    foreach(var item in Calendar)
                    {
                        HTitulo.Text = "Titulo: " + RSA.Decryption(item.CalTitulo);
                        HMensaje.Text = "Mensaje: \n" + RSA.Decryption(item.CalMensaje);
                        HFecha.Text = "Fecha y hora del recordatorio: " + item.CalFecha.ToString();
                        
                    }
                }
            }
        }

        async void Guardar(Object sender, EventArgs e)
        {
            Completa = Fecha.Add(TimePicker.Time);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);

            if (string.IsNullOrEmpty(VMensaje.Text) || string.IsNullOrEmpty(VTitulo.Text))
            {
                var p1 = new MessageBox("Enviar", "Favor de ingresar los datos que se le solicitan arriba.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                ////await DisplayAlert("Enviar", "Favor de ingresar los datos que se le solicitan arriba.", "Aceptar");
            }
            else
            {
                if (Completa <= DateTime.Now.Add(TimeSpan.FromHours(1)))
                {
                    var p1 = new MessageBox("Enviar", "La fecha y hora no pueden ser menores a la fecha de este momento más una hora.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    ////await DisplayAlert("Enviar", "La fecha y hora no pueden ser menores a la fecha de este momento más una hora.", "Aceptar");
                }
                else
                {
                    //var answer = await DisplayAlert("Enviar", "¿Esta seguro de guardar el recordatorio?", "Sí", "No");
                    var pop = new MessageBox("Enviar", "¿Esta seguro de guardar el recordatorio?", 1);

                    pop.OnDialogClosed += async (s, arg) =>
                    { 
                        if (arg)
                        {
                            Activity.IsRunning = true;
                            Activity.IsVisible = true;
                            All.IsVisible = false;
                            try
                            {
                                if (usuario.Tipo == "C") { userid = RSA.Encryption(usuario.Usuario); }

                                var Registro = new CalendarioC
                                {
                                    CalID = 0,
                                    CalEstatus = 1,
                                    CalFecha = Completa,
                                    CalMenMovId = 0,
                                    CalMensaje = RSA.Encryption(VMensaje.Text),
                                    CalTitulo = RSA.Encryption(VTitulo.Text),
                                    CalPara = userid,
                                    CalLimpieza = 0,
                                    CalTurno = 0
                                };
                                var json = JsonConvert.SerializeObject(Registro);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var request = await client.PostAsync(Constants.uri + "Calendario/Agrega/", content);

                                if (request.IsSuccessStatusCode)
                                {
                                    var p1 = new MessageBox("Enviar", "El recordatorio se ha guardado con éxito.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    ////await DisplayAlert("Enviar", "El recordatorio se ha guardado con éxito.", "Ok");
                                    await Navigation.PopAsync();

                                }
                                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                {
                                    usuario.Token = await Constants.RefreshToken();
                                    var p1 = new MessageBox("Enviar", "Su sesión se ha agotado, vuelva a intentarlo.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    ////await DisplayAlert("Enviar", "Su sesión se ha agotado, vuelva a intentarlo.", "Ok");
                                }
                                else
                                {
                                    var p1 = new MessageBox("Enviar", "No se ha podido guardar el registro.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    //await DisplayAlert("Enviar", "No se ha podido guardar el registro.", "Ok");
                                }

                            }
                            catch (Exception es)
                            {
                                var p1 = new MessageBox("Enviar", "Ha ocurrido un error al querer guardar el registro.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                //await DisplayAlert("Enviar", "Ha ocurrido un error al querer guardar el registro.", "Ok");
                                Console.WriteLine(es);
                            }
                            Activity.IsRunning = false;
                            Activity.IsVisible = false;
                            All.IsVisible = true;
                        }
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                }
            }
        }


        private void SeleccionFecha(object sender, DateChangedEventArgs e)
        {
            Fecha = e.NewDate;
        }

        private void Usuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Deptos.SelectedIndex;
            if (posicion > -1)
            {
                userid = departamentos[posicion].Usua;
            }
        }
    }
}