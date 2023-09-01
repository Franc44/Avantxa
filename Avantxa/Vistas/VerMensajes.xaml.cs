using Avantxa.Base;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avantxa.WebApi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WebApiAvantxa.Security;
using Avantxa.Modelos;

namespace Avantxa.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerMensajes : ContentPage
    {
        private static readonly List<TablaVerMensajes> Tchats = new List<TablaVerMensajes>();
        private static TablaUsuario Usuario = new TablaUsuario();

        public VerMensajes(TablaUsuario user) 
        {
            Usuario = user;

            InitializeComponent();

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;

                var item = (TablaVerMensajes)e.Item;

                ChatM.ChatMen chatMen = new ChatM.ChatMen
                {
                    Identifica = 1,
                    ChatMovil = new ChatM.ChatMovil
                    {
                        ChID = item.ID
                    }
                };

                var pop = new ChatBox(chatMen, Usuario.Token);
                App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
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

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void Agregar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NuevoChat(Usuario));
        }

        private async void Load()
        {
            List.ItemsSource = null;
            Tchats.Clear();

            var chatMovils = await Constants.ObtenerChatsUsua(Usuario.Token, Usuario.Usuario);
            var deptos = await Constants.ObtenerDepartmanetos(Usuario.Token, (int)Usuario.UHid);

            if (chatMovils == null || deptos == null)
            {
                var p1 = new MessageBox("Error", "Ha ocuurido un error inesperado, intentelo más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                await Navigation.PopAsync();
            }
            else if (chatMovils.Count == 0)
            {
                var p1 = new MessageBox("Mensajes", "No ha mandado ningún mensaje, ¿desea enviar alguno?", 1);

                p1.OnDialogClosed += async (s, arg) =>
                {
                    if (arg)
                        await Navigation.PushAsync(new NuevoChat(Usuario));
                };

                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                foreach (var itemC in chatMovils)
                {
                    string UsuaDepto = Usuario.Usuario == RSA.Decryption(itemC.ChUsuaInic) ? RSA.Decryption(itemC.ChUsuaFin) : Usuario.Usuario == RSA.Decryption(itemC.ChUsuaFin) ? RSA.Decryption(itemC.ChUsuaInic) : "";

                    foreach (var itemD in deptos)
                    {
                        var UsuaDecrypt = RSA.Decryption(itemD.Usua);

                        if(Usuario.Tipo.Trim() == "C")
                        {
                            TablaVerMensajes verMensajes = new TablaVerMensajes
                            {
                                ID = itemC.ChID,
                                Mensaje = "Mensaje de: " + UsuaDepto,
                                Fecha = (DateTime)itemC.ChFecha
                            };

                            Tchats.Add(verMensajes);
                            break;
                        }
                        else
                        {
                            if (UsuaDepto == UsuaDecrypt)
                            {
                                TablaVerMensajes verMensajes = new TablaVerMensajes
                                {
                                    ID = itemC.ChID,
                                    Mensaje = "Mensaje de: " + RSA.Decryption(itemD.Nombre) + ", " + itemD.Departamento,
                                    Fecha = (DateTime)itemC.ChFecha
                                };

                                Tchats.Add(verMensajes);
                            }
                        }
                    }
                }
            }

            List.ItemsSource = Tchats;
        }
    }
}