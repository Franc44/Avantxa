using System;
using System.Collections.Generic;
using Avantxa.Tools;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class VerUsuarios : ContentPage
    {
        readonly string jwt;
        readonly List<TablaUsuario> usuariosList = new List<TablaUsuario>();

        public VerUsuarios(string Token)
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

            var usuarioApi = await Constants.ObtenerUsuariosSuper(jwt);

            if (usuarioApi == null || usuarioApi.Count == 0)
            {
                var p1 = new MessageBox("Error", "No se ha podido recuperar los datos solicitados , intentelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                usuariosList.Clear();

                foreach (var item in usuarioApi)
                {
                    string rol = item.UsuaMovTipo.Trim() == "S" ? "Super" : item.UsuaMovTipo.Trim() == "A" ? "Administrador" : "Inquilino";
                    string deptostring = item.UsuaMovNombre.Length < 256 ?  "" : item.UsuaMovAPat.Length >= 256 ? RSA.Decryption(item.UsuaMovNombre) + " " + RSA.Decryption(item.UsuaMovAPat) : RSA.Decryption(item.UsuaMovNombre);

                    usuariosList.Add(new TablaUsuario
                    {
                        Usuario = RSA.Decryption(item.UsuaMovID),
                        UHid = item.UnHabMovID,
                        Departamento = "Departamento: " + item.MenMovEdif.Trim() + " " + item.MenMovDepto.Trim(),
                        Nombre = deptostring,
                        Tipo = rol
                    });
                }

                List.ItemsSource = usuariosList;
            }

            List.IsRefreshing = false;
        }

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void Agregar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregaUsuarios(jwt));
        }
    }
}
