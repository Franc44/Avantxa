using Avantxa.Vistas;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Solicitudes : ContentPage
    {
        List<Usuario.Usuar> deptos = new List<Usuario.Usuar>();
        readonly List<Motivos> motivos = new List<Motivos>();
        readonly HttpClient cliente = new HttpClient();
        TablaUsuario usuario = new TablaUsuario();
        string motivo = "", depto = "";
        byte[] fileByte = new byte[64];
        byte[] nombre = new byte[64];
        DateTime Completa = new DateTime();
        private DateTime Fecha = DateTime.Today;
        private DateTime Fecha1 = DateTime.Today;
        private DateTime Fecha2 = DateTime.Today;
        ListaMensajes data = new ListaMensajes();
        int Limpieza = 0, horario = 0;
        

        class Motivos
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
        }

        public Solicitudes()
        {
            InitializeComponent();

            motivos.Add(new Motivos { Id = "M", Nombre = "Mantenimiento" });
            motivos.Add(new Motivos { Id = "R", Nombre = "Roof Garden" });
            motivos.Add(new Motivos { Id = "V", Nombre = "Vacaciones" });
            motivos.Add(new Motivos { Id = "O", Nombre = "Otros" });

            foreach (var item in motivos)
            {
                Motivo.Items.Add(item.Nombre);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            usuario = await App.Database.GetItemsAsync();

            /*if (usuario.Tipo.Trim() == "A")
            {
                VMotivos.IsVisible = false;
                VDeptos.IsVisible = true;
                VMensaje.IsVisible = true;
                Fotos.IsVisible = true;
                GEnviar.IsVisible = true;

                string mjwt = usuario.Token;
                int muhid = (int)usuario.UHid;

                deptos = await Constants.ObtenerUsuariosUH(mjwt, muhid);

                if (deptos.Count == 0 || deptos == null)
                {
                    var p1 = new MessageBox("Error", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    ////await DisplayAlert("Visitas", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", "Ok");
                }
                else
                {
                    deptos.Add(new Usuario.Usuar { Usua = RSA.Encryption("T"), Departamento = "Todos", Estatus = 0, Nombre = new byte[64], Tipo = "", Token = "", UHid = 0 });

                    foreach (var item in deptos)
                    {
                        Depar.Items.Add(item.Departamento);
                    }

                }
                motivo = "J";
            }*/

            GCancelar.IsVisible = true;
        }

        protected void Cancelar(Object sender, EventArgs e)
        {
             Navigation.PushAsync(new Menu());
        }

        private void Motivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Motivo.SelectedIndex;
            if (posicion > -1)
            {
                motivo = motivos[posicion].Id;
            }

            if(motivo == "M")
            {
                VMensaje.IsVisible = true;
                Fotos.IsVisible = true;
                Otros.IsVisible = false;
                FechaHora.IsVisible = false;
                FechasVacaciones.IsVisible = false;
                Roof.IsVisible = false;
            }
            else if(motivo == "R")
            {
                VMensaje.IsVisible = false;
                Otros.IsVisible = false;
                Fotos.IsVisible = false;
                FechaHora.IsVisible = true;
                FechasVacaciones.IsVisible = false;
                Roof.IsVisible = true;
                HorasPicker.IsVisible = false;
            }
            else if(motivo == "V")
            {
                VMensaje.IsVisible = false;
                Otros.IsVisible = false;
                Fotos.IsVisible = false;
                FechaHora.IsVisible = false;
                FechasVacaciones.IsVisible = true;
                Roof.IsVisible = false;
            }
            else
            {
                LeyendaRes.IsVisible = false;
                VMensaje.IsVisible = true;
                Otros.IsVisible = true;
                Fotos.IsVisible = true;
                FechaHora.IsVisible = true;
                HorasPicker.IsVisible = true;
                FechasVacaciones.IsVisible = false;
                Roof.IsVisible = false;
            }
            GEnviar.IsVisible = true;
        }

        async void TomarCamara(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                var p1 = new MessageBox("Error", "Camara no disponible.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true); 
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            var stream1 = file.GetStream();
            fileByte = new byte[stream1.Length];
            stream1.Read(fileByte, 0, (int)stream1.Length);
            

            GridFoto.IsVisible = true;
            file.Dispose();
        }

        async void TomarGaleria(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                var p1 = new MessageBox("Error", "Tu celular no soporta esta esta funcionalidad.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                return;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            var stream1 = file.GetStream();
            fileByte = new byte[stream1.Length];
            stream1.Read(fileByte, 0, (int)stream1.Length);

            GridFoto.IsVisible = true;

            file.Dispose();
        }

        private void SeleccionFecha(object sender, DateChangedEventArgs e)
        {
            Fecha = e.NewDate;
        }
        private void SeleccionFecha1(object sender, DateChangedEventArgs e)
        {
            Fecha1 = e.NewDate;
        }
        private void SeleccionFecha2(object sender, DateChangedEventArgs e)
        {
            Fecha2 = e.NewDate;
        }

        async void Enviar(Object sender, EventArgs e)
        {
            Completa = Fecha.Add(TimePicker.Time);
            if (motivo == "")
            {
                var p1 = new MessageBox("Error", "Favor de seleccionar motivo", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Error", "Favor de seleccionar motivo", "Aceptar");
            }
            else
            {
                switch (motivo)
                {
                    case "O":
                        if (string.IsNullOrEmpty(TMensaje.Text))
                        {
                            var p1 = new MessageBox("Error", "Escribe el mensaje de la solicitud", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        else
                            Guardar();
                        break;
                    case "M":
                        if (string.IsNullOrEmpty(TMensaje.Text))
                        {
                            var p1 = new MessageBox("Error", "Escribe el mensaje de la solicitud", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        else
                        {
                            if (fileByte.Length < 500)
                            {
                                var p1 = new MessageBox("Error", "Se debe de tomar una foto o seleccionar una.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                            else
                                Guardar();
                        }
                        break;
                    case "R":
                        if (Completa.Date <= DateTime.Now.Date)
                        {
                            var p1 = new MessageBox("Error", "La fecha debe ser posterior a la fecha de hoy.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        else
                        {
                            if (horario > 0)
                            {
                                TMensaje.Text = "Apartar Roof Garden";

                                if (Completa.DayOfWeek == DayOfWeek.Sunday || Completa.DayOfWeek == DayOfWeek.Saturday || Completa.DayOfWeek == DayOfWeek.Friday)
                                {
                                    int revisa = await Constants.RevisaFechaRoof(usuario.Token, Completa, 0, horario);

                                    if (revisa == 1)
                                        Guardar();
                                    else if (revisa == 2)
                                    {
                                        var p1 = new MessageBox("Error", "Esta fecha y horario ya han sido apartado con anterioridad, por favor seleccione otro horario.", 0);
                                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    }
                                    else
                                    {
                                        var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    }
                                }
                                else
                                {
                                    var p1 = new MessageBox("Error", "Los días deben ser de viernes a domingo.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                }
                            }
                            else
                            {
                                var p1 = new MessageBox("Error", "Favor de elegir un horario.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                        }
                        break;
                    case "V":
                        if (Fecha1.Date != Fecha2.Date)
                        {
                            if (Fecha1.Date >= DateTime.Now.Date && Fecha2.Date > DateTime.Now.Date)
                            {
                                if (Fecha1.Date < Fecha2.Date)
                                {
                                    TMensaje.Text = "\nFecha inicial: " + Fecha1.ToShortDateString() + "\nFecha final: " + Fecha2.ToShortDateString() + "\n" + TManVac.Text;
                                    Guardar();
                                }
                                else
                                {
                                    var p1 = new MessageBox("Error", "Las fecha final no puede ser menor a la inicial.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                }
                            }
                            else
                            {
                                var p1 = new MessageBox("Error", "Las fechas no pueden ser anteriores a la fecha de hoy.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "Las fechas no pueden ser iguales.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        break;
                }
            }
        }

        async void Guardar()
        {
            var pop = new MessageBox("Enviar", "¿Esta seguro de enviar la solicitud", 1);

            pop.OnDialogClosed += async (s, arg) =>
            {
                if (arg)
                {
                    Todo.IsVisible = false;
                    Act.IsRunning = true;
                    Act.IsVisible = true;
                    try
                    {
                        if (motivo != "J")
                        {
                            nombre = RSA.Encryption(usuario.Nombre);
                            depto = usuario.Departamento;
                        }

                        var Ingresar = new Mensajes
                        {
                            MenMovID = 0,
                            MenMovDeDepto = depto,
                            MenMovDeNombre = nombre,
                            MenMovEstatus = 0,
                            MenMovFecha = DateTime.Now,
                            MenMovFotoA = fileByte,
                            MenMovFotoD = new byte[10],
                            MenMovMensaje = RSA.Encryption(TMensaje.Text),
                            MenMovMensajeRes = new byte[10],
                            UnHabMovID = (short?)usuario.UHid,
                            MenMovMotivo = motivo,
                            MenMovFechaEntrega = DateTime.Parse("01-01-2000"),
                            MenMovFechaFinal = DateTime.Parse("01-01-2000"),
                            MenMovObservaciones = new byte[10],
                            MenMovResponsables = new byte[10]
                        };

                        var json = JsonConvert.SerializeObject(Ingresar);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage respond = null;

                        cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);
                        respond = await cliente.PostAsync(Constants.uri + "Mensajes/", content);

                        if (respond.IsSuccessStatusCode)
                        {
                            var resultado = await respond.Content.ReadAsStringAsync();
                            data = JsonConvert.DeserializeObject<ListaMensajes>(resultado);

                            if (motivo == "R" || (motivo == "O" && Completa >= DateTime.Now.Add(TimeSpan.FromHours(1))))
                                Calendario(data.MenMovID);

                            var p1 = new MessageBox("Enviar", "Su mensaje ha sido enviado.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            await Navigation.PopAsync();
                        }
                        else if (respond.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            usuario.Token = await Constants.RefreshToken();
                            var p1 = new MessageBox("Error", "Su sesión ha expirado, vuelva a intentarlo.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "Su mensaje no ha podido ser enviado.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        Console.WriteLine(ex);
                    }
                }
                Todo.IsVisible = true;
                Act.IsRunning = false;
                Act.IsVisible = false;
            };
            await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }

        async void Calendario(short menid)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);

            try
            {
                var Registro = new CalendarioC
                {
                    CalID = 0,
                    CalEstatus = 1,
                    CalFecha = Completa,
                    CalMenMovId = menid,
                    CalMensaje = RSA.Encryption("Apartado"),
                    CalTitulo = RSA.Encryption("Roof Garden"),
                    CalPara = RSA.Encryption(usuario.Usuario),
                    CalLimpieza = (short)Limpieza,
                    CalTurno = (short)horario
                };
                var json = JsonConvert.SerializeObject(Registro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = await client.PostAsync(Constants.uri + "Calendario/Agrega/", content);

                if (request.IsSuccessStatusCode)
                {
                    ////await DisplayAlert("Enviar", "El recordatorio se ha guardado con éxito.", "Ok");
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var p1 = new MessageBox("Error", "No se ha podido guardar el recordatorio.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else
                {
                    var p1 = new MessageBox("Error", "No se ha podido guardar el recordatorio.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }

            }
            catch (Exception es)
            {
                var p1 = new MessageBox("Error", "Ha ocurrido un error al querer guardar el registro.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                Console.WriteLine(es);
            }
        }

        void Depar_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Depar.SelectedIndex;
            if (posicion > -1)
            {
                depto = deptos[posicion].Departamento;
                nombre = deptos[posicion].Nombre;
            }
        }

        void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //RadioButton button = sender as RadioButton;
            //horario = int.Parse(button.Text);
            if (RB1.IsChecked) horario = 1;
            else if (RB2.IsChecked) horario = 2;
            else horario = 0;
        }

        void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value) Limpieza = 1;
            else Limpieza = 0;
        }
    }
}