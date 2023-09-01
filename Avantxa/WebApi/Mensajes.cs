using System;

namespace Avantxa.WebApi
{
    public class Mensajes
    {
        public short MenMovID { get; set; }
        public byte[] MenMovDeNombre { get; set; }
        public byte[] MenMovMensaje { get; set; }
        public DateTime? MenMovFecha { get; set; }
        public short? MenMovEstatus { get; set; }
        public short? UnHabMovID { get; set; }
        public string MenMovDeDepto { get; set; }
        public byte[] MenMovMensajeRes { get; set; }
        public byte[] MenMovFotoA { get; set; }
        public byte[] MenMovFotoD { get; set; }
        public string MenMovMotivo { get; set; }
        public DateTime? MenMovFechaEntrega { get; set; }
        public DateTime? MenMovFechaFinal { get; set; }
        public byte[] MenMovObservaciones { get; set; }
        public byte[] MenMovResponsables { get; set; }
    }
}
