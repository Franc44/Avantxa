using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Avantxa.WebApi;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Avantxa.Vistas
{
    public partial class AgregaRecibo : ContentPage
    {
        List<Usuario.Usuar> departamentos = new List<Usuario.Usuar>();
        private DateTime Fecha = DateTime.Today;
        private DateTime Fecha1 = DateTime.Today;
        private byte[] userid = null;
        readonly int UHId, RecId;
        string jwt, tipo;
        private string ConceptoText { get; set; } = "";
        public bool ReqPeriodo { get; set; } = false;

        public AgregaRecibo(int uhid, string Token, string rol, int recid)
        {
            InitializeComponent();

            Title = "Recibo";

            UHId = uhid;
            jwt = Token;
            tipo = rol;
            RecId = recid;

            //Se incluye perido
            //Se se agrega conceptos nuevos que desplieguen peridos, se deberá modificar la condicinal 'If' del método 'Concepto_SelectedIndexChanged'
            Concepto.Items.Add("Renta");
            Concepto.Items.Add("Agua");
            Concepto.Items.Add("Luz");
            Concepto.Items.Add("Estacionamiento");
            Concepto.Items.Add("Gas");
            //Sin periodo
            Concepto.Items.Add("Limpieza Roof Garden");
            Concepto.Items.Add("Mantenimiento");
            Concepto.Items.Add("Paquetería");
            Concepto.Items.Add("Otros");

            Load();
        }

        void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void Load()
        {
            departamentos = await Constants.ObtenerUsuariosUH(jwt, UHId);
            if (departamentos.Count == 0 || departamentos == null)
            {
                var p1 = new MessageBox("Recibos", "No se ha podido cargar los datos correctamente, intetelo nuevamente más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                foreach (var item in departamentos)
                {
                    Deptos.Items.Add(item.Departamento);
                }

            }
        }

        async void Agrega(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            if (Fecha > Fecha1)
            {
                var p1 = new MessageBox("Error", "La fecha inicial no puede ser mayor a la final", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                if(userid == null && string.IsNullOrEmpty(ConceptoText) && string.IsNullOrEmpty(Cantidad.Text))
                {
                    var p1 = new MessageBox("Error", "Introduzca los datos que se le piden arriba.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else
                {
                    string periodo = ReqPeriodo ? ",</u> correspondiente al periodo del <u>" + Fecha.Day.ToString() + " de " + Fecha.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " del " + Fecha.Year + " al " + Fecha.Day.ToString() + " de " + Fecha.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " del " + Fecha.Year + "</u>" : ".</u>";

                    var ReciboPdf = new PDFrequest
                    {
                        UHId = (short)UHId,
                        NoRecibo = NoRecibo.Text,
                        Usuario = userid,
                        Cantidad = Cantidad.Text,
                        Concepto = ConceptoText,
                        Periodo = periodo
                    };

                    Activity.IsRunning = true;
                    Activity.IsVisible = true;
                    Todo.IsVisible = false;
                    try
                    {
                        var json = JsonConvert.SerializeObject(ReciboPdf);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        HttpResponseMessage responseMessage = null;

                        //Se decide si se añade un registro o se actualiza
                        if (RecId == 0)
                            responseMessage = await client.PostAsync(Constants.uri + "Recibos/Agrega/", content);
                        else
                            responseMessage = await client.PutAsync(Constants.uri + "Recibos/" + RecId, content);


                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var p1 = new MessageBox("Enviar", "El registro del recibo se ha guardado con éxito", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);

                            string resultado = await responseMessage.Content.ReadAsStringAsync();
                            var data = JsonConvert.DeserializeObject<int>(resultado);

                            if (RecId == 0)
                                await Navigation.PushAsync(new PDFViewer(jwt, data, tipo, 1, UHId));
                            else
                                await Navigation.PushAsync(new PDFViewer(jwt, data, tipo, 2, UHId));

                        }
                        else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            jwt = await Constants.RefreshToken();
                            var p1 = new MessageBox("Enviar", "Su sesión se ha agotado, vuelva a intentarlo.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                        else
                        {
                            var p1 = new MessageBox("Enviar", "No se ha podido guardar el registro.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }

                    }
                    catch (Exception es)
                    {
                        var p1 = new MessageBox("Enviar", "Ha ocurrido un error al guardar el registro.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        Console.WriteLine(es);
                    }
                    Activity.IsRunning = false;
                    Activity.IsVisible = false;
                    Todo.IsVisible = true;
                }
            }
        }

        private void Concepto_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Concepto.SelectedIndex;
            if (posicion > -1)
            {
                ConceptoText = (string)Concepto.SelectedItem;
            }

            //Se visualiza periodo dependiendo a las primeras posiciones
            //El cinco se cambia dependiendo cuantos conceptos requieran los periodos
            if(posicion > -1 && posicion < 5)
            {
                Periodo.IsVisible = true;
                ReqPeriodo = true;
            }
            else
            {
                Periodo.IsVisible = false;
                ReqPeriodo = false;
            }

            if (ConceptoText == "Renta")
                Numero.IsVisible = true;
            else
                Numero.IsVisible = false;
        }

        private void SeleccionFecha(object sender, DateChangedEventArgs e)
        {
            Fecha = e.NewDate;
        }
        private void SeleccionFecha1(object sender, DateChangedEventArgs e)
        {
            Fecha1 = e.NewDate;
        }

        private void Usuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Deptos.SelectedIndex;
            if (posicion > -1)
            {
                userid = departamentos[posicion].Usua;
            }
        }
    }
}
