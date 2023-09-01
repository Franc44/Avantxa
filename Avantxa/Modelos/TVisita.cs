using System;

namespace Avantxa.Modelos
{
    public class TVisita
    {
        public short VisID { get; set; }
        public byte[] UsuaMovID { get; set; }
        public System.DateTime VisHoraEnt { get; set; }
        public DateTime? VisHoraSal { get; set; }
        public string VisMotivo { get; set; }
        public short UnHabMovID { get; set; }
    }
}
