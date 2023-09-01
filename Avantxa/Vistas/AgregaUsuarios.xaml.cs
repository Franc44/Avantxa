using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using WebApiAvantxa.Security;
using System.Text.RegularExpressions;
using System.Text;
using Rg.Plugins.Popup.Extensions;
using Avantxa.Tools;
using System.Threading.Tasks;

namespace Avantxa.Vistas
{
    public partial class AgregaUsuarios : ContentPage
    {
        static HttpClient cliente = new HttpClient();
        static List<UHs> unidades;
        readonly List<Tipos> tipos = new List<Tipos>();
        static string jwt = "", roles = "";
        static short? unid = 0, estatus = 0;

        public AgregaUsuarios(string Token)
        {
            InitializeComponent();
            jwt = Token;
            Load();
        }

        void Volver(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        class Tipos
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
        }

        async void Load()
        {
            var datos = await Constants.ObtenerUHs(jwt);

            unidades = new List<UHs>();
            if(datos == null)
            {
                var p1 = new MessageBox("Error", "No se ha podido cargar los datos.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                foreach (var item in datos)
                {
                    string Nombre = item.UnHabMovNombre.TrimEnd() + " " + RSA.Decryption(item.UnHabMovCalle) + "," + RSA.Decryption(item.UnHabMovColonia) + "," + RSA.Decryption(item.UnHabMovMunicipio);
                    unidades.Add(new UHs { ID = item.UnHabMovID, Nombre = Nombre });
                    UNIDAD.Items.Add(Nombre);
                }
            }

            tipos.Add(new Tipos { Id = "S", Nombre = "Super" });
            tipos.Add(new Tipos { Id = "A", Nombre = "Administración" });
            tipos.Add(new Tipos { Id = "C", Nombre = "Inquilino" });

            foreach (var item in tipos)
            {
                Rol.Items.Add(item.Nombre);
            }
        }

        async void Agrega(object sender, EventArgs e)
        {
            //Estas variables se ocuparan según el tipo de usario a crear
            string passs;
            byte[] nombreByte;

            //Revisa si los primeros datos has sido llenados
            if (!string.IsNullOrEmpty(Usuario.Text) && !string.IsNullOrEmpty(Email.Text) && !string.IsNullOrEmpty(roles))
            {
                //Válida el formato del correo
                if (Regex.IsMatch(Email.Text, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"))
                {
                    //Se invisiviliza toda la parte blanca del formulario y empieza a correr el Activity Indicator
                    Todo.IsVisible = false;
                    Act.IsVisible = true;
                    Act.IsRunning = true;

                    //Se revisa que tipo de usario para saber que tipo de validaciones  se requiriran
                    if (roles == "S")
                    {
                        //Usuario "Super"
                        if(!string.IsNullOrEmpty(Contra.Text) && !string.IsNullOrEmpty(Nombre.Text))
                        {
                            //Válida el formato de la contraseña para obligar que sea fuerte
                            if(Regex.IsMatch(Contra.Text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,}$"))
                            {
                                passs = Contra.Text;
                                nombreByte = RSA.Encryption(Nombre.Text);

                                //Llama el metodo Guardar para hacer la solicitud al servidor.
                                //Si devuelve true se manda la alerta
                                if(await Guardar(nombreByte, passs))
                                {
                                    var p1 = new MessageBox("Enviar", "El usuario ha sido agregado con éxito.", 0);
                                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                    await Navigation.PopAsync();
                                }
                            }
                            else
                            {
                                var p1 = new MessageBox("Error", "La contraseña debe de contener al menos una letra mayúscula, una letra minúscula, un número y un carácter no alfanúmerico, además de tener una longitud mayor a 8.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                Contra.Text = string.Empty;
                            }

                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "La contraseña o el nombre no pueden ir vacios.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }
                    }
                    else
                    {
                        //Usuarios "Administrador" y "Cliente"
                        if(unid != 0)
                        {
                            passs = GeneraCadena();
                            nombreByte = new byte[10];

                            //Llama el metodo Guardar para hacer la solicitud al servidor.
                            if(await Guardar(nombreByte, passs))
                                //Los demás usuarios se procedera a mandarles un mensaje de correo electronico con su usuario y clave temporal.
                                Correo(Usuario.Text, passs, Email.Text);
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "Seleccione una unidad habitacional para el usuario.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }

                    }
                    //Se visibiliza la sección blanca y se pausa el Acivity Indicator
                    Todo.IsVisible = true;
                    Act.IsVisible = false;
                    Act.IsRunning = false;
                }
                else
                {
                    var p1 = new MessageBox("Error", "El correo electrónico no tiene el formato correcto.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
            }
            else
            {
                var p1 = new MessageBox("Error", "Favor de ingresar el usuario y el correo electrónico.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
        }

        protected async Task<bool> Guardar(byte[] nombreByte, string passs)
        {
            //Se llena el objeto UsuarioActualiza, el cual tiene todos los parametros que existen en la base de datos
            var Registra = new WebApi.UsuarioActualiza
            {
                UsuaMovID = RSA.Encryption(Usuario.Text),
                UnHabMovID = unid,
                UsuaMovNombre = nombreByte,
                UsuaMovTipo = roles,
                MenMovEdif = "",
                MenMovDepto = "",
                UsuaMovFecha = DateTime.Now,
                UsuaMovEstatus = estatus,
                UsuaMovContra = RSA.Encryption(passs),
                UsuaMovEmail = RSA.Encryption(Email.Text),
                UsuaMovAPat = new byte[10],
                UsuaMovAMat = new byte[10]
            };

            try
            {
                //Se serializa en formato Json
                var json = JsonConvert.SerializeObject(Registra);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //Se inicializa una variable para la respuesta de la solicitud
                HttpResponseMessage respond = null;

                //El encabezado principal de la solicitud a la api es Bearer
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                //Se realiza la solicitud a la solicitud
                respond = await cliente.PostAsync(Constants.uri + "Base/", content);

                if (respond.IsSuccessStatusCode) //IsSuccessStatusCode indica todos los códigos de estatus de la solicitud 200´s, si no realiza alguna opción correctamente, revisar en el debug el código.
                {
                    return true;
                }
                else if (respond.StatusCode == HttpStatusCode.Conflict)
                {
                    var p1 = new MessageBox("Registro", "El nombre de usuario que ha introducido ya existe, favor de elegir otro.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    return false;
                }
                else
                {
                    var p1 = new MessageBox("Error", "No se ha actualizado correctamente, favor de intentar más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    return false;
                }
            }
            catch (Exception es)
            {
                var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                Console.WriteLine(es);
                return false;
            }
        }

        string GeneraCadena()
        {
            Guid miGuid = Guid.NewGuid();
            string token = Convert.ToBase64String(miGuid.ToByteArray());
            token = token.Replace("=", "").Replace("+", "");

            return token.Substring(0, 7);
        }

        void Correo(string usuario, string contra, string correo)
        {
            CorreosTemplates correos = new CorreosTemplates();

            string body = correos.CorreoNuevoUsuario(usuario, contra);

            if (correos.EnviarCorreo(body, correo, "", 1))
            {
                var p1 = new MessageBox("Registro", "Se ha registrado con éxito al usuario.", 0);
                App.Current.MainPage.Navigation.PushPopupAsync(p1, true);

                Navigation.PopAsync();
            }
            else
            {
                var p2 = new MessageBox("Error", "El correo no se ha podido enviar.", 0);
                App.Current.MainPage.Navigation.PushPopupAsync(p2, true);
            }
        }

        private void UNIDAD_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = UNIDAD.SelectedIndex;
            if (posicion > -1)
            {
                unid = (short)unidades[posicion].ID;
            }
        }
        private void Rol_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posicion = Rol.SelectedIndex;
            if (posicion > -1)
            {
                roles = tipos[posicion].Id;
                Registro.IsVisible = true;
            }

            if (roles == "S")
            {
                estatus = 1;
                NombreStack.IsVisible = true;
                ContraStack.IsVisible = true;
                UHS.IsVisible = false;
            }
            else
            {
                NombreStack.IsVisible = false;
                ContraStack.IsVisible = false;
                UHS.IsVisible = true;
                estatus = 0;
            }
                
        }
    }
}