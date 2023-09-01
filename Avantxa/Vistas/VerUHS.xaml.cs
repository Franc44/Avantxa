using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Avantxa.Tools;
using WebApiAvantxa.Security;

namespace Avantxa.Vistas
{
    public partial class VerUHS : ContentPage
    {
        readonly string jwt;
        readonly List<UHs> UnidadesList = new List<UHs>();

        public VerUHS(string Token)
        {
            InitializeComponent();
            jwt = Token;

            List.RefreshCommand = new Command(() =>
            {
                CargaDatos();
            });

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargaDatos();
        }

        async void CargaDatos()
        {
            List.IsRefreshing = true;

            var unidadesApi = await Constants.ObtenerUHs(jwt);

            if(unidadesApi == null || unidadesApi.Count == 0)
            {
                var p1 = new MessageBox("Error", "No se ha podido recuperar los datos solicitados , intentelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                UnidadesList.Clear();

                foreach(var item in unidadesApi)
                {
                    UnidadesList.Add(new UHs { ID = item.UnHabMovID, Nombre = item.UnHabMovNombre.TrimEnd() + " " + RSA.Decryption(item.UnHabMovCalle) + "," + RSA.Decryption(item.UnHabMovColonia) + "," + RSA.Decryption(item.UnHabMovMunicipio) });
                }

                List.ItemsSource = UnidadesList;
            }

            List.IsRefreshing = false;
        }

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void Agregar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregaUHab(jwt));
        }

    }
}
