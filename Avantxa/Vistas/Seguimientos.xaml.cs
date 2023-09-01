using Avantxa.Modelos;
using Avantxa.Vistas;
using Avantxa.WebApi;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using WebApiAvantxa.Security;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Avantxa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Seguimientos : ContentPage
    {
        List<ListaMensajes> Mensajes = new List<ListaMensajes>();
        private List<Tmensajes> ListMensajes { get; set; } = new List<Tmensajes>();
        private readonly string jwt;
        private readonly string depa;
        private readonly string tipo;
        readonly int id = 0;
        public ICommand OnSolicitudCommand { get; set; }

        public Seguimientos(TablaUsuario usuario)
        {
            InitializeComponent();
            jwt = usuario.Token;
            depa = usuario.Departamento.Trim();
            id = (int)usuario.UHid;
            tipo = usuario.Tipo.Trim();

            List.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                //Si no se selecciono nada sigue haciendo lo que haces
                if (e.Item == null) return;
                //Cuando se seleccione una fila del ListView se debe de volver a deseleccionar
                if (sender is ListView lv) lv.SelectedItem = null;

                //Se realiza la conversión de la fila seleccionada al objeto que deseas solicitar todos los datos
                var item = (Tmensajes)e.Item;
                Navigation.PushAsync(new SolicitudV(item.ID));
            };

            //Ejecuta el código para actualizar el ListView
            List.RefreshCommand = new Command(() =>
            {
                List.IsRefreshing = true;
                Load();
            });

            //Manda a llamar otra pantalla
            OnSolicitudCommand = new Command(execute: () => { Navigation.PushAsync(new SolicitudAdministrador(usuario)); });

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            //OnAppering se utiliza sobre todo cuando existe listviews, al tener la propiedad de que al volver de una página consecuente del stack de páginas(Navigation.PopAsync),  se ejecute el código en el cual se encuentre
            //base.OnAppearing(); esto nos ayuda a recargar el ListView que tenemos en la página
            base.OnAppearing();
            Load();
        }

        protected async void Load()
        {
            //Se prepara el ListView
            List.HasUnevenRows = false;
            List.IsGroupingEnabled = false;
            //Estas variables se inicializan por ser ocupadas en ambos tipos del usuario
            string estatus = "", motivo = "", Depto = "";
            Tmensajes lmsj = new Tmensajes();
            //Se limpia los datos de la lista
            ListMensajes.Clear();
            

            //se checa cual de los usuarios hace la solictud para mostrar los datos correspondientes
            if (tipo == "A")
            {
                BSolicitud.IsVisible = true;
                //Se cambia el titulo dependiendo el usuario
                Titulo.Text = "Solicitudes";
                //Se hace la solicitud al servidor para obtener los mensajes de la unidad habitacional
                //En los dos usuarios se utilizan el mismo metodo, pero se define en la última sobrecarga
                //0 indica que el método solicitará los datos de un usuario en específico; 1 indica que se solicitará los mensajes de toda la unidad 
                Mensajes = await Constants.Mensajes(depa, id, jwt, 1);

                //Se válida que el resultante de la solicitud no venga vacio o nulo.
                if (Mensajes == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }  
                else if (Mensajes.Count == 0)
                {
                    var p1 = new MessageBox("Seguimientos", "No tiene registrada ninguna solicitud.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    await Navigation.PopAsync();
                }
                else
                {
                    foreach (var item in Mensajes)
                    {
                        Depto = "Solicitud de: " + item.MenMovDeDepto.Trim();
                        estatus = item.MenMovEstatus == 0 ? "Enviada" : item.MenMovEstatus == 1 ? "Recibida" : item.MenMovEstatus == 2 ? "En proceso" : "Completada";

                        switch (item.MenMovMotivo.Trim().ToString())
                        {
                           case "A":
                                estatus = item.MenMovEstatus == 0 ? "En proceso" : "Completado";
                                Depto = "Mantenimiento ";
                                motivo = "";
                                break;
                            case "M":
                                motivo = "Mantenimiento";
                                break;
                            case "R":
                                estatus = item.MenMovEstatus == 0 ? "Enviada" : item.MenMovEstatus == 1 ? "Espera" : "Aprobada";
                                motivo = "Roof Garden";
                                break;
                            case "O":
                                motivo = "Otros";
                                break;
                            case "V":
                                estatus = item.MenMovEstatus == 0 ? "Enviada" : item.MenMovEstatus == 1 ? "Recibida" : "Completada";
                                motivo = "Vacaciones";
                                break;
                            default:
                                break;
                        }

                        lmsj = new Tmensajes
                        {
                            Depto = Depto.Trim(),
                            ID = item.MenMovID,
                            Fecha = "Fecha: " + Convert.ToDateTime(item.MenMovFecha).ToString("dd/MM/yy HH:mm"),
                            Estatus = estatus,
                            Motivo = motivo
                        };

                        ListMensajes.Add(lmsj);
                    }
                    List.ItemsSource = ListMensajes;
                }
            }
            else
            {
                Titulo.Text = "Seguimientos";
                //Se hace la solicitud de obtención de mensajes de la api
                Mensajes = await Constants.Mensajes(depa, id, jwt, 0);

                //Se verifica que los mensajes hayan sido recuperados con éxito o que si tenga
                if (Mensajes == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo nuevamente más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else if (Mensajes.Count == 0)
                {
                    //Si no tiene mensajes, se le pregunta si desea agregar
                    var pop = new MessageBox("Seguimientos", "No tiene registrada ninguna solicitud enviada, ¿desea registrar alguna?", 1);

                    pop.OnDialogClosed += (s, arg) =>
                    {
                        if (arg)
                        {
                            Navigation.PushAsync(new Solicitudes());
                        }
                        else
                            Navigation.PopAsync();
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                }
                else
                {
                    //Se recorre la lista del JSON entrante de la api
                    foreach (var item in Mensajes)
                    {
                        //Cada mensaje se le asigna sus especificación del estatus y el motivo
                        estatus = item.MenMovEstatus == 0 ? "Enviada" : item.MenMovEstatus == 1 ? "Recibida" : item.MenMovEstatus == 2 ? "En proceso" : "Completada";//Este se pone afuera porque se repite en en dos ocaciones en las solicitudes
                        switch (item.MenMovMotivo.Trim().ToString())
                        {
                            case "M":
                                motivo = "Mantenimiento";
                                break;
                            case "R":
                                estatus = item.MenMovEstatus == 0 ? "Enviada" : item.MenMovEstatus == 1 ? "Espera" : "Aprobada";
                                motivo = "Roof Garden";
                                break;
                            case "O":
                                motivo = "Otros";
                                break;
                            case "V":
                                estatus = item.MenMovEstatus == 0 ? "Enviada" : "Recibida";
                                motivo = "Vacaciones";
                                break;
                            default:
                                break;
                        }

                        DateTime Fecha = (DateTime)item.MenMovFecha;

                        lmsj = new Tmensajes
                        {
                            //El list view no se ajusta, así que lo arreglamos a vergazos, por eso se desacomada así.
                            Depto = estatus,
                            ID = item.MenMovID,
                            Fecha = motivo,
                            Estatus = "Fecha: " + Fecha.ToString("dd/MM/yy HH:mm")
                        };
                        ListMensajes.Add(lmsj);
                    }
                    List.ItemsSource = ListMensajes;
                }
            }
            List.HasUnevenRows = true;

            List.IsRefreshing = false;
        }

        protected void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}