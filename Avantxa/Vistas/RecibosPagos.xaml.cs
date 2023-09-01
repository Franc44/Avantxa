using System;
using System.Collections.Generic;
using Avantxa.Modelos;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class RecibosPagos : ContentPage
    {
        List<RecibosW2> Recibos = new List<RecibosW2>();
        List<Usuario.Usuar> Usuarios = new List<Usuario.Usuar>();
        List<TRecibo> Trecibos = new List<TRecibo>();

        readonly string Usuario, Token, Tipo;
        readonly int uhid;

        public RecibosPagos(TablaUsuario usuario)
        {
            InitializeComponent();

            Usuario = usuario.Usuario;
            uhid = (int)usuario.UHid;
            Token = usuario.Token;
            Tipo = usuario.Tipo.Trim();

            //Load();

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;

                var item = (TRecibo)e.Item;
               Navigation.PushAsync(new PDFViewer(Token, item.RecID, Tipo, 0, uhid));
            };

            List.RefreshCommand = new Command(() =>
            {
                Load();
            });
        }

        protected override void OnAppearing()
        {
            Load();
            base.OnAppearing(); 
        }

        void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        void Agregar(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AgregaRecibo(uhid, Token, Tipo, 0));
        }


        async void Load()
        {
            if (Tipo == "A")
            {
                agrega.IsVisible = true;

                Usuarios = await Constants.ObtenerUsuariosUH(Token, uhid);
                Recibos = await Constants.ObtenerRecibosUH(Token, (int)uhid);

                if (Usuarios == null || Recibos == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Error", "Ha ocurrido un error inesperado, intentelo más tarde.", "OK");
                }
                else if (Recibos.Count == 0)
                {
                    var pop = new MessageBox("Recibos", "No tiene registrada ningún recibo aún, ¿desea registrar alguno?", 1);

                    pop.OnDialogClosed += (s, arg) =>
                    {
                        if (arg)
                        {
                            Navigation.PushAsync(new AgregaRecibo(uhid, Token, Tipo, 0));
                        }
                        else
                            Navigation.PopAsync();
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                }
                else
                {
                    foreach(var itemR in Recibos)
                    {
                        var RecUsuD = RSA.Decryption(itemR.RecUsu);
                        foreach(var itemU in Usuarios)
                        {
                            var UsuaDecrypt = RSA.Decryption(itemU.Usua);
                            if(RecUsuD == UsuaDecrypt)
                            {
                                TRecibo recibo = new TRecibo
                                {
                                    RecID = itemR.RecID,
                                    RecConcepto = "Concepto: " + itemR.RecConcepto.Trim(),
                                    RecFecha = "Fecha: " + itemR.RecFecha.ToString("dd/MM/yy HH:mm"),
                                    Depto = "Departamento: " + itemU.Departamento.Trim()
                                };

                                Trecibos.Add(recibo);
                            }
                        }
                    }
                }

                List.ItemsSource = Trecibos;
            }
            else if(Tipo == "C")
            {
                Recibos = await Constants.ObtenerRecibosUsua(Token, Usuario);

                if (Recibos == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Error", "Ha ocurrido un error inesperado, intentelo más tarde.", "OK");
                }
                else
                {
                    foreach (var itemR in Recibos)
                    {
                        TRecibo recibo = new TRecibo
                        {
                            RecID = itemR.RecID,
                            RecConcepto = "Concepto: " + itemR.RecConcepto.Trim(),
                            RecFecha = "Fecha: " + itemR.RecFecha.ToString("dd/MM/yy HH:mm")
                        };

                        Trecibos.Add(recibo);
                    }
                }
                List.ItemsSource = Trecibos;
            }

            List.IsRefreshing = false;
        }

    }
}
