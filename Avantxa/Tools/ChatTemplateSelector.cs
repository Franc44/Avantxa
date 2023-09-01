using System;
using Xamarin.Forms;
using Avantxa.Modelos;
using Avantxa.Vistas.Cell;

namespace Avantxa.Tools
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as MensajesChat;
            if (messageVm == null)
                return null;


            return (messageVm.User == MainPage.User) ? incomingDataTemplate : outgoingDataTemplate;
        }
    }
}
