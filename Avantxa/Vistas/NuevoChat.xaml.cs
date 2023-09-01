using System;
using System.Collections.Generic;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class NuevoChat : ContentPage
    {
        private static TablaUsuario usuario = new TablaUsuario();
        private string Motivoseleccionado { get; set; } = "";
        private static List<Usuario.Usuar> departamentos = new List<Usuario.Usuar>();
        private static byte[] IdUsuarioFin = new byte[10];

        public NuevoChat(TablaUsuario user)
        {
            InitializeComponent();
            usuario = user;
        }

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Motivo.Items.Add("Mantenimiento");
            Motivo.Items.Add("Roof Garden");
            Motivo.Items.Add("Pagos");
            Motivo.Items.Add("Otros");

            Load();
        }

        private async void Load()
        {
            departamentos = await Constants.ObtenerDepartmanetos(usuario.Token, (int)usuario.UHid);

            if(departamentos == null)
            {
                var p1 = new MessageBox("Error", "Ha ocuurido un error inesperado, intentelo más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                await Navigation.PopAsync();
            }
            else
            {
                if (usuario.Tipo.Trim() == "C")
                {
                    Leyenda.Text = "Seleccione al usuario:";
                    foreach (var item in departamentos)
                    {
                        if(item.Tipo.Trim() != "C")
                            Deptos.Items.Add(item.Departamento.Trim() + ": " + RSA.Decryption(item.Nombre));
                    }
                }
                else
                {
                    foreach (var item in departamentos)
                    {
                        string usuaDecryp = RSA.Decryption(item.Usua);

                        if(usuaDecryp != usuario.Usuario)
                        {
                            Deptos.Items.Add(item.Departamento.Trim() + ": " + RSA.Decryption(item.Nombre));
                        }
                    }
                }
            }
        }

        private void Motivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Motivoseleccionado = (string)Motivo.SelectedItem;

            if (IdUsuarioFin.Length > 250)
                GEnviar.IsVisible = true;
        }

        private void Usuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Deptos.SelectedIndex;
            if (posicion > -1)
            {
                IdUsuarioFin = departamentos[posicion].Usua;
            }

            if (!string.IsNullOrEmpty(Motivoseleccionado))
                GEnviar.IsVisible = true;
        }

        protected void Iniciar(object sender, EventArgs e)
        {
            ChatM.ChatMen chatMen = new ChatM.ChatMen
            {
                Identifica = 3,
                ChatMovil = new ChatM.ChatMovil
                {
                    ChID = 0,
                    ChEstatus = 1,
                    ChFecha = DateTime.Now,
                    ChMotivo = Motivoseleccionado,
                    ChSoliId = 0,
                    ChUsuaFin = IdUsuarioFin,
                    ChUsuaInic = RSA.Encryption(usuario.Usuario)
                }
            };

            var pop = new ChatBox(chatMen, usuario.Token);
            App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }
    }
}
