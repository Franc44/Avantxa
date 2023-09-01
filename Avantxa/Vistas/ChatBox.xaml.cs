using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using Avantxa.VistaModelos;
using Avantxa.WebApi;

namespace Avantxa.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatBox : PopupPage
    {
        private static string jwt = "";
        private static int opcion = 0;

        public ChatBox(ChatM.ChatMen chatMen, string Token)
        {
            InitializeComponent();
            jwt = Token;

            Load(chatMen);
        }

        private async void Load(ChatM.ChatMen chat)
        {
            ChatM.ChatMen objetoChatMen = new ChatM.ChatMen
            {
                ChatMensajesMovil = new List<ChatM.ChatMensajesMovil>()
            };

            if (chat.Identifica == 0)
            {
                var chatMensajes = await Constants.ObtenerChatsSolicitud(jwt, chat.ChatMovil.ChSoliId);

                if (chatMensajes == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else if (chatMensajes.ChatMensajesMovil != null)
                {
                    if(chatMensajes.ChatMensajesMovil.Count > 0)
                    {
                        var mensajes = new List<ChatM.ChatMensajesMovil>();

                        foreach(var item in chatMensajes.ChatMensajesMovil)
                        {
                            mensajes.Add(item);
                        }

                        objetoChatMen = new ChatM.ChatMen
                        {
                            Identifica = 1,
                            ChatMovil = chatMensajes.ChatMovil,
                            ChatMensajesMovil = mensajes
                        };
                        opcion = 1;
                    }
                }
                else
                {
                    objetoChatMen = new ChatM.ChatMen
                    {
                        Identifica = 0,
                        ChatMovil = chat.ChatMovil
                    };
                }
            }
            else if (chat.Identifica == 1)
            {
                var mensajesMovils = await Constants.ObtenerChatId(jwt, chat.ChatMovil.ChID);

                if (mensajesMovils == null)
                {
                    var p1 = new MessageBox("Error", "Ha ocurrido un error inesperado", 0);
                    await App.Current.MainPage.Navigation.PushPopupAsync(p1, true);
                }
                else if (mensajesMovils.Count > 0)
                {
                    var mensajes = new List<ChatM.ChatMensajesMovil>();

                    foreach (var item in mensajesMovils)
                    {
                        mensajes.Add(item);
                    }

                    objetoChatMen = new ChatM.ChatMen
                    {
                        Identifica = 1,
                        ChatMovil = chat.ChatMovil,
                        ChatMensajesMovil = mensajes
                    };
                    opcion = 1;
                }
                else
                {
                    objetoChatMen = new ChatM.ChatMen
                    {
                        Identifica = 0,
                        ChatMovil = chat.ChatMovil
                    };
                }
            }
            else
            {
                objetoChatMen = new ChatM.ChatMen
                {
                    Identifica = 0,
                    ChatMovil = chat.ChatMovil
                };
            }

            if (!objetoChatMen.Equals(null))
                this.BindingContext = new ChatPageViewModel(objetoChatMen, opcion, jwt);
            else
                Load(chat);
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            var usuario = await App.Database.GetItemsAsync();

            if (usuario.Tipo.Trim() == "A")
                MainPage.User = usuario.Usuario;
        }

        protected void Button_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PopPopupAsync(true);
        }

        public void ScrollTap(object sender, EventArgs e)
        {
            lock (new object())
            {
                if (BindingContext != null)
                {
                    var vm = BindingContext as ChatPageViewModel;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        while (vm.DelayedMessages.Count > 0)
                        {
                            vm.Messages.Insert(0, vm.DelayedMessages.Dequeue());
                        }
                        vm.ShowScrollTap = false;
                        vm.LastMessageVisible = true;
                        vm.PendingMessageCount = 0;
                        ChatList?.ScrollToFirst();
                    });


                }

            }
        }

        public void OnListTapped(object sender, ItemTappedEventArgs e)
        {
            chatInput.UnFocusEntry();
        }
    }
}
