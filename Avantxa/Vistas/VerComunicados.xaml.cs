using System;
using System.Collections.Generic;
using Avantxa.Modelos;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class VerComunicados : ContentPage
    {
        TablaUsuario Usuario { get; set; } = new TablaUsuario();
        public VerComunicados(TablaUsuario user)
        {
            InitializeComponent();
            Usuario = user;

            AgregaBoton.IsVisible = user.Tipo.Trim() != "C";

            List.RefreshCommand = new Command(() =>
            {
                CargaDatos();
            });

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;

                var item = (TAnuncios)e.Item;
                Navigation.PushAsync(new VerComunicado(user, item.AnID, item.AnNombre));
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargaDatos();
        }

        private async void CargaDatos()
        {
            List.IsRefreshing = true;
            List.ItemsSource = null;

            var AnunciosList = await Constants.ObtenerAnuncios(Usuario.Token, Usuario.Usuario, (int)Usuario.UHid);

            if(AnunciosList == null)
            {
                var p1 = new MessageBox("Error", "No se ha podido recuperar los datos solicitados , intentelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                AnunciosList.Sort((x, y) => y.AnFecha.CompareTo(x.AnFecha));
                List.ItemsSource = AnunciosList;
            }

            List.IsRefreshing = false;
        } 

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void Agregar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregaAnuncios(Usuario));
        }
    }
}
