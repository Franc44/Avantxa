using System;
namespace Avantxa.WebApi
{
    public class RecibosW
    {
        public int RecID { get; set; }
        public byte[] RecUsu { get; set; }
        public short RecUHId { get; set; }
        public string RecConcepto { get; set; }
        public byte[] RecArchivo { get; set; }
        public short RecEstatus { get; set; }
        public DateTime RecFecha { get; set; }
    }
}
