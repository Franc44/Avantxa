using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Xamarin.Forms;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using System.Net.Http;
using System.Text;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa.Vistas
{
    public partial class AgregaUHab : ContentPage
    {
        private string colonias = "";
        CodigoPostal.Root json = new CodigoPostal.Root();
        static readonly HttpClient cliente = new HttpClient();

        public AgregaUHab(string token)
        {
            InitializeComponent();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void Registrar(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Nombre.Text) && !string.IsNullOrEmpty(Codigo.Text) && !string.IsNullOrEmpty(Calle.Text) && !string.IsNullOrEmpty(NumExt.Text) && !string.IsNullOrEmpty(colonias))
            {
                Todo.IsVisible = false;
                Act.IsVisible = true;
                Act.IsRunning = true;

                byte[] interior;

                if (!string.IsNullOrEmpty(NumInt.Text))
                    interior = RSA.Encryption(NumInt.Text);
                else
                    interior = RSA.Encryption("SN");

                var Unidad = new UH
                {
                    UnHabMovID = 0,
                    UnHabMovCalle = RSA.Encryption(Calle.Text),
                    UnHabMovColonia = RSA.Encryption(colonias),
                    UnHabMovCP = RSA.Encryption(Codigo.Text),
                    UnHabMovEstado = Estado.Text,
                    UnHabMovEstatus = 1,
                    UnHabMovFecha = DateTime.Now,
                    UnHabMovMunicipio = RSA.Encryption(Municipio.Text),
                    UnHabMovNombre = Nombre.Text,
                    UnHabMovNumExt = RSA.Encryption(NumExt.Text),
                    UnHabMovNumInt = interior,
                };

                try
                {
                    var data = JsonConvert.SerializeObject(Unidad);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    var respond = await cliente.PostAsync(Constants.uri + "UH/", content);

                    if (respond.IsSuccessStatusCode)
                    {
                        var p1 = new MessageBox("Registrado", "La unidad se ha registrado con éxito.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Registrado", "La unidad se ha registrado con éxito.", "Aceptar");
                        await Navigation.PopAsync();
                    }
                    else if(respond.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string refresh = await Constants.RefreshToken();
                        var p1 = new MessageBox("Error", "Su sesión ha expirado, intentelo nuevamente.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Error", "Su sesión ha expirado, intentelo nuevamente.", "Aceptar");
                    }
                    else
                    {
                        var p1 = new MessageBox("Error", "No se ha podido guardar el registro.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Error", "No se ha podido guardar el registro.", "Aceptar");
                    }
                }
                catch(Exception es)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Error", "Ha ocurrido un error inesperado.", "Aceptar");
                    Console.WriteLine(es);
                }

                Todo.IsVisible = true;
                Act.IsVisible = false;
                Act.IsRunning = false;
            }
            else
            {
                var p1 = new MessageBox("Error", "Favor de introducir toda la información requerida arriba.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Error", "Favor de introducir toda la información requerida arriba.", "Aceptar");
            }
        }

        async void Codigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Codigo.Text.Length == 5)
            {
                Asterisco.IsVisible = false;

                //Api codigo postal
                string url = "http://api-sepomex.hckdrk.mx/query/info_cp/" + Codigo.Text + "?type=simplified";

                try
                {
                    var response = new WebClient().DownloadString(url);
                    json = JsonConvert.DeserializeObject<CodigoPostal.Root>(response);

                    if (json == null)
                    {
                        var p1 = new MessageBox("Error", "No se ha podido recuperar la información del Código Postal.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //DisplayAlert("Error", "No se ha podido recuperar la información del Código Postal.", "Aceptar");
                    }
                    else
                    {
                        if (json.Error == false)
                        {
                            Estado.Text = json.Response.Estado;
                            Municipio.Text = json.Response.Municipio;

                            for (int i = 0; i < json.Response.Asentamiento.Count; i++)
                            {
                                Colonia.Items.Add(json.Response.Asentamiento[i]);
                            }

                            DespuesCodigo.IsVisible = true;
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "No se encontro el código postal.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //DisplayAlert("Error", "No se encontro el código postal.", "Aceptar");
                        }
                    }
                }
                catch(Exception es)
                {
                    Console.WriteLine(es);
                    var p1 = new MessageBox("Error", "Introduzca un código postal válido.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //DisplayAlert("Error", "Introduzca un código postal válido.", "Aceptar");
                }
                
            }
            else
            {
                Colonia.Items.Clear();
                DespuesCodigo.IsVisible = false;
                Asterisco.IsVisible = true;
            }
            
        }
        private void Colonia_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Colonia.SelectedIndex;
            if (posicion > -1)
            {
                colonias = json.Response.Asentamiento[posicion];
            }
        }
    }
}
