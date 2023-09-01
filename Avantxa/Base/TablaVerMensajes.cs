using SQLite;
using System;

namespace Avantxa.Base
{
    public class TablaVerMensajes
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(100)]
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}
