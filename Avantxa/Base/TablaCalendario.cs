using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avantxa.Base
{
    public class TablaCalendario
    {
        [PrimaryKey]
        [MaxLength(10)]
        public short CalID { get; set; }
        [MaxLength(20)]
        public string CalPara { get; set; }
        [MaxLength(20)]
        public string CalTitulo { get; set; }
        [MaxLength(50)]
        public string CalMensaje { get; set; }
        [MaxLength(2)]
        public short CalEstatus { get; set; }
        public DateTime CalFecha { get; set; }
        [MaxLength(10)]
        public short? CalMenMovId { get; set; }
    }
}
