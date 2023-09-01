using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class AgregaAnuncios : ContentPage
    {
        public ICommand OnBackCommand { get; set; }
        public ICommand OnSaveCommand { get; set; }
        private List<Usuario.Usuar> Departamentos { get; set; } = new List<Usuario.Usuar>();
        public byte[] Userid { get; set; } = new byte[10];
        private string Jwt { get; set; } = "";
        private int UhId { get; set; } = 0;

        public AgregaAnuncios(TablaUsuario usuario)
        {
            InitializeComponent();
            Jwt = usuario.Token;
            UhId = (int)usuario.UHid;

            OnBackCommand = new Command(execute: () => {  Navigation.PopAsync(); });
            OnSaveCommand = new Command(async() =>
            {
                if(!string.IsNullOrEmpty(Asunto.Text) && !string.IsNullOrEmpty(Mensaje.Text) && Userid.Length > 250)
                {
                    var Anuncio = new AnunciosW
                    {
                        AnID = 0,
                        AnAsunto = Asunto.Text,
                        AnFecha = DateTime.Now,
                        AnMensaje = Mensaje.Text,
                        AnUHId = UhId,
                        AnUsuario = Userid
                    };

                    if(await Constants.MandarAnuncio(Jwt, Anuncio))
                    {
                        var p1 = new MessageBox("Anuncio", "El anuncio ha sido guardado correctamente.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var p1 = new MessageBox("Error", "Su anuncio no ha sido guradado, favor de intentarlo más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    }
                }
                else
                {
                    var p1 = new MessageBox("Error", "Favor de llenar los datos de arriba.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
            });

            BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Deptos.Items.Clear();
            Departamentos.Clear();

            Departamentos = await Constants.ObtenerUsuariosUH(Jwt, UhId);
            if (Departamentos.Count == 0 || Departamentos == null)
            {
                var p1 = new MessageBox("Error", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                Departamentos.Add(new Usuario.Usuar { Usua = RSA.Encryption("Todos"), Departamento = "Todos", Estatus = 0, Nombre = new byte[64], Tipo = "", Token = "", UHid = 0 });
                foreach (var item in Departamentos)
                {
                    Deptos.Items.Add(item.Departamento);
                }

            }
        }

        private void Usuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Deptos.SelectedIndex;
            if (posicion > -1)
            {
                Userid = Departamentos[posicion].Usua;
            }
        }

    }
}
