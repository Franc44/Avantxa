namespace Avantxa.WebApi
{
    public struct Visita
    {
        public short VisID { get; set; }
        public byte[] UsuaMovID { get; set; }
        public System.DateTime VisHoraEnt { get; set; }
        public System.DateTime VisHoraSal { get; set; }
        public string VisMotivo { get; set; }
        public short UnHabMovID { get; set; }
        public byte[] VisIdentificacion { get; set; }
    }
}
