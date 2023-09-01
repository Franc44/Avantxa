using System;
namespace Avantxa.WebApi
{
    public struct ListaMensajes
    {
        public short MenMovID { get; set; }
        public byte[] MenMovDeNombre { get; set; }
        public DateTime? MenMovFecha { get; set; }
        public string MenMovDeDepto { get; set; }
        public string MenMovMotivo { get; set; }
        public short MenMovEstatus { get; set; }
    }
}
