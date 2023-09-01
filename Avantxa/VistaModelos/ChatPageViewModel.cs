using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Avantxa.Modelos;
using Xamarin.Forms;
using Avantxa.WebApi;
using WebApiAvantxa.Security;
using Rg.Plugins.Popup.Extensions;
using Avantxa.Vistas;

namespace Avantxa.VistaModelos
{
    public class ChatPageViewModel : INotifyPropertyChanged
    {
        public bool ShowScrollTap { get; set; } = false;
        public bool LastMessageVisible { get; set; } = true;
        public int PendingMessageCount { get; set; } = 0;
        public bool PendingMessageCountVisible { get { return PendingMessageCount > 0; } }

        public Queue<MensajesChat> DelayedMessages { get; set; } = new Queue<MensajesChat>();
        public ObservableCollection<MensajesChat> Messages { get; set; } = new ObservableCollection<MensajesChat>();
        public string TextToSend { get; set; }
        public ICommand OnSendCommand { get; set; } 
        public ICommand MessageAppearingCommand { get; set; }
        public ICommand MessageDisappearingCommand { get; set; }

        public string JWT { get; set; }
        public int ChId { get; set; } = 0;

        public ChatPageViewModel(ChatM.ChatMen chatMen, int opcion, string Token)
        {
            JWT = Token;

            try
            {
                if (opcion == 1)
                {
                    foreach (var item in chatMen.ChatMensajesMovil)
                    {
                        Messages.Insert(0, new MensajesChat() { Text = RSA.Decryption(item.ChMensaje), User = RSA.Decryption(item.ChMUsua) });
                    }
                }
            }
            catch(Exception es)
            {
                Console.WriteLine(es);
            }

            MessageAppearingCommand = new Command<MensajesChat>(OnMessageAppearing);
            MessageDisappearingCommand = new Command<MensajesChat>(OnMessageDisappearing);

            OnSendCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(TextToSend))
                {
                    Messages.Insert(0, new MensajesChat() { Text = TextToSend, User = MainPage.User });

                     List<ChatM.ChatMensajesMovil> mensajes = new List<ChatM.ChatMensajesMovil>
                     {
                         new ChatM.ChatMensajesMovil
                         {
                             ChMID = 0,
                             ChFecha = DateTime.Now,
                             ChID1 = chatMen.ChatMovil.ChID,
                             ChMensaje = RSA.Encryption(TextToSend),
                             ChMUsua = RSA.Encryption(MainPage.User)
                         }
                     };

                     TextToSend = string.Empty;

                    if (chatMen.ChatMovil.ChID > 0)
                        ChId = chatMen.ChatMovil.ChID;

                     ChatM.ChatMen chat = new ChatM.ChatMen
                     {
                         Identifica = chatMen.Identifica,
                         ChatMovil = new ChatM.ChatMovil
                         {
                             ChID = ChId,
                             ChEstatus = chatMen.ChatMovil.ChEstatus,
                             ChFecha = chatMen.ChatMovil.ChFecha,
                             ChMotivo = chatMen.ChatMovil.ChMotivo,
                             ChSoliId = chatMen.ChatMovil.ChSoliId,
                             ChUsuaFin = chatMen.ChatMovil.ChUsuaFin,
                             ChUsuaInic = chatMen.ChatMovil.ChUsuaInic
                         },
                         ChatMensajesMovil = mensajes
                     };

                     ChId = await Constants.MandarChatMensaje(JWT, chat);
                     if (ChId < 1)
                     {
                         var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado, intentelo más tarde.", 0);
                         await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                     }
                }
            });

            /*Code to simulate reveing a new message procces
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (LastMessageVisible)
                {
                    Messages.Insert(0, new MensajesChat() { Text = "New MensajesChat test", User = "Mario" });
                }
                else
                {
                    DelayedMessages.Enqueue(new MensajesChat() { Text = "New message test", User = "Mario" });
                    PendingMessageCount++;
                }
                return true;
            });*/
        }


        void OnMessageAppearing(MensajesChat message)
        {
            var idx = Messages.IndexOf(message);
            if (idx <= 6)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    while (DelayedMessages.Count > 0)
                    {
                        Messages.Insert(0, DelayedMessages.Dequeue());
                    }
                    ShowScrollTap = false;
                    LastMessageVisible = true;
                    PendingMessageCount = 0;
                });
            }
        }

        void OnMessageDisappearing(MensajesChat message)
        {
            var idx = Messages.IndexOf(message);
            if (idx >= 6)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowScrollTap = true;
                    LastMessageVisible = false;
                });

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
