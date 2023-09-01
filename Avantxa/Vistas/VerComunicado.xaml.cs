using System;
using System.Collections.Generic;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class VerComunicado : ContentPage
    {
        private string Jwt { get; set; } = "";
        private int ID { get; set; } = 0;
        public ICommand OnVolver { get; set; }
        public ICommand OnModify { get; set; }
        private string Nombre { get; set; } = "";

        public VerComunicado(TablaUsuario user,int Id, string nombre)
        {
            InitializeComponent();
            Jwt = user.Token;
            Nombre = nombre;
            ID = Id;

            OnVolver = new Command( () => { Navigation.PopAsync(); });
            OnModify = new Command(() => { Navigation.PopAsync(); });

            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var Anuncio = await Constants.ObtenerAnuncioId(Jwt, ID);

            if(Anuncio == null)
            {
                var p1 = new MessageBox("Error", "No se ha podido recuperar los datos solicitados , intentelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                string UsuarioDecry = RSA.Decryption(Anuncio.AnUsuario);

                TAsunto.Text = "Asunto: " + Anuncio.AnAsunto;
                TPara.Text = UsuarioDecry == "Todos" ? "Estimados " + UsuarioDecry + ".": "Estimado " + Nombre + ".";
                TMensaje.Text = Anuncio.AnMensaje;

                Texto.IsVisible = true;
            }

        }
    }
}
