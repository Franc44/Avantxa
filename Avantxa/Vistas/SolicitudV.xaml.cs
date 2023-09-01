using System;
using System.Collections.Generic;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using System.IO;
using Rg.Plugins.Popup.Extensions;

namespace Avantxa.Vistas
{
    public partial class SolicitudV : ContentPage
    {
        byte[] fileByte = new byte[64];
        DateTime Completa = new DateTime();
        private DateTime Fecha = DateTime.Today;
        private readonly int MenId = 0;
        Mensajes Mensajes = new Mensajes();
        TablaUsuario usuario = new TablaUsuario();
        private string motivo = "";
        CalendarioC Calendar = new CalendarioC();
        int Limpieza = 0, horario = 0;

        public SolicitudV(int id)
        {
            InitializeComponent();
            MenId = id;

            LoadData();
        }

        private async void LoadData()
        {
            //Se cargan los datos del usuario desde la base local
            usuario = await App.Database.GetItemsAsync();

            //Aquì se utiliza una variable temporal, solo para poder usarla en la siguiente solicitud
            string jwt = usuario.Token;

            //Se requiere los datos de la solicitud al servidor
            Mensajes = await Constants.MensajesSolo(MenId, jwt);

            //Sea válida
            if (Mensajes == null)
            {
                var p1 = new MessageBox("Carga", "No se ha podido cargar la información de la solicitud.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                await Navigation.PopAsync();
            }
            else
            {
                DesplegarDatos();
            }
        }

        private void DesplegarDatos()
        {
            //Datos en general de la solicitud
            //Esta solo se utiliza en dos casos, así que se cambia
            VEstatus.Text = Mensajes.MenMovEstatus == 0 ? "Enviada" : Mensajes.MenMovEstatus == 1 ? "Recibida" : Mensajes.MenMovEstatus == 2 ? "En proceso" : "Completado";
            switch (Mensajes.MenMovMotivo.Trim().ToString())
            {
                case "M":
                    motivo = "Mantenimiento";
                    OrdenaMante();
                    break;
                case "R":
                    VEstatus.Text = Mensajes.MenMovEstatus == 0 ? "Enviada" : Mensajes.MenMovEstatus == 1 ? "Espera" : "Aprobada";
                    motivo = "Roof Garden";
                    DespliegaCalendario();
                    OrdenaRoof();
                    break;
                case "O":
                    motivo = "Otros";
                    DespliegaCalendario();
                    OrdenaOtros();
                    break;
                case "V":
                    VEstatus.Text = Mensajes.MenMovEstatus == 0 ? "Enviada" : Mensajes.MenMovEstatus == 1 ? "Recibida" : "Completada";
                    motivo = "Vacaciones";
                    OrdenaVacaciones();
                    break;
                case "A":
                    VEstatus.Text = Mensajes.MenMovEstatus == 0 ? "Enviada" : Mensajes.MenMovEstatus == 1 ? "En proceso" : "Completada";
                    motivo = "Mantenimiento";
                    OrdenaMantenimientoAdmin();
                    break;
                default:
                    break;
            }
            //Los demás datos
            VDe.Text = usuario.Tipo.Trim() == "A" && Mensajes.MenMovMotivo.Trim() != "A" ? "De: " + RSA.Decryption(Mensajes.MenMovDeNombre) + ", " + Mensajes.MenMovDeDepto.Trim() : "";
            VMotivo.Text = "Motivo: " + motivo;
            VMensaje.Text = "Mensaje: " + RSA.Decryption(Mensajes.MenMovMensaje);
            VFechaE.Text = "Fecha enviado: " + Convert.ToDateTime(Mensajes.MenMovFecha).ToString("dd/MM/yy H:mm");
            VFechaEntrega.Text = Mensajes.MenMovFechaEntrega != DateTime.Parse("01-01-2000") && Mensajes.MenMovFechaEntrega != null ? "Fecha de enterado: " + Convert.ToDateTime(Mensajes.MenMovFechaEntrega).ToString("dd/MM/yy H:mm") : "";

            //Si la solicitud tiene una imagen se depliega
            if (Mensajes.MenMovFotoA.Length > 512)
            {
                var ms = new MemoryStream(Mensajes.MenMovFotoA);
                image1.Source = ImageSource.FromStream(() => ms);
                GridFoto1.IsVisible = true;
            }

            //Si hay foto de respuesta y mensaje, se despliega
            if (Mensajes.MenMovMensajeRes.Length >= 256)
            {
                VMensajeR.Text = Mensajes.MenMovMotivo.Trim() == "R" || Mensajes.MenMovMotivo.Trim() == "V" ? "Mensaje de la administración:\n" + RSA.Decryption(Mensajes.MenMovMensajeRes) : "Trabajo realizado:\n" + RSA.Decryption(Mensajes.MenMovMensajeRes);

                if (Mensajes.MenMovFotoD.Length > 512)
                {
                    MenAdmin.IsVisible = true;
                    var ms = new MemoryStream(Mensajes.MenMovFotoD);
                    image2.Source = ImageSource.FromStream(() => ms);
                    GridFoto2.IsVisible = true;
                }
            }

            //Verificar todos los Label's de los datos principales para saber si estan vacios, si estan vacios los hace invisibles
            //Solo son los Label dentro del StackLayout TextContainer
            foreach (View el in TextContainer.Children)
            {
                if (el.GetType() == typeof(Label))
                {
                    Label j = (Label)el;

                    if (string.IsNullOrEmpty(j.Text))
                    {
                        j.IsVisible = false;
                    }
                }
            }

            Todo.IsVisible = true;
            Act.IsRunning = false;
            Act.IsVisible = false;
        }

        protected async void Mantenimiento(object sender, EventArgs e)
        {
            Mensajes mensajeT = new Mensajes();
            int MetodoActualizacion = 0;
            byte[] observacion = !string.IsNullOrEmpty(TObservaciones.Text) ? RSA.Encryption(TObservaciones.Text) : new byte[10];

            //Empieza validaciones dependiendo los estatus y el motivo de la solicitud

            //Empezamos con el matenimiento y otros
            if (Mensajes.MenMovEstatus == 1 && Mensajes.MenMovMotivo.Trim() != "A")
            {
                //se necesitan los responsables esta parte
                if (string.IsNullOrEmpty(TRespobsables.Text))
                {
                    var p1 = new MessageBox("Error", "Favor de llenar el recuadro de responsables.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //Como no se cumple se sale del void
                    return;
                }

                mensajeT = new Mensajes
                {
                    MenMovID = Mensajes.MenMovID,
                    MenMovDeDepto = "",
                    MenMovDeNombre = new byte[10],
                    MenMovEstatus = 0,
                    MenMovFecha = DateTime.Now,
                    MenMovFotoA = new byte[10],
                    MenMovFotoD = new byte[10],
                    MenMovMensaje = new byte[10],
                    MenMovMensajeRes = new byte[10],
                    MenMovMotivo = "",
                    UnHabMovID = 0,
                    MenMovFechaEntrega = DateTime.Now,
                    MenMovFechaFinal = DateTime.Now,
                    MenMovObservaciones = new byte[10],
                    MenMovResponsables = RSA.Encryption(TRespobsables.Text)
                };

                MetodoActualizacion = 1;
            }
            if (Mensajes.MenMovEstatus == 2 && Mensajes.MenMovMotivo.Trim() != "A")
            {
                if ((!string.IsNullOrEmpty(TMensajeRes.Text) && fileByte.Length > 100 && Mensajes.MenMovMotivo.Trim() == "M") || (!string.IsNullOrEmpty(TMensajeRes.Text) & Mensajes.MenMovMotivo.Trim() == "O"))
                {
                    var p1 = new MessageBox("Error", "Favor de llenar los campos de arriba.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //Como no se cumple se sale del void
                    return;
                }

                mensajeT = new Mensajes
                {
                    MenMovID = Mensajes.MenMovID,
                    MenMovDeDepto = "",
                    MenMovDeNombre = new byte[10],
                    MenMovEstatus = 0,
                    MenMovFecha = DateTime.Now,
                    MenMovFotoA = new byte[10],
                    MenMovFotoD = fileByte,
                    MenMovMensaje = new byte[10],
                    MenMovMensajeRes = RSA.Encryption(TMensajeRes.Text),
                    MenMovMotivo = "",
                    UnHabMovID = 0,
                    MenMovFechaEntrega = DateTime.Now,
                    MenMovFechaFinal = DateTime.Now,
                    MenMovObservaciones = observacion,
                    MenMovResponsables = new byte[10]
                };

                MetodoActualizacion = 2;
            }

            if(Mensajes.MenMovEstatus == 1 && Mensajes.MenMovMotivo.Trim() == "A")
            {
                if (string.IsNullOrEmpty(TMensajeRes.Text) || fileByte.Length < 100 || string.IsNullOrEmpty(TRespobsables.Text))
                {
                    var p1 = new MessageBox("Error", "Favor de llenar los campos de arriba.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //Como no se cumple se sale del void
                    return;
                }

                mensajeT = Mensajes;

                mensajeT.MenMovMensajeRes = RSA.Encryption(TMensajeRes.Text);
                mensajeT.MenMovFotoD = fileByte;
                mensajeT.MenMovObservaciones = observacion;
                mensajeT.MenMovResponsables = RSA.Encryption(TRespobsables.Text);
                mensajeT.MenMovFechaFinal = DateTime.UtcNow;
                mensajeT.MenMovEstatus = 2;
            }

            //Confirmar que quieren hacer la consulta
            var pop = new MessageBox("Solicitud", "¿Esta seguro de realizar esta acción?", 1);

            pop.OnDialogClosed += async (s, arg) =>
            {
                if (arg)
                {
                    Todo.IsVisible = false;
                    Act.IsRunning = true;
                    Act.IsVisible = true;

                    //Respuesta afirmativa, realizas la solicitud a la api
                    if (await Constants.ActualizaMensajes(usuario.Token, mensajeT, MetodoActualizacion) == 1)
                    {
                        var p1 = new MessageBox("Respuesta", "Mensaje enviado.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var p1 = new MessageBox("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    }

                    Todo.IsVisible = true;
                    Act.IsRunning = false;
                    Act.IsVisible = false;
                }
            };
            await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }

        async void CambiaFecha(object sender, EventArgs e)
        {
            Completa = Fecha.Add(TimePicker.Time);

            if (Completa.Date <= DateTime.Now.Date)
            {
                var p1 = new MessageBox("Solicitud", "La fecha debe ser posterior a la fecha de hoy.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Enviar", "La fecha debe ser posterior a la fecha de hoy.", "Aceptar");
            }
            else
            {
                Todo.IsVisible = false;
                Act.IsRunning = true;
                Act.IsVisible = true;

                if (Mensajes.MenMovMotivo == "R")
                {
                    if (horario > 0)
                    {
                        if (Completa.DayOfWeek == DayOfWeek.Sunday || Completa.DayOfWeek == DayOfWeek.Saturday || Completa.DayOfWeek == DayOfWeek.Friday)
                        {
                            int revisa = await Constants.RevisaFechaRoof(usuario.Token, Completa, Calendar.CalID, horario);
                            if (revisa == 1)
                                ActualizaCalendario();
                            else if (revisa == 2)
                            {
                                var p1 = new MessageBox("Solicitud", "Esta fecha y horario ya han sido apartado con anterioridad, por favor seleccione otro horario.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                            else
                            {
                                var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, favor de intentar más tarde.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                        }
                        else
                        {
                            var p1 = new MessageBox("Error", "Los días deben ser de viernes a domingo.", 0);
                            await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        }

                    }
                    else
                    {
                        var p1 = new MessageBox("Error", "Favor de elegir un horario.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    }
                }
                else
                    ActualizaCalendario();

                Todo.IsVisible = true;
                Act.IsRunning = false;
                Act.IsVisible = false;
            }
        }

        async void ActualizaCalendario()
        {
            Calendar.CalFecha = Completa;
            Calendar.CalTurno = (short)horario;
            Calendar.CalLimpieza = (short)Limpieza;

            var Act = await Constants.ActualizaCalendario(Calendar, Calendar.CalID, usuario.Token);

            if (Act == 0)
            {
                var p1 = new MessageBox("Respuesta", "No se ha podido modificar, intentelo más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            } 
            else if (Act == 1)
            {
                var p1 = new MessageBox("Respuesta", "Se ha actualizado exitosamente el registro.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                await Navigation.PopAsync();
            }
        }

        private void FinalizaVR(object sender, EventArgs e)
        {
            var pop = new MessageBox("Solicitud", "¿Esta seguro de finalizar la solicitud?", 1);

            pop.OnDialogClosed += async (s, arg) =>
            {
                if (arg)
                {
                    Todo.IsVisible = false;
                    Act.IsRunning = true;
                    Act.IsVisible = true;

                    if (!string.IsNullOrEmpty(TMensajeRes.Text))
                    {
                        Mensajes.MenMovMensajeRes = RSA.Encryption(TMensajeRes.Text);
                    }

                    Mensajes.MenMovFechaFinal = DateTime.Now;   
                    Mensajes.MenMovEstatus = 2;

                    if (await Constants.ActualizaMensajes(usuario.Token, Mensajes, 0) == 1)
                    {
                        var p1 = new MessageBox("Respuesta", "Mensaje enviado.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var p1 = new MessageBox("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    }

                    Todo.IsVisible = true;
                    Act.IsRunning = false;
                    Act.IsVisible = false;
                }
            };
            App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
        }

        protected void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected void SolicitaMaterial(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ReqMaterial("No. " + Mensajes.MenMovID + ": " + RSA.Decryption(Mensajes.MenMovMensaje) + ". Del departamento " + Mensajes.MenMovDeDepto.Trim() + "."));
        }
        private void SeleccionFecha(object sender, DateChangedEventArgs e)
        {
            Fecha = e.NewDate;
        }

        async void TomarCamara(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                var p1 = new MessageBox(":(", "Camara no disponible.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            var stream1 = file.GetStream();
            fileByte = new byte[stream1.Length];
            stream1.Read(fileByte, 0, (int)stream1.Length);


            GridFoto.IsVisible = true;
            file.Dispose();
        }

        async void TomarGaleria(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                var p1 = new MessageBox(":(", "Tu celular no soporta esta esta funcionalidad", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                return;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            var stream1 = file.GetStream();
            fileByte = new byte[stream1.Length];
            stream1.Read(fileByte, 0, (int)stream1.Length);

            GridFoto.IsVisible = true;

            file.Dispose();
        }

        void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //RadioButton button = sender as RadioButton;
            //horario = int.Parse(button.Text);
            if (RB1.IsChecked) horario = 1;
            else if (RB2.IsChecked) horario = 2;
            else horario = 0;
        }

        protected void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value) Limpieza = 1;
            else Limpieza = 0;
        }

        protected async void Chat(object sender, EventArgs e)
        {
            BtnChat.IsEnabled = false;
            MainPage.User = "Administrador";

            byte[] UsId = await Constants.ObtenerIdUsuario(usuario.Token, (int)Mensajes.UnHabMovID, Mensajes.MenMovDeDepto);

            ChatM.ChatMen chatMen = new ChatM.ChatMen
            {
                Identifica = 0,
                ChatMovil = new ChatM.ChatMovil
                {
                    ChID = 0,
                    ChEstatus = 1,
                    ChFecha = DateTime.Now,
                    ChMotivo = motivo,
                    ChSoliId = Mensajes.MenMovID,
                    ChUsuaFin = UsId,
                    ChUsuaInic = RSA.Encryption(MainPage.User)
                }
            };

            if(usuario.Tipo.Trim() == "C")
                MainPage.User = usuario.Usuario;

            var pop = new ChatBox(chatMen, usuario.Token);
            await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);

            BtnChat.IsEnabled = true;
        }

        private void OrdenaVacaciones()
        {
            if(Mensajes.MenMovEstatus == 1)
            {
                if(usuario.Tipo.Trim() == "A")
                {
                    RespAdmin.IsVisible = true;
                    GEnviar.IsVisible = true;
                }
            }
            else
            {
                VFechaCompleto.Text = "Fecha de finalizado: " + Convert.ToDateTime(Mensajes.MenMovFechaFinal).ToString("dd/MM/yy H:mm");
                BtnChat.IsVisible = false;
            }
        }

        private void OrdenaRoof()
        {
            if (Mensajes.MenMovEstatus == 1)
            {
                if (usuario.Tipo.Trim() == "A")
                {
                    FechaHora.Padding = new Thickness(0, -40);
                    Leyenda.Text = "*No puede modificar horarios y fechas, solo avisar por mensaje si es necesario cambiar.*";
                    GCambiaF.IsVisible = false;
                    FechaHora.IsEnabled = false;
                    RespAdmin.IsVisible = true;
                    LeyendaFin.IsVisible = true;
                    GEnviar.IsVisible = true;
                }
            }
            else if(Mensajes.MenMovEstatus == 2)
            {
                VFechaCompleto.Text = "Fecha de finalizado: " + Convert.ToDateTime(Mensajes.MenMovFechaFinal).ToString("dd/MM/yy H:mm");
                BtnChat.IsVisible = false;
                FechaHora.Padding = new Thickness(0, -40);
                Leyenda.Text = "*Ya no se puede modificar.*";
                GCambiaF.IsVisible = false;
                FechaHora.IsEnabled = false;
            }
        }

        private void OrdenaMante()
        {
            switch (Mensajes.MenMovEstatus)
            {
                case 1:
                    if (usuario.Tipo.Trim() == "A")
                    {
                        Material.IsVisible = true;
                        SMante.IsVisible = true;
                        GMante.IsVisible = true;
                    }
                    break;

                case 2:
                    if(usuario.Tipo.Trim() == "A")
                    {
                        Material.IsVisible = true;
                        LResAdmin.Text = "Trabajo realizado: ";
                        SManteObser.IsVisible = true;
                        Fotos.IsVisible = true;
                        RespAdmin.IsVisible = true;
                        LBMante.Text = "Finalizar Solicitud";
                        GMante.IsVisible = true;
                    }

                    VResponsables.Text = Mensajes.MenMovResponsables.Length > 100 ? "Responsables: \n" + RSA.Decryption(Mensajes.MenMovResponsables) : "";
                    break;
                case 3:
                    VFechaCompleto.Text = "Fecha de finalizado: " + Convert.ToDateTime(Mensajes.MenMovFechaFinal).ToString("dd/MM/yy H:mm");
                    BtnChat.IsVisible = false;
                    TObservacion.Text = Mensajes.MenMovObservaciones.Length > 100 ? "Observaciones: \n" + RSA.Decryption(Mensajes.MenMovObservaciones) : "";
                    VResponsables.Text = Mensajes.MenMovResponsables.Length > 100 ?  "Responsables: \n" + RSA.Decryption(Mensajes.MenMovResponsables) : "";
                    Material.IsVisible = false;
                    break;
            }
        }

        private void OrdenaOtros()
        {
            switch (Mensajes.MenMovEstatus)
            {
                case 1:
                    if (usuario.Tipo.Trim() == "A")
                    {
                        Material.IsVisible = true;
                        SMante.IsVisible = true;
                        GMante.IsVisible = true;
                        FechaHora.Padding = new Thickness(0, -40);
                        Leyenda.Text = "*No puede modificar horarios y fechas, solo avisar por mensaje si es necesario cambiar.*";
                        GCambiaF.IsVisible = false;
                        FechaHora.IsEnabled = false;
                    }
                    break;
                case 2:
                    if (usuario.Tipo.Trim() == "A")
                    {
                        Material.IsVisible = true;
                        Leyenda.Text = "*No puede modificar horarios y fechas, solo avisar por mensaje si es necesario cambiar.*";
                        FechaHora.IsEnabled = false;
                        LResAdmin.Text = "Trabajo realizado: ";
                        SManteObser.IsVisible = true;
                        Fotos.IsVisible = true;
                        RespAdmin.IsVisible = true;
                        LBMante.Text = "Finalizar Solicitud";
                        GMante.IsVisible = true;
                    }

                    VResponsables.Text = Mensajes.MenMovResponsables.Length > 100 ? "Responsables: \n" + RSA.Decryption(Mensajes.MenMovResponsables) : "";
                    break;
                case 3:
                    Leyenda.Text = "*Ya no se puede modificar.*";
                    VFechaCompleto.Text = "Fecha de finalizado: " + Convert.ToDateTime(Mensajes.MenMovFechaFinal).ToString("dd/MM/yy H:mm");
                    BtnChat.IsVisible = false;
                    TObservacion.Text = Mensajes.MenMovObservaciones.Length > 100 ? "Observaciones: \n" + RSA.Decryption(Mensajes.MenMovObservaciones) : "";
                    VResponsables.Text = Mensajes.MenMovResponsables.Length > 100 ? "Responsables: \n" + RSA.Decryption(Mensajes.MenMovResponsables) : "";
                    GCambiaF.IsVisible = false;
                    FechaHora.IsEnabled = false;
                    Material.IsVisible = false;
                    LeyendaFoto1.Text = "Antes";
                    LeyendaFoto2.Text = "Después";
                    break;
            }
        }

        private void OrdenaMantenimientoAdmin()
        {
            switch (Mensajes.MenMovEstatus)
            {
                case 1:
                    BtnChat.IsVisible = false;
                    Material.IsVisible = true;
                    SMante.IsVisible = true;
                    LResAdmin.Text = "Trabajo realizado: ";
                    SManteObser.IsVisible = true;
                    Fotos.IsVisible = true;
                    RespAdmin.IsVisible = true;
                    LBMante.Text = "Finalizar Solicitud";
                    GMante.IsVisible = true;
                    break;

                case 2:
                    VFechaCompleto.Text = "Fecha de finalizado: " + Convert.ToDateTime(Mensajes.MenMovFechaFinal).ToString("dd/MM/yy H:mm");
                    BtnChat.IsVisible = false;
                    TObservacion.Text = Mensajes.MenMovObservaciones.Length > 100 ? "Observaciones: \n" + RSA.Decryption(Mensajes.MenMovObservaciones) : "";
                    VResponsables.Text = Mensajes.MenMovResponsables.Length > 100 ? "Responsables: \n" + RSA.Decryption(Mensajes.MenMovResponsables) : "";
                    Material.IsVisible = false;
                    break;
            }
        }

        private async void DespliegaCalendario()
        {
            //En el caso de esta solicitud se desplegaran las datos del calendario
            //Se requiere desde el servidor
            var calendario = await Constants.ObtenerCalendarioMenId(usuario.Token, Mensajes.MenMovID);

            //Se válida que traiga algo
            if (calendario == null)
            {
                var p1 = new MessageBox("Solicitud", "Ha ocurrido un error, intentelo más tarde.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
            }
            else
            {
                if(calendario.Count > 0)
                {
                    FechaHora.IsVisible = true;

                    //Se carga el registro del calendario
                    foreach (var item in calendario)
                    {
                        Calendar = item;
                    }
                    //Se despliegan los datos del calendario en los pickers
                    DatePicker.Date = Calendar.CalFecha;
                    TimePicker.Time = Calendar.CalFecha.TimeOfDay;

                    if (Mensajes.MenMovMotivo == "R")
                    {
                        PickerHora.IsVisible = false;
                        Roof.IsVisible = true;
                        //Se realiza la conversión al tener un valor de O y 1
                        SwitcHorario.IsToggled = Convert.ToBoolean(Calendar.CalLimpieza);

                        //Se selecciona en que RadioButton viene
                        if (Calendar.CalTurno == 1)
                            RB1.IsChecked = true;
                        else
                            RB2.IsChecked = true;
                    }
                }
            }
        }
    }
}


/*Antiguo código
 * 
if (usuario.Tipo == "A")
{
    else if (Mensajes.MenMovEstatus == 1)
    {
        if(Mensajes.MenMovMotivo == "O")
        {
            RespAdmin.IsVisible = true;
            Fotos.IsVisible = true;
            FechaHora.IsVisible = true;
            GEnviar.IsVisible = true;
        }
    }
}

Era de mensajes directos                
if (Mensajes.MenMovEstatus == 0 && Mensajes.MenMovMotivo == "J")
{
    if(Mensajes.MenMovDeDepto.Trim() != "Todos")
    {
        RespAdmin.IsVisible = true;
        GEnviarRes.IsVisible = true;
    }

    if (await Constants.ActualizaMen(Mensajes.MenMovID, usuario.Token) == 0)
    {
        var p1 = new MessageBox("Solicitud", "Ha ocurrido un error, intentelo más tarde.", 0);
        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
        //await DisplayAlert("Solicitud", "Ha ocurrido un error, intentelo más tarde.", "Aceptar");
    }
}


async void MandaRespuesta(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TMensajeRes.Text))
            {
                Todo.IsVisible = false;
                Act.IsRunning = true;
                Act.IsVisible = true;

                Mensajes.MenMovMensajeRes = RSA.Encryption(TMensajeRes.Text);

                if (await Constants.ActualizaMensajes(usuario.Token, Mensajes, 0) == 1)
                {
                    var p1 = new MessageBox("Respuesta", "Mensaje enviado.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Respuesta", "Mensaje enviado.", "Aceptar");
                    await Navigation.PopAsync();
                }
                else
                {
                    var p1 = new MessageBox("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    //await DisplayAlert("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", "Aceptar");
                }

                Todo.IsVisible = true;
                Act.IsRunning = false;
                Act.IsVisible = false;
            }
            else
            {
                var p1 = new MessageBox("Respuesta", "Favor de llenar el campo de arriba.", 0);
                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                //await DisplayAlert("Respuesta","Favor de llenar el campo de arriba.","Aceptar")
            }
        }





if (Mensajes.MenMovEstatus == 1)
            {
                if (!string.IsNullOrEmpty(TRespobsables.Text))
                {
                    mensajeT = new Mensajes
                    {
                        MenMovID = Mensajes.MenMovID,
                        MenMovDeDepto = "",
                        MenMovDeNombre = new byte[10],
                        MenMovEstatus = 0,
                        MenMovFecha = DateTime.Now,
                        MenMovFotoA = new byte[10],
                        MenMovFotoD = new byte[10],
                        MenMovMensaje = new byte[10],
                        MenMovMensajeRes = new byte[10],
                        MenMovMotivo = "",
                        UnHabMovID = 0,
                        MenMovFechaEntrega = DateTime.Now,
                        MenMovFechaFinal = DateTime.Now,
                        MenMovObservaciones = new byte[10],
                        MenMovResponsables = RSA.Encryption(TRespobsables.Text)
                    };

                    if (await Constants.ActualizaMensajes(usuario.Token, mensajeT, 1) == 1)
                    {
                        var p1 = new MessageBox("Respuesta", "Mensaje enviado.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var p1 = new MessageBox("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", 0);
                        await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                    }
                }
                else
                {
                    var p1 = new MessageBox("Error", "Favor de llenar el recuadro de responsables.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
            }
            else if (Mensajes.MenMovEstatus == 2)
            {
                if ((!string.IsNullOrEmpty(TMensajeRes.Text) && fileByte.Length > 100 && Mensajes.MenMovMotivo.Trim() == "M") || (!string.IsNullOrEmpty(TMensajeRes.Text) & Mensajes.MenMovMotivo.Trim() == "O"))
                {
                    byte[] observacion = !string.IsNullOrEmpty(TObservaciones.Text) ? RSA.Encryption(TObservaciones.Text) : new byte[10];

                    mensajeT = new Mensajes
                    {
                        MenMovID = Mensajes.MenMovID,
                        MenMovDeDepto = "",
                        MenMovDeNombre = new byte[10],
                        MenMovEstatus = 0,
                        MenMovFecha = DateTime.Now,
                        MenMovFotoA = new byte[10],
                        MenMovFotoD = fileByte,
                        MenMovMensaje = new byte[10],
                        MenMovMensajeRes = RSA.Encryption(TMensajeRes.Text),
                        MenMovMotivo = "",
                        UnHabMovID = 0,
                        MenMovFechaEntrega = DateTime.Now,
                        MenMovFechaFinal = DateTime.Now,
                        MenMovObservaciones = observacion,
                        MenMovResponsables = new byte[10]
                    };

                    var pop = new MessageBox("Solicitud", "¿Esta seguro de finalizar la solicitud?", 1);

                    pop.OnDialogClosed += async (s, arg) =>
                    {
                        if (arg)
                        {

                            if (await Constants.ActualizaMensajes(usuario.Token, mensajeT, 2) == 1)
                            {
                                var p1 = new MessageBox("Respuesta", "Mensaje enviado.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                                await Navigation.PopAsync();
                            }
                            else
                            {
                                var p1 = new MessageBox("Respuesta", "Su mensaje no ha podido ser enviado, intentelo más tarde.", 0);
                                await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                            }
                        }
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                }
                else
                {
                    var p1 = new MessageBox("Error", "Favor de llenar lo que se le pide arriba.", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
            }
 */