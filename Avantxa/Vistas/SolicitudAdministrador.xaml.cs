using System;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Avantxa.WebApi;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class SolicitudAdministrador : ContentPage
    {
        private byte[] FileByte { get; set; }
        public TablaUsuario Usuario { get; set; }
        public ICommand OnBackComman { get; set; }

        public SolicitudAdministrador(TablaUsuario usuario)
        {
            InitializeComponent();
            Usuario = usuario;

            OnBackComman = new Command(execute: () => { Navigation.PopAsync(); });


            BindingContext = this;
        }

        protected async void Enviar(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(TMensaje.Text) || FileByte == null)
            {
                var pop1 = new MessageBox("Error","Favor de escribir el mensaje de la solicitud y tomar la fotografía.",0);
                await App.Current.MainPage.Navigation.PushPopupAsync(pop1, true);
                return;
            } 

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
                        var Ingresar = new Mensajes
                        {
                            MenMovID = 0,
                            MenMovDeDepto = Usuario.Departamento,
                            MenMovDeNombre = RSA.Encryption(Usuario.Nombre),
                            MenMovEstatus = 0,
                            MenMovFecha = DateTime.Now,
                            MenMovFotoA = FileByte,
                            MenMovFotoD = new byte[10],
                            MenMovMensaje = RSA.Encryption(TMensaje.Text),
                            MenMovMensajeRes = new byte[10],
                            UnHabMovID = (short?)Usuario.UHid,
                            MenMovMotivo = "A",
                            MenMovFechaEntrega = DateTime.Parse("01-01-2000"),
                            MenMovFechaFinal = DateTime.Parse("01-01-2000"),
                            MenMovObservaciones = new byte[10],
                            MenMovResponsables = new byte[10]
                        };

                        HttpClient cliente = new HttpClient();
                        var json = JsonConvert.SerializeObject(Ingresar);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage respond = null;

                        cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Usuario.Token);
                        respond = await cliente.PostAsync(Constants.uri + "Mensajes/", content);

                        if (respond.IsSuccessStatusCode)
                        {
                            var p1 = new MessageBox("Enviar", "Su mensaje ha sido enviado.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            await Navigation.PopAsync();
                        }
                        else if (respond.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            Usuario.Token = await Constants.RefreshToken();
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

        protected async void TomarCamara(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                var p1 = new MessageBox(":(", "Camara no disponible.", 0);
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
            FileByte = new byte[stream1.Length];
            stream1.Read(FileByte, 0, (int)stream1.Length);


            GridFoto.IsVisible = true;
            file.Dispose();
        }

        protected async void TomarGaleria(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                var p1 = new MessageBox(":(", "Tu celular no soporta esta esta funcionalidad", 0);
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
            FileByte = new byte[stream1.Length];
            stream1.Read(FileByte, 0, (int)stream1.Length);

            GridFoto.IsVisible = true;

            file.Dispose();
        }
    }
}
