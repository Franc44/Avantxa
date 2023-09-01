using System;
using System.Collections.Generic;
using SQLite;

namespace Avantxa
{
    public class TablaUsuario
    {
        [PrimaryKey]
        [MaxLength(10)]
        public string Usuario { get; set; }

        [MaxLength(10)]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string Tipo { get; set; }

        [MaxLength(10)]
        public int? UHid { get; set; }

        [MaxLength(10)]
        public string Departamento { get; set; }

        [MaxLength(300)]
        public string Token { get; set; }

        public static implicit operator TablaUsuario(List<TablaUsuario> v)
        {
            throw new NotImplementedException();
        }
    }
}
