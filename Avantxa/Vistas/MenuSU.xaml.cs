using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;

using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class MenuSU : ContentPage
    {
        readonly TablaUsuario sesion = new TablaUsuario();

        public MenuSU(TablaUsuario usuario)
        {
            sesion = usuario;
            InitializeComponent();
        }

        protected void verUsuario(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerUsuarios(sesion.Token));
        }

        protected void verUH(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerUHS(sesion.Token));
        }

        protected void Chat(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerMensajes(sesion));
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                /*
                var pop = new MessageBox("Cerrar", "¿Deseas salir de la aplicación?", 1);
                pop.OnDialogClosed += (s, arg) => { 
                if (arg)
                {
                    Environment.Exit(0);
                }
                };
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                */
            });
            return true;
        }

        async void CerrarSesion(Object sender, EventArgs e)
        {
            var pop = new MessageBox("Sesión", "¿Deseas salir de tu sesión?", 1);

            pop.OnDialogClosed += (s, arg) =>
            {
                if (arg)
                {
                    App.Database.DeleteItemAsync();
                    App.Database.DeleteItemAsyncMen();
                    App.Database.DeleteItemAsyncVer();
                    var navPage = new NavigationPage(new Inicio());
                    Application.Current.MainPage = navPage;
                    Navigation.PushAsync(new Inicio());
                }
            };

            await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Usuario.Text = sesion.Nombre;
        }
    }
}
