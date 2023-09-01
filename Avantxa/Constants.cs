using Avantxa.Base;
using Avantxa.Modelos;
using Avantxa.Tools;
using Avantxa.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiAvantxa.Security;

namespace Avantxa
{
    public class Constants
    {
        static TablaUsuario sesion = new TablaUsuario();

        static HttpClient cliente = new HttpClient();

        public static int result;

        //public const string uri = "http://192.168.1.60/WebApiAvantxa/api/";
        public const string uri = "https://www.maxal-cloud.com.mx/api/";
        //public const string uri = "http://192.168.1.59/AvantxaWA/api/";

        public const string DatabaseFilename = "Avantxa.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }


        public static async Task<Usuario.Usuar> Login(User user)
        {
            try
            {
                var Autenticar = new User
                {
                    Usua = user.Usua,
                    Contras = user.Contras,
                    Token = user.Token
                };
                var json = JsonConvert.SerializeObject(Autenticar);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage respond = null;

                respond = await cliente.PostAsync(uri + "Inicio/authenticate", content);

                if (respond.IsSuccessStatusCode)
                {
                    var resultado = await respond.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<WebApi.Usuario.Usuar>(resultado);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        internal static Task<List<Usuario.Usuar>> ObtenerUsuariosUH(string token, int? uHid)
        {
            throw new NotImplementedException();
        }

        public static async Task<string> RefreshToken()
        {
            Byte[] array = new Byte[64]; //No importa el valor de este array, pero no tampoco se inicializa como null

            sesion = await App.Database.GetItemsAsync();
            User Autenticar = new User();
            try
            {
                Autenticar.Usua = RSA.Encryption(sesion.Usuario);
                Autenticar.Contras = array;
                Autenticar.Token = sesion.Token;

                var json = JsonConvert.SerializeObject(Autenticar);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage respond = null;

                respond = await cliente.PostAsync(uri + "Inicio/authenticate", content);

                if (respond.IsSuccessStatusCode)
                {
                    var resultado = await respond.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<WebApi.Usuario.Usuar>(resultado);

                    return data.Token;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<ListaMensajes>> Mensajes(string depto, int UHId, string token, int metodo) //Método 0: Obtener mensajes por departamento; Método 1: Obtener mensajes por Unidad
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            /* Se utiliza hasta que se implemente el almacenamiento interno en el telefono
            await App.Database.DeleteItemAsyncMen();
            await App.Database.DeleteItemAsyncVer();
            */

            try
            {
                HttpResponseMessage request = null;
                if(metodo == 0)
                    request = await cliente.GetAsync(uri + "Mensajes/Dep?id=" + depto);
                else
                    request = await cliente.GetAsync(uri + "Mensajes/" + UHId);

                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ListaMensajes>>(resultado);
                }
                else if(request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await Mensajes(depto, UHId, refresh, metodo);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<Mensajes> MensajesSolo(int Id, string token)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Mensajes/ID?id=" + Id);

                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Mensajes>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await MensajesSolo(Id, refresh);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<int> ActualizaMen(int Id, string token)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Mensajes/Est?id=" + Id);

                if (request.IsSuccessStatusCode)
                {
                    return 1;
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ActualizaMen(Id, refresh);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return 0;
            }
        }

        public static async Task<int> ActualizaMensajes(string token, Mensajes men, int metodo)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var json = JsonConvert.SerializeObject(men);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage respond = null;

                if(metodo == 0)
                    respond = await cliente.PutAsync(uri + "Mensajes/" + men.MenMovID, content);
                else if(metodo == 1)
                    respond = await cliente.PutAsync(uri + "Mensajes/Visto?id=" + men.MenMovID, content);
                else
                    respond = await cliente.PutAsync(uri + "Mensajes/Finalizar?id=" + men.MenMovID, content);

                if (respond.IsSuccessStatusCode)
                {
                    return 1;
                }
                else if (respond.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ActualizaMensajes(refresh, men, metodo);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return 0;
            }
        }

        //Recuerda quitar este después
        public static async Task<List<Usuario.Usuar>> ObtenerUsuariosUH(string Token, int UHId)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Base/UH?id=" + UHId);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Usuario.Usuar>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerUsuariosUH(refresh, UHId);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<Usuario.Usuar>> ObtenerDepartmanetos(string Token, int UHId)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Base/Deptos?id=" + UHId);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Usuario.Usuar>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerDepartmanetos(refresh, UHId);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<TVisita>> ObtenerVisitas(string Token, int UHId)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Visitas/All?id=" + UHId);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TVisita>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerVisitas(refresh, UHId);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<Visita>> ObtenerVisitaId(string Token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Visitas/ID?id=" + id);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Visita>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerVisitaId(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<CalendarioC>> ObtenerCalendarioId(string Token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Calendario/" + id);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CalendarioC>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerCalendarioId(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<CalendarioC>> ObtenerCalendarioUsua()
        {
            sesion = await App.Database.GetItemsAsync();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sesion.Token);

            try
            {
                var request = await cliente.GetAsync(uri + "Calendario/Usu?usuario=" + sesion.Usuario);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CalendarioC>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerCalendarioUsua();
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<CalendarioC>> ObtenerCalendarioMenId(string token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Calendario/Men?id=" + id);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<CalendarioC>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerCalendarioMenId(refresh,id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<int> RevisaFechaRoof(string token, DateTime fecha, int id, int turno)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string fechaString = fecha.Month + "-" + fecha.Day + "-" + fecha.Year;

            try
            {
                var request = await cliente.GetAsync(uri + "Calendario/Rev?fechaString=" + fechaString + "&id=" + id + "&turno=" + turno);
                if (request.IsSuccessStatusCode)
                {
                    return 1;
                }
                else if(request.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return 2;
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await RevisaFechaRoof(refresh, fecha, id,turno);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return 0;
            }
        }

        public static async Task<int> ActualizaCalendario(CalendarioC cal, int id, string token)
        {
            try
            {
                cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(cal);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage respond = null;

                respond = await cliente.PutAsync(uri + "Calendario/" + id, content);

                if (respond.IsSuccessStatusCode)
                {
                    return 1;
                }
                else if (respond.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ActualizaCalendario(cal, id, refresh);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return 0;
            }
        }

        public static async Task<List<RecibosW2>> ObtenerRecibosUH(string token, int UH)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Recibos/UH?uh=" + UH);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RecibosW2>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerRecibosUH(refresh, UH);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<RecibosW>> ObtenerReciboId(string token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Recibos/ID?ID=" + id);
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RecibosW>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerReciboId(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<RecibosW2>> ObtenerRecibosUsua(string token, string user)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await cliente.GetAsync(uri + "Recibos/Usu?usuario=" + user);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RecibosW2>>(resultado);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerRecibosUsua(refresh, user);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<UH>> ObtenerUHs(string token)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "UH/");
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UH>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerUHs(refresh);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<UsuarioActualiza>> ObtenerUsuariosSuper(string token)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var request = await cliente.GetAsync(uri + "Base/");
                if (request.IsSuccessStatusCode)
                {
                    var resultado = await request.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UsuarioActualiza>>(resultado);
                }
                else if (request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerUsuariosSuper(refresh);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<ChatM.ChatMovil>> ObtenerChatsUsua(string token, string user)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Chat/Usua?usuario=" + user);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ChatM.ChatMovil>>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerChatsUsua(refresh, user);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }
        public static async Task<ChatM.ChatMen> ObtenerChatsSolicitud(string token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Chat/SolID?SolId=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ChatM.ChatMen>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerChatsSolicitud(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<ChatM.ChatMensajesMovil>> ObtenerChatId(string token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Chat/ID?id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ChatM.ChatMensajesMovil>>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerChatId(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<int> MandarChatMensaje(string token, ChatM.ChatMen mensajes)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var json = JsonConvert.SerializeObject(mensajes);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await cliente.PostAsync(uri + "Chat", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return Convert.ToInt16(await responseMessage.Content.ReadAsStringAsync());
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await MandarChatMensaje(refresh, mensajes);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return 0;
            }
        }

        //Esto lo hubieras evitado si lo hubieras hecho bien 
        public static async Task<byte[]> ObtenerIdUsuario(string token, int UhId, string Depto)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Base/UsuId?Uhid=" + UhId + "&Depto=" + Depto);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<byte[]>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerIdUsuario(refresh, UhId, Depto);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<AnunciosW> ObtenerAnuncioId(string token, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Anuncios/AnuncioGet?Id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<AnunciosW>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerAnuncioId(refresh, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<List<TAnuncios>> ObtenerAnuncios(string token, string usuario, int id)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage responseMessage = await cliente.GetAsync(uri + "Anuncios/AnunciosGet?usuario=" + usuario + "&id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultado = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TAnuncios>>(resultado);
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await ObtenerAnuncios(refresh, usuario, id);
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return null;
            }
        }

        public static async Task<bool> MandarAnuncio(string token, AnunciosW anunciosW)
        {
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var json = JsonConvert.SerializeObject(anunciosW);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await cliente.PostAsync(uri + "Anuncios/Agrega", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refresh = await RefreshToken();
                    await MandarAnuncio(refresh, anunciosW);
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                return false;
            }
        }

    }
}
