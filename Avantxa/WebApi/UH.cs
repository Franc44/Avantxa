using System;
namespace Avantxa.WebApi
{
    public class UH
    {
        public short UnHabMovID { get; set; }
        public string UnHabMovNombre { get; set; }
        public byte[] UnHabMovCP { get; set; }
        public string UnHabMovEstado { get; set; }
        public byte[] UnHabMovMunicipio { get; set; }
        public byte[] UnHabMovColonia { get; set; }
        public byte[] UnHabMovCalle { get; set; }
        public byte[] UnHabMovNumExt { get; set; }
        public byte[] UnHabMovNumInt { get; set; }
        public DateTime? UnHabMovFecha { get; set; }
        public short? UnHabMovEstatus { get; set; }
    }
}
