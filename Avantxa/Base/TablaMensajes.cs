using SQLite;

namespace Avantxa
{
    public class TablaMensajes
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(100)]
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public string Mensaje { get; set; }
    }
}