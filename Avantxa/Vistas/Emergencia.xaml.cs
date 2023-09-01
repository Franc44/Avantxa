using Avantxa.Vistas;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Emergencia : ContentPage
    {
        public Emergencia()
        {
            InitializeComponent();
        }
        private void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        private void Emergencias(object sender, EventArgs e)
        {
            _ = LlamadaAsync("911");
        }
        void Denuncia(System.Object sender, System.EventArgs e)
        {
            _ = LlamadaAsync("089");
        }
        void Locatel(System.Object sender, System.EventArgs e)
        {
            _ = LlamadaAsync("56581111");
        }
        void PC(System.Object sender, System.EventArgs e)
        {
            _ = LlamadaAsync("56832222");
        }
        
        public async System.Threading.Tasks.Task LlamadaAsync(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }

            catch (FeatureNotSupportedException ex)
            {
                var p1 = new MessageBox("Error", "Comuniquece al número telefónico:", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
               // DisplayAlert("Llamar", "Comuniquece al número telefónico: " + number, "Aceptar");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                var p1 = new MessageBox("Error", "Algo ha salido mal :(", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //DisplayAlert("Error", "Algo ha salido mal :(", "Aceptar");
                Console.WriteLine(ex);
            }
        }

    }
}