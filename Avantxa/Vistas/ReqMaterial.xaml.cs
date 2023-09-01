using System;
using System.Collections.Generic;
using Avantxa.Tools;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class ReqMaterial : ContentPage
    {
        public ReqMaterial(string DescripcionSol)
        {
            InitializeComponent();

            DesSolicitud.Text = DescripcionSol;
        }

        protected void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void Enviar(object sender, EventArgs e)
        {
            CorreosTemplates correos = new CorreosTemplates();

            Activity.IsRunning = true;
            Activity.IsVisible = true;
            Todo.IsVisible = false;

            if (string.IsNullOrEmpty(Material.Text))
            {
                var p1 = new MessageBox("Error", "Favor de llenar los datos que se le piden en el formulario.", 0);
                App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                string body = correos.CorreoMaterial(DesSolicitud.Text, Material.Text);

                if (correos.EnviarCorreo(body, "erika.aldama@avantxa.com.mx", "", 1))
                {
                    var p1 = new MessageBox("Envio", "Se ha mandado la información correctamente.", 0);
                    App.Current.MainPage.Navigation.PushPopupAsync(p1, true);

                    Navigation.PopAsync();
                }
                else
                {
                    var p2 = new MessageBox("Error", "El correo no se ha podido enviar.", 0);
                    App.Current.MainPage.Navigation.PushPopupAsync(p2, true);
                }
            }

            Activity.IsRunning = false;
            Activity.IsVisible = false;
            Todo.IsVisible = true;
        }
    }
}
