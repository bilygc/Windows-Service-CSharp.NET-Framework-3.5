using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ServiceSyncNotificacionesMayoristas
{
    class Cnotificaciones
    {
        /*consumimos el servicio de login, de metodo tipo post y regresamos los datos del tipo streamreader*/
        public StreamReader Login()
        {

            StreamReader streamReader;
            WebRequest reqObject = WebRequest.Create("http://apps._.com/v1/api/login");
            reqObject.Method = "POST";
            reqObject.ContentType = "application/json";

            try
            {
                using (var streamWriter = new StreamWriter(reqObject.GetRequestStream()))
                {
                    /*llenamos el objeto de la solicitud*/
                    ReqToken requestToken = new ReqToken();
                    requestToken.email = "@notificaciones.com";
                    requestToken.password = "secret";

                    /*serialisamos el objeto y lo enviamos*/
                    String json = JsonConvert.SerializeObject(requestToken, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (WebException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }

            
            /*obtenemos la respuesta y la retornamos*/
            HttpWebResponse response = reqObject.GetResponse() as HttpWebResponse;

            streamReader = new StreamReader(response.GetResponseStream());
            return streamReader;                        
           
            
        }

        /*obetenemos el token del metodo login, validamos si ya expiro y lo retornamos*/
        public string obtenerToken()
        {

            string sToken = string.Empty;
            string sTokenBD = string.Empty;
            string sReturnToken = string.Empty;
            string[] subsrtToken;
            byte[] data;
            string fechaCaducidad;
            Token token;
            DateTime dtFechaCaducidad;
            DateTime FechaActual;
            int cmpDates;
            try
            {
                using (SqlConnection conn = this.obtenerConexion())
                {

                    //nos traemos el token almacenado en bd
                    SqlCommand query = new SqlCommand("select token from sAppMayoristas.SyncNotificacionesMayoristas where id = 1", conn);

                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        reader.Read();
                        sTokenBD = reader["token"].ToString();

                        subsrtToken = sTokenBD.Split('.');
                        data = Convert.FromBase64String(subsrtToken[1] += "=");//Decodificamos la seccion de datos que nos interesa, se agrega '=' al final ya que viene truncada la cadena
                        fechaCaducidad = Encoding.UTF8.GetString(data);

                        token = JsonConvert.DeserializeObject<Token>(fechaCaducidad);//la seccion que tomamos la convertimos a tipo Token
                        dtFechaCaducidad = this.TimestampAFecha(token.exp);//Convertimos la fecha tipo unix a tipo datetime
                        FechaActual = DateTime.Now;//Obtenemos la fecha y hora actual
                        cmpDates = DateTime.Compare(FechaActual, dtFechaCaducidad);//Comparamos la fecha actual y la fecha del token para saber si ya expiro
                        reader.Close();

                        /*cmpDates = -1 no ha expirado el token, 0 fecha actual y de token es la misma, 1 token ya expiro*/
                        if (cmpDates >= 0)
                        {
                            //insertar codigo aqui para traer nuevo token
                            using (var loginData = this.Login())
                            {


                                Login login = JsonConvert.DeserializeObject<Login>(loginData.ReadToEnd());//Convertimos a tipo Login la respuesta del ws
                                sToken = login.token;

                                SqlCommand updateQuery = new SqlCommand("Update sAppMayoristas.SyncNotificacionesMayoristas  set token = @TOKEN where id = 1", conn);
                                updateQuery.Parameters.Add(new SqlParameter("TOKEN", sToken));

                                updateQuery.ExecuteNonQuery();
                                sReturnToken = sToken;

                            }
                        }
                        else
                        {
                            sReturnToken = sTokenBD;
                        }


                    }
                }
            }
            catch (SqlException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }

            return sReturnToken;

        }

        public bool EnviarNotificacion(string Titulo, string sMensaje, string sCliente )
        {

            String token = this.obtenerToken();


            WebRequest reqObject = WebRequest.Create("http://apps._.com/v1/api/firebase/notificaciones/enviar");
            reqObject.Method = "POST";
            reqObject.ContentType = "application/json";
            reqObject.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                using (var streamWriter = new StreamWriter(reqObject.GetRequestStream()))
                {

                    CrequestNotification objNotificacion = new CrequestNotification(1);
                    objNotificacion.titulo = Titulo;
                    objNotificacion.descripcion = sMensaje;
                    objNotificacion.usuarios[0].id = sCliente;

                    String json = JsonConvert.SerializeObject(objNotificacion, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (WebException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
                return false;
            }

            try
            {
                HttpWebResponse response = reqObject.GetResponse() as HttpWebResponse;
                var streamReader = new StreamReader(response.GetResponseStream());
                string lbl_response_status = "Status: " + response.StatusCode;
                string lbl_status_description = " Descripcion: " + response.StatusDescription;
                string lbl_response_data = "Data: " + streamReader.ReadToEnd();

                int iStatusCode = (int)response.StatusCode;

                if (iStatusCode < 200 && iStatusCode > 299)
                    return false;
            }
            catch
            {
                return false;
            }


            return true;
        }

        public string EnviarNotificacion(CrequestNotification objCliente)
        {
            String token = this.obtenerToken();


            WebRequest reqObject = WebRequest.Create("http://apps._.com/v1/api/firebase/notificaciones/enviar");
            reqObject.Method = "POST";
            reqObject.ContentType = "application/json";
            reqObject.Headers.Add("Authorization", "Bearer " + token);

            try
            {

                using (var streamWriter = new StreamWriter(reqObject.GetRequestStream()))
                {

                    String json = JsonConvert.SerializeObject(objCliente, Formatting.Indented);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (WebException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }

            
            HttpWebResponse response = reqObject.GetResponse() as HttpWebResponse;
            var streamReader = new StreamReader(response.GetResponseStream());
            string lbl_response_status = "Status: " + response.StatusCode;
            string lbl_status_description = " Descripcion: " + response.StatusDescription;
            string lbl_response_data = "Data: " + streamReader.ReadToEnd();

            return lbl_response_data;
        }

        public int totalDifusiones()
        {
            int iTotal = 0;

            using (SqlConnection conn = this.obtenerConexion())
            {

                SqlCommand query = new SqlCommand("exec [sAppMayoristas].[procNotificaciones_Abonos]", conn);


                using (SqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        iTotal++;
                    }
                }
            }
            return iTotal;

        }

        /*public CrequestNotification obTenerDifusion()
        {

        }*/

        protected SqlConnection obtenerConexion()
        {

            SqlConnection conn = new SqlConnection();

            try
            {

                conn.ConnectionString = "";
                conn.Open();
            }
            catch (SqlException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }
            return conn;
        }

        protected DateTime TimestampAFecha(uint unixdate)
        {

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixdate).ToLocalTime();

            return dtDateTime;
        }

        protected void escribirLog(string msg, EventLogEntryType tipo)
        {

            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
                System.Diagnostics.EventLog.CreateEventSource("MySource", "MyNewLog");
            

            EventLog.WriteEntry("MySource", msg, tipo);
           
        }

        public static void Log(string FileName, string Texto)
        {
            using (StreamWriter w = File.AppendText(@"c:\log\" + FileName))
            {
                w.WriteLine("\r\n-------------------------------------");
                w.WriteLine("{0} {1} -> {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), Texto.Trim());
                w.WriteLine("-------------------------------------");
                w.Flush();

            }
        }

        public void ActualizarUltimaSincronizacion(string dFecha, int iOperacion)
        {
            DateTime dFechaActualizacion = new DateTime();
            dFechaActualizacion = Convert.ToDateTime(dFecha);

            try
            {
                using (SqlConnection conn = this.obtenerConexion())
                {
                    SqlCommand query;
                    //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", "fecha: " + dFecha);

                    //Fecha de abono
                    if (iOperacion == 1)
                        query = new SqlCommand("update sAppMayoristas.SyncNotificacionesMayoristas set dUltimaFecha_Abonos = CONVERT(DATETIME,@FECHA) where id = 1", conn);
                    else//Fecha de compra
                        query = new SqlCommand("update sAppMayoristas.SyncNotificacionesMayoristas set dUltimaFecha_Compras = CONVERT(DATETIME,@FECHA) where id = 1", conn);

                    query.Parameters.Add(new SqlParameter("FECHA", dFechaActualizacion));

                    query.ExecuteNonQuery();

                }
            }
            catch(SqlException ex)
            {
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", "Error en actualizacion de fecha: "+ex.Message);
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }


        }
    }
}
