using System;
using System.Collections.Generic;

namespace Avantxa.WebApi
{
    public class ChatM
    {
        public class ChatMen
        {
            public int? Identifica { get; set; } = 0;
            public ChatMovil ChatMovil { get; set; } = new ChatMovil();
            public List<ChatMensajesMovil> ChatMensajesMovil { get; set; } = new List<ChatMensajesMovil>();
        }

        public struct ChatMovil
        { 
            public int ChID { get; set; }
            public byte[] ChUsuaInic { get; set; }
            public byte[] ChUsuaFin { get; set; }
            public short ChSoliId { get; set; }
            public string ChMotivo { get; set; }
            public short ChEstatus { get; set; }
            public DateTime? ChFecha { get; set; }
        }

        public struct ChatMensajesMovil
        {
            public int ChMID { get; set; }
            public int ChID1 { get; set; }
            public byte[] ChMUsua { get; set; }
            public byte[] ChMensaje { get; set; }
            public DateTime ChFecha { get; set; }

            public static implicit operator List<object>(ChatMensajesMovil v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
