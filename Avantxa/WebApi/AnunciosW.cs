using System;
namespace Avantxa.WebApi
{
    public class AnunciosW
    {
        public int AnID { get; set; }
        public int AnUHId { get; set; }
        public byte[] AnUsuario { get; set; }
        public string AnAsunto { get; set; }
        public string AnMensaje { get; set; }
        public DateTime AnFecha { get; set; }
    }
}
