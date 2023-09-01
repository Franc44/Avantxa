using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using Avantxa.Vistas;
using Rg.Plugins.Popup.Extensions;
using Avantxa.Tools;

namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrearCuenta : ContentPage
    {

        static HttpClient cliente = new HttpClient();
        static string tipo, jwt;
        static List<UHs> unidades;
        static short? unid;

        public CrearCuenta()
        {
            InitializeComponent();
        }
        protected async void Crear(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Usuario.Text) && !string.IsNullOrEmpty(Clave.Text))
            {
                Verificar.IsVisible = false;
                Act.IsRunning = true;

                var Verifica = new User
                {
                    Usua = RSA.Encryption(Usuario.Text),
                    Contras = RSA.Encryption(Clave.Text),
                    Token = null
                };

                try
                {
                    //Se verfifica que exista el usuario en la base de datos
                    var data = await Constants.Login(Verifica);

                    if (data != null)
                    {
                        jwt = data.Token; //El token es importante en todas las transacciones que se realicen en las acciones en la app
                        tipo = data.Tipo.Trim();

                        if (data.Estatus == 0)
                        {
                            //Una vez verificado de que el usuario existe y que tenga un estatus inactivo, se procede a a hacer visibles la parte del formulario para actualizar su información
                            GUsuario.IsVisible = false;
                            GActualizar.IsVisible = true;
                            Verificar.IsVisible = false;
                            Act.IsRunning = false;
                            Registro.IsVisible = true;
                            Titulo.Text = "Ingresa tus datos";

                            //Se recuperan los datos de las unidades habitacionales existentes en la base de datos
                            var datos = await Constants.ObtenerUHs(jwt);

                            //Se verifica que se hayan traido correctamente los datos
                            if (datos != null)
                            {
                                //La variable de "unidades" es una colección para alamacenar momentaneamente los datos llegados del servidor, pero descifrados. (RSA Cipher -> Strings)
                                unidades = new List<UHs>(); //Para eso la razón de crear un objeto a parte de la UH
                                //Se procede a llenar la coleccion "unidades" desde los datos recuperados de la api
                                foreach (var item in datos)
                                {
                                    string Nombre = item.UnHabMovNombre.TrimEnd() + " " + RSA.Decryption(item.UnHabMovCalle) + "," + RSA.Decryption(item.UnHabMovColonia) + "," + RSA.Decryption(item.UnHabMovMunicipio);
                                    unidades.Add(new UHs { ID = item.UnHabMovID, Nombre = Nombre });
                                    UNIDAD.Items.Add(Nombre);
                                }
                            }
                            else
                            {
                                var p1 = new MessageBox("Verificación", "No se ha podido recuperar la información, intentelo de nuevo más tarde. ", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                        }
                        else
                        {
                            var p1 = new MessageBox("Verificación", "Tu cuenta ya ha sido activada. ", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            await Navigation.PushAsync(new Inicio());
                        }
                    }
                    else
                    {
                        var p1 = new MessageBox("Verificación", "Usuario o clave de acceso erroneos.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Verificación", "Usuario o clave de acceso erroneos. ", "Aceptar");
                        Verificar.IsVisible = true;
                        Act.IsRunning = false;
                    }
                }
                catch (Exception es)
                {
                    var p1 = new MessageBox("Verificación", "Ha ocurrido un error inesperado, intentalo más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Verificación", "Ha ocurrido un error inesperado, intentalo más tarde.", "Aceptar");
                    Verificar.IsVisible = true;
                    Act.IsRunning = false;
                    Console.WriteLine(es);
                }
            }
            else
            {
                var p1 = new MessageBox("Error", "Ingrese usuario y clave.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
        }

        protected async void Registrar(object sender, EventArgs e)
        {
            //Se válida todo lo necesario para mandar la solicitud a la api
            if (!string.IsNullOrEmpty(Nombre.Text) && !string.IsNullOrEmpty(Paterno.Text) && !string.IsNullOrEmpty(Materno.Text)
                && !string.IsNullOrEmpty(Edificio.Text) && !string.IsNullOrEmpty(Departamento.Text)
                && !string.IsNullOrEmpty(Correo.Text) && !string.IsNullOrEmpty(Contra.Text) && !string.IsNullOrEmpty(Contra1.Text))
            {
                if (Contra.Text == Contra1.Text)
                {
                    //Validación de formatos
                    if (Regex.IsMatch(Contra.Text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,}$"))
                    {
                        if (Regex.IsMatch(Correo.Text, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"))
                        {
                            Registro.IsVisible = false;
                            Act.IsRunning = true;

                            try
                            {
                                //Llenado de objetos 
                                var Actualizar = new UsuarioActualiza
                                {
                                    UsuaMovID = RSA.Encryption(Usuario.Text),
                                    UnHabMovID = unid,
                                    UsuaMovNombre = RSA.Encryption(Nombre.Text),
                                    UsuaMovTipo = tipo,
                                    MenMovEdif = Edificio.Text,
                                    MenMovDepto = Departamento.Text,
                                    UsuaMovFecha = DateTime.Now,
                                    UsuaMovEstatus = 1,
                                    UsuaMovContra = RSA.Encryption(Contra.Text),
                                    UsuaMovEmail = RSA.Encryption(Correo.Text),
                                    UsuaMovAPat = RSA.Encryption(Paterno.Text),
                                    UsuaMovAMat = RSA.Encryption(Materno.Text)
                                };

                                //Serialización de objeto anterior a JSON (Conversión)
                                var json = JsonConvert.SerializeObject(Actualizar);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                //Inicialización de la variable para hacer la solictud a la api
                                HttpResponseMessage respond = null;

                                //Adición de Header para la autorización en la api
                                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                                //Ejecución de la solicitud a la api
                                respond = await cliente.PostAsync(Constants.uri + "Base/Actualiza/", content);

                                //Verificación de respuesta de la api
                                if (respond.IsSuccessStatusCode)
                                {
                                    var p1 = new MessageBox("Registro", "Sus datos se han actual izado correctamente.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    await Navigation.PushAsync(new Inicio());
                                }
                                else
                                {
                                    var p1 = new MessageBox("Error", "No se ha actualizado correctamente, favor de intentar más tarde.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    //await DisplayAlert("Error", "No se ha actualizado correctamente, favor de intentar más tarde.", "Aceptar");
                                    Contra.Text = string.Empty;
                                    Contra1.Text = string.Empty;
                                    Registro.IsVisible = true;
                                    Act.IsRunning = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                //await DisplayAlert("Error", "Ha ocurrido un error inesperado.", "Aceptar");
                                Contra.Text = string.Empty;
                                Contra1.Text = string.Empty;
                                Registro.IsVisible = true;
                                Act.IsRunning = false;
                                Console.WriteLine(ex);
                            }
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "El correo no tiene el formato correcto.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            //await DisplayAlert("Error", "El correo no tiene el formato correcto.", "Aceptar");
                            Contra.Text = string.Empty;
                            Contra1.Text = string.Empty;
                        }
                    }
                    else
                    {
                        var p1 = new MessageBox("Error", "La contraseña debe de contener al menos una letra mayúscula, una letra minúscula, un número y un caraterer no alfanúmerico.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        //await DisplayAlert("Error", "La contraseña debe de contener al menos una letra mayúscula, una letra minúscula, un número y un caraterer no alfanúmerico.", "Aceptar");
                        Contra.Text = string.Empty;
                        Contra1.Text = string.Empty;
                    }
                }
                else
                {
                    var p1 = new MessageBox("Error", "Las contraseñas deben de coincidir.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Error", "Las contraseñas deben de coincidir.", "Aceptar");
                    Contra.Text = string.Empty;
                    Contra1.Text = string.Empty;
                }
            }
            else
            {
                var p1 = new MessageBox("Error", "Favor de ingresar los datos que se le solicitan.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Error", "Favor de ingresar los datos que se le solicitan.", "Aceptar");
                Contra.Text = string.Empty;
                Contra1.Text = string.Empty;
            }
        }

        async void Volver(Object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void UNIDAD_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = UNIDAD.SelectedIndex;
            if (posicion > -1)
            {
                unid = (short)unidades[posicion].ID;
            }
        }
    }
}