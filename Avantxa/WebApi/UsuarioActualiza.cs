using System;

namespace Avantxa.WebApi
{
    public class UsuarioActualiza
    {
        public byte[] UsuaMovID { get; set; }
        public short? UnHabMovID { get; set; }
        public byte[] UsuaMovNombre { get; set; }
        public string UsuaMovTipo { get; set; }
        public string MenMovEdif { get; set; }
        public string MenMovDepto { get; set; }
        public DateTime? UsuaMovFecha { get; set; }
        public short? UsuaMovEstatus { get; set; }
        public byte[] UsuaMovContra { get; set; }
        public byte[] UsuaMovEmail { get; set; }
        public byte[] UsuaMovAPat { get; set; }
        public byte[] UsuaMovAMat { get; set; }

    }
}
