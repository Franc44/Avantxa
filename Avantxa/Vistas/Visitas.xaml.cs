using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Avantxa.WebApi;
using Newtonsoft.Json;
using Plugin.Media;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class Visitas : ContentPage
    {
        List<Usuario.Usuar> departamentos = new List<Usuario.Usuar>();

        List<Motivos> motivos = new List<Motivos>();
        string jwt, motivo = null;
        readonly int id = 0,vid = 0;
        private byte[] userid = null, fileByte = null;

        public Visitas(string token, int uhid, short visid)
        {
            InitializeComponent();
            jwt = token;
            id = uhid;
            vid = visid;
            Load();
        }

        async void Load()
        {
            All.IsVisible = false;
            Activity.IsRunning = true;
            Activity.IsVisible = true;
            departamentos = await Constants.ObtenerUsuariosUH(jwt, id);

            if (departamentos.Count == 0 || departamentos == null)
            {
                var p1 = new MessageBox("Error", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Visitas", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", "Ok");
            }
            else
            {
                foreach (var item in departamentos)
                {
                    Usuario.Items.Add(item.Departamento + ", " + RSA.Decryption(item.Nombre));
                }

                motivos.Add(new Motivos { Id = "V", Nombre = "Visita" });
                motivos.Add(new Motivos { Id = "M", Nombre = "Mensajeria" });

                foreach (var item in motivos)
                {
                    Motivo.Items.Add(item.Nombre);
                }
            }
            if(vid > 0)
            {
                GEnviar.IsVisible = false;
                GSalida.IsVisible = true;
                Variables.IsVisible = false;
                Constantes.IsVisible = true;

                var Visita = await Constants.ObtenerVisitaId(jwt, vid);

                if (departamentos == null || Visita == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else
                {
                    foreach (var vis in Visita)
                    {
                        var ms = new MemoryStream(vis.VisIdentificacion);

                        foreach (var item in departamentos)
                        {
                            var depusu = RSA.Decryption(item.Usua);
                            var visusu = RSA.Decryption(vis.UsuaMovID);

                            if (depusu == visusu)
                            {
                                TDepto.Text = "Visita a: " + RSA.Decryption(item.Nombre) + ", " + item.Departamento;
                                HEntrada.Text = "Entrada: " + vis.VisHoraEnt.ToString("dd/MM/yy HH:mm");

                                if (vis.VisHoraSal == new DateTime(2000, 01, 01))
                                    HSalida.Text = "";
                                else
                                {
                                    GSalida.IsVisible = false;
                                    HSalida.Text = "Salida: " + vis.VisHoraSal.ToString("dd/MM/yy HH:mm");
                                }
                                if (vis.VisMotivo.Trim() == "V")
                                    TMotivo.Text = "Motivo de la visita: Visita";
                                else
                                    TMotivo.Text = "Motivo de la visita: Mensajería";

                                image.Source = ImageSource.FromStream(() => ms);
                            }
                        }
                    }

                }
            }
            Activity.IsRunning = false;
            All.IsVisible = true;
            Activity.IsVisible = false;
        }

        void Volver(Object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void Guardar(Object sender, EventArgs e)
        {
            Todo.IsVisible = false;
            Activity.IsRunning = true;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            if (userid != null && motivo != null && id != 0 && fileByte != null)
            {
                var pop = new MessageBox("Enviar", "¿Esta seguro de guardar el registro de visita?", 1);

                pop.OnDialogClosed += async (s, arg) =>
                {

                    if (arg)
                    {
                        All.IsVisible = false;
                        Activity.IsRunning = true;
                        Activity.IsVisible = true;
                        try
                        {
                            var Registro = new Visita
                            {
                                UsuaMovID = userid,
                                VisID = 0,
                                VisHoraEnt = DateTime.Now,
                                VisHoraSal = new DateTime(2000, 01, 01),
                                VisMotivo = motivo,
                                UnHabMovID = (short)id,
                                VisIdentificacion = fileByte
                            };
                            var json = JsonConvert.SerializeObject(Registro);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var request = await client.PostAsync(Constants.uri + "Visitas/Agrega/", content);

                            if (request.IsSuccessStatusCode)
                            {
                                var p1 = new MessageBox("Éxito", "El registro de la visita se ha guardado con éxito.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                //await DisplayAlert("Enviar", "El resgistro de la visita se ha guardado con éxito.", "Ok");
                                await Navigation.PopAsync();

                            }
                            else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                jwt = await Constants.RefreshToken();
                                var p1 = new MessageBox("Error", "Su sesión se ha agotado, vuelva a intentarlo.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                //await DisplayAlert("Enviar", "Su sesión se ha agotado, vuelva a intentarlo.", "Ok");
                            }
                            else
                            {
                                var p1 = new MessageBox("Error", "No se ha podido guardar el registro.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                //await DisplayAlert("Enviar", "No se ha podido guardar el registro.", "Ok");
                            }

                        }
                        catch (Exception es)
                        {
                            var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //await DisplayAlert("Enviar", "Ha ocurrido un error al querer guardar el registro.", "Ok");
                            Console.WriteLine(es);
                        }
                        Activity.IsRunning = false;
                        All.IsVisible = true;
                        Activity.IsVisible = false;
                    }
                };
                await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else
            {
                var p1 = new MessageBox("Error", "Favor de seleccionar el departamento y motivo, así como tomar la foto a la identificación.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Enviar", "Favor de seleccionar el departamento y motivo, así como tomar la foto a la identificación.", "Aceptar");
            }
        }

        void Salida(Object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var pop = new MessageBox("Enviar", "¿Esta seguro de finalizar la visita?", 1);

            pop.OnDialogClosed += async (s, arg) =>
            {
                if (arg)
                {
                    All.IsVisible = false;
                    Activity.IsRunning = true;
                    Activity.IsVisible = true;
                    try
                    {
                        var Registro = new Visita
                        {
                            UsuaMovID = userid,
                            VisID = (short)vid,
                            VisHoraEnt = DateTime.Now,
                            VisHoraSal = DateTime.Now,
                            VisMotivo = motivo,
                            UnHabMovID = (short)id,
                            VisIdentificacion = new byte[10]
                        };
                        var json = JsonConvert.SerializeObject(Registro);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var request = await client.PutAsync(Constants.uri + "Visitas/" + vid, content);

                        if (request.IsSuccessStatusCode)
                        {
                            var p1 = new MessageBox("Éxito", "El registro de visita se ha guardado con éxito.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //await DisplayAlert("Enviar", "El registro de visita se ha guardado con éxito.", "Ok");
                            await Navigation.PopAsync();
                        }
                        else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            jwt = await Constants.RefreshToken();
                            var p1 = new MessageBox("Error", "Su sesión se ha agotado, vuelva a intentarlo.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //await DisplayAlert("Enviar", "Su sesión se ha agotado, vuelva a intentarlo.", "Ok");
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "No se ha podido guardar el registro.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //await DisplayAlert("Enviar", "No se ha podido guardar el registro.", "Ok");
                        }

                    }
                    catch (Exception es)
                    {
                        var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Enviar", "Ha ocurrido un error al querer guardar el registro.", "Ok");
                        Console.WriteLine(es);
                    }
                    Activity.IsRunning = false;
                    All.IsVisible = true;
                    Activity.IsVisible = false;
                }
            };
            App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }

        private void Usuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Usuario.SelectedIndex;
            if (posicion > -1)
            {
                userid = departamentos[posicion].Usua;
            }
        }

        private void Motivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Motivo.SelectedIndex;
            if (posicion > -1)
            {
                motivo = motivos[posicion].Id;
            }
        }

        async void TomarCamara(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                var p1 = new MessageBox("Error", "Camara no disponible.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert(":(", "Camara no disponible.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
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

            file.Dispose();
        }

        class Motivos
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
        }
    }
}
