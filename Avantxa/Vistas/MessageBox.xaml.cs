using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBox : PopupPage
    {
        public MessageBox(string titulo, string mensaje,int tipo)
        {
            InitializeComponent();

            LblTitulo.Text = titulo;
            LblMensaje.Text = mensaje;

            if (tipo == 0)
            {
                BtnAceptar.Text = "Aceptar";
                BtnNo.IsVisible = false;    
            }
            else
            {
                BtnAceptar.Text = "Sí";
                BtnNo.IsVisible = true;
            }
        }

        public EventHandler<bool> OnDialogClosed;

       /* void Button_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
        }*/

        void Boton_Aceptar(object sender, EventArgs e)
        {
            OnDialogClosed?.Invoke(this, true);
            App.Current.MainPage.Navigation.PopPopupAsync(true);
        }

        void Boton_Cancelar(object sender, EventArgs e)
        {
            OnDialogClosed?.Invoke(this, false);
            App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
    }
}
