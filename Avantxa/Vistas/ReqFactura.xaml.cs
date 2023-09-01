using System;
using System.Collections.Generic;
using System.Net.Mail;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Avantxa.Tools;
using System.Text.RegularExpressions;
using System.Linq;

namespace Avantxa.Vistas
{
    public partial class ReqFactura : ContentPage
    {
        readonly string filepath = "";
        string UsoCFDI, MetPago;

        public ReqFactura(string direccion)
        {
            InitializeComponent();
            filepath = direccion;

            CargaDatosPickers();
        }

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected async void Enviar(object sender, EventArgs e)
        {
            CorreosTemplates correos = new CorreosTemplates();

            if (string.IsNullOrEmpty(Nombre.Text) || string.IsNullOrEmpty(RFC.Text) || string.IsNullOrEmpty(Correo.Text) || string.IsNullOrEmpty(UsoCFDI) || string.IsNullOrEmpty(MetPago))
            {
                var p1 = new MessageBox("Error", "Favor de llenar los datos que se le piden en el formulario.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                if (Regex.IsMatch(Correo.Text, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"))
                {
                    Activity.IsRunning = true;
                    Activity.IsVisible = true;
                    Todo.IsVisible = false;

                    string body = correos.CorreoFactura(Nombre.Text, RFC.Text, Correo.Text, UsoCFDI, MetPago);

                    if(correos.EnviarCorreo(body, "erika.aldama@avantxa.com.mx", filepath, 0))
                    {
                        var p1 = new MessageBox("Registro", "Se ha mandado la información correctamente.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);

                        //Para ir detras mas páginas que la anterior, primero debes de eliminar la página anterior
                        var page2 = Navigation.NavigationStack.FirstOrDefault(p => p.Title == "PDF");
                        if (page2 != null)
                        {
                            Navigation.RemovePage(page2);
                        }
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var p2 = new MessageBox("Error", "El correo no se ha podido enviar.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p2, true);
                    }
                    Activity.IsRunning = false;
                    Activity.IsVisible = false;
                    Todo.IsVisible = true;
                }
                else
                {
                    var p1 = new MessageBox("Error", "El correo no tiene el formato correcto.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
            }
        }

        void CFDI_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = CFDI.SelectedIndex;
            if (posicion > -1)
            {
                UsoCFDI = (string)CFDI.SelectedItem;
            }
        }

        void Pago_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Pago.SelectedIndex;
            if (posicion > -1)
            {
                MetPago = (string)Pago.SelectedItem;
            }
        }

        private void CargaDatosPickers()
        {
            CFDI.Items.Add("Gastos en general.");
            CFDI.Items.Add("Por definir.");

            Pago.Items.Add("Efectivo");
            Pago.Items.Add("Cheque nominativo");
            Pago.Items.Add("Transferencia electrónica de fondos");
            Pago.Items.Add("Tarjeta de crédito");
            Pago.Items.Add("Tarjeta de débito");
            Pago.Items.Add("Por definir");
        }
    }
}