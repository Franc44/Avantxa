using System;
namespace Avantxa.WebApi
{
    public class RecibosW2
    {
        public int RecID { get; set; }
        public byte[] RecUsu { get; set; }
        public short RecUHId { get; set; }
        public string RecConcepto { get; set; }
        public short RecEstatus { get; set; }
        public DateTime RecFecha { get; set; }
    }
}

//Este objeto se utiliza para la forma corta de recibir los datos del recibo, sin el byte[] del archivo, así aligeramos el json y el tiempo de descarga del token