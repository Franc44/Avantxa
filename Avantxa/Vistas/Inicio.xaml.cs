using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using Avantxa.Vistas;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();
        }

        private async void LoginClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Usuario.Text) && !string.IsNullOrEmpty(Contra.Text))
            {
                Login.IsVisible = false;
                Titulo.IsVisible = false;
                Act.IsRunning = true;
                Act.IsVisible = true;
                try
                {
                    var Autenticacion = new User
                    {
                        Usua = RSA.Encryption(Usuario.Text),
                        Contras = RSA.Encryption(Contra.Text),
                        Token = null
                    };

                    var data = await Constants.Login(Autenticacion);

                    if(data == null)
                    {
                        var p1 = new MessageBox("Error", "Usuario o contraseña incorrecta.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        Contra.Text = string.Empty;
                    }
                    else
                    {
                        string tipo = data.Tipo.Trim();

                        if (data.Estatus == 1)
                        {
                            var Login = new TablaUsuario
                            {
                                Usuario = RSA.Decryption(data.Usua),
                                Nombre = RSA.Decryption(data.Nombre),
                                Tipo = data.Tipo.Trim(),
                                UHid = data.UHid,
                                Departamento = data.Departamento.Trim(),
                                Token = data.Token
                            };
                            await App.Database.SaveItemAsync(Login);

                            MainPage.User = Login.Usuario;

                            if (tipo == "S")
                                Application.Current.MainPage = new NavigationPage(new MenuSU(Login));
                            else
                                Application.Current.MainPage = new NavigationPage(new Menu());
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "Favor de llenar el formulario de registro.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            Contra.Text = string.Empty;
                        }
                    }
                }
                catch (Exception es)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    Contra.Text = string.Empty;
                    Console.WriteLine(es);
                }

                Login.IsVisible = true;
                Titulo.IsVisible = true;
                Act.IsRunning = false;
                Act.IsVisible = false;
            }
            else
            {
                var p1 = new MessageBox("Error", "Ingrese usuario y contraseña.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        protected void Crear(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CrearCuenta());
        }
    }
}