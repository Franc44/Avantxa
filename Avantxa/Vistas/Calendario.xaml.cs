using System;
using System.Collections.Generic;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using Syncfusion;
using Syncfusion.SfCalendar.XForms;
using WebApiAvantxa.Security;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class Calendario : ContentPage
    {
        private readonly string usua;
        private readonly string jwt;
        List<CalendarioC> ListaCal = new List<CalendarioC>();

        public Calendario(string Usuario, string Token)
        {
            InitializeComponent();
            usua = Usuario;
            jwt = Token;
            calendar.Locale = new System.Globalization.CultureInfo("es-MX");
            calendar.InlineItemTapped += Calendar_InlineItemTapped;
        }
        private async void Calendar_InlineItemTapped(object sender, InlineItemTappedEventArgs e)
        {
            var appointment = e.InlineEvent;
            string[] substrings = appointment.Subject.Split('.');

            await Navigation.PushAsync(new AgregaCalendario(int.Parse(substrings[0])));

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            ListaCal = await Constants.ObtenerCalendarioUsua();

            if(ListaCal == null)
            {
                var p1 = new MessageBox("Calendario", "No se han podido recuperar los eventos.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Calendario", "No se han podido recuperar los eventos.", "Aceptar");
            }
            else if(ListaCal.Count == 0)
            {
                var pop = new MessageBox("Calendario", "Usted no posee ningún recordatorio registrado, ¿desea agregar alguno?.", 1);

                pop.OnDialogClosed += async (s, arg) =>
                {
                    if(arg)
                        await Navigation.PushAsync(new AgregaCalendario(0));
                };
                await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
            }
            else
            {
                calendar.ShowInlineEvents = true;
                calendar.BindingContext = new ViewModel(ListaCal);
            }
        }

        async void Volver(Object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void Agregar(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgregaCalendario(0));
        }
    }

    public class ViewModel
    {
        public ViewModel(List<CalendarioC> cal)
        {
            foreach (var item in cal)
            {
                Collcetion.Add(new CalendarInlineEvent() { StartTime = item.CalFecha, EndTime = item.CalFecha.Add(TimeSpan.FromHours(1)), Subject = item.CalID.ToString() + ". " + RSA.Decryption(item.CalTitulo) });
            }
        }

        public CalendarEventCollection Collcetion { get; set; } = new CalendarEventCollection();
    }
}
