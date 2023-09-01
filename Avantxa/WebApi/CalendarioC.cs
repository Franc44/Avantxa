using System;
namespace Avantxa.WebApi
{
    public class CalendarioC
    {
        public short CalID { get; set; }
        public byte[] CalPara { get; set; }
        public byte[] CalTitulo { get; set; }
        public byte[] CalMensaje { get; set; }
        public short CalEstatus { get; set; }
        public DateTime CalFecha { get; set; }
        public short? CalMenMovId { get; set; }
        public short? CalLimpieza { get; set; }
        public short? CalTurno { get; set; }
    }
}
