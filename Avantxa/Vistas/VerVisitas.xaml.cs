using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avantxa;
using Avantxa.Modelos;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class VerVisitas : ContentPage
    {
        readonly List<VisitaModelo> Visitas = new List<VisitaModelo>();
        List<WebApi.Usuario.Usuar> Usuarios = new List<WebApi.Usuario.Usuar>();
        List<TVisita> Visita = new List<TVisita>();

        readonly string jwt;
        readonly int id;

        public VerVisitas(string Token,int UHId)
        {
            InitializeComponent();
            jwt = Token;
            id = UHId;

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;

                var item = (VisitaModelo)e.Item;
                Navigation.PushAsync(new Visitas(jwt, id, item.ID));

            };

            List.RefreshCommand = new Command(() =>
            {
                Load();
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Load();
        }

        protected async void Load()
        {
            List.IsRefreshing = true;

            Usuarios = await Constants.ObtenerUsuariosUH(jwt, id);
            Visita = await Constants.ObtenerVisitas(jwt, id);

            if (Usuarios == null || Visita == null)
            {
                var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else if (Visita.Count == 0)
            {
                var pop = new MessageBox("Visitas", "No tiene registrada ninguna visita aún, ¿desea registrar alguna?", 1);

                pop.OnDialogClosed += (s, arg) =>
                {
                    if (arg)
                    {
                        Navigation.PushAsync(new Visitas(jwt, id, 0));
                    }
                    else
                        Navigation.PopAsync();
                };
                await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else
            {
                int count = 0;

                foreach (var visit in Visita)
                {
                    var VisUsu = RSA.Decryption(visit.UsuaMovID);
                    foreach (var usu in Usuarios)
                    {
                        var User = RSA.Decryption(usu.Usua);
                        if (VisUsu == User)
                        {
                            count++;

                            var vis = new VisitaModelo
                            {
                                ID = visit.VisID,
                                Nombre = "Visita a: " + RSA.Decryption(usu.Nombre),
                                Departamento = usu.Departamento,
                                HoraEntrada = "Fecha: " + visit.VisHoraEnt.ToString("dd/MM/yy")
                            };
                           
                            Visitas.Add(vis);
                        }
                    }
                }
                if (count < 1)
                {
                    var pop = new MessageBox("Visitas", "No tiene registrada ninguna visita aún, ¿desea registrar alguna?", 1);

                    pop.OnDialogClosed += (s, arg) =>
                    {
                        if (arg)
                        {
                            Navigation.PushAsync(new Visitas(jwt, id, 0));
                        }
                        else
                            Navigation.PopAsync();
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                }
            }
            List.ItemsSource = Visitas;

            List.IsRefreshing = false;
        }
       
        void Volver(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new Menu());
        }

        void Agregar(Object sender, EventArgs e)
        {
            Navigation.PushAsync(new Visitas(jwt,id,0));
        }
    }
}
