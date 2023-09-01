using System;
using System.Collections.Generic;
using Avantxa.Vistas;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        TablaUsuario sesion = new TablaUsuario();
        string usua, depto, jwt, rol;
        private static short? UH;

        public Menu()
        {
            InitializeComponent();
        }

        private void Emergencia(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Emergencia());
        }
        void Calendario(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new Calendario(usua, jwt));
        }
        void Seguimientos(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new Seguimientos(sesion));
        }
        void Solicitud(Object sender, EventArgs e)
        {
            if(rol == "A")
                Navigation.PushAsync(new Seguimientos(sesion));
            else
                Navigation.PushAsync(new Solicitudes());
        }
        void VerVisita(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerVisitas(jwt, (int)UH));
        }
        void RecibosPago(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RecibosPagos(sesion));
        }
        void Chat(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerMensajes(sesion));
        }

        protected void Anuncios(object sender, EventArgs e)
        {
            Navigation.PushAsync(new VerComunicados(sesion));
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                /*var pop = new MessageBox("Cerrar", "¿Deseas salir de la aplicación?", 1);
                pop.OnDialogClosed += (s, arg) =>
                {
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
                    var navPage = new Xamarin.Forms.NavigationPage(new Inicio());
                    Xamarin.Forms.Application.Current.MainPage = navPage;
                    Navigation.PushAsync(new Inicio());
                }
            };
            await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            sesion = await App.Database.GetItemsAsync();

            Usuario.Text = sesion.Nombre + "\n" + sesion.Departamento;
            usua = sesion.Usuario;
            UH = (short)sesion.UHid;
            depto = sesion.Departamento.Trim();
            jwt = sesion.Token;
            rol = sesion.Tipo.Trim();

            if (rol == "A")
            {
                VerSeguimientos.IsVisible = false;
                VerVisitas.IsVisible = true;
            }
        }
    }
}
