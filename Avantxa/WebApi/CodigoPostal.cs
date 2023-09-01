using System;
using System.Collections.Generic;

namespace Avantxa.WebApi
{
    public class CodigoPostal
    {
        public class Response
        {
            public string CP { get; set; }
            public List<string> Asentamiento { get; set; }
            public string Tipo_asentamiento { get; set; }
            public string Municipio { get; set; }
            public string Estado { get; set; }
            public string Ciudad { get; set; }
            public string Pais { get; set; }
        }

        public class Root
        {
            public bool Error { get; set; }
            public int Code_error { get; set; }
            public string Error_message { get; set; }
            public Response Response { get; set; }
        }


    }
}
