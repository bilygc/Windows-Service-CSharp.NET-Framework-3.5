using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Data.SqlClient;

namespace ServiceSyncNotificacionesMayoristas
{
    public partial class ServiceSyncNotificacionesMayoristas : ServiceBase
    {
        public ServiceSyncNotificacionesMayoristas()
        {
            InitializeComponent();

            if(!System.Diagnostics.EventLog.SourceExists("MySource") )
            {
                System.Diagnostics.EventLog.CreateEventSource("MySource", "MyNewLog");
            }

            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
            //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt","inicio de servicio");

        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Inicio de sincronizacion!", EventLogEntryType.Information);
            int iIntervalo = this.obtenerIntervalo();
            

            TimerCallback callBack = syncNotificaciones;
            Timer timer = new Timer(callBack);
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(iIntervalo));
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Fin de sincronizacion!", EventLogEntryType.Information);
            
            
        }

        public static void syncNotificaciones(object state)
        {
            
            
            bool bNotificacion = false;
            Cnotificaciones objNotificaciones = new Cnotificaciones();

            CplantillaNotificacion objPlantillaNotificacion = new CplantillaNotificacion();
            CplantillaNotificacion objPlantillaNotificacionAux = new CplantillaNotificacion();
            CnotificacionAbono objNotificacionAbono = new CnotificacionAbono();
            
            //Inicializamos el hilo que mandara la notificaciones de compras
            Thread Hilo = new Thread(new ThreadStart(NotificacionesCompras));
            Hilo.Start();

            objPlantillaNotificacion = objNotificacionAbono.plantilla();//Llenamos la plantilla de notificacion para el abono


            List<CnotificacionAbono> lstNotificacionAbono = new List<CnotificacionAbono>();
            

            lstNotificacionAbono = objNotificacionAbono.obtenerClientes();//Traemos los clientes a ser notificados de abonos
            

            string dFechaNotificacionHasta = string.Empty;//variable auxiliar para retener la fecha de ultima notificacion

            //mandamos notificacion una por una para cada cliente
            foreach (var abono in lstNotificacionAbono)
            {
                dFechaNotificacionHasta = abono.dFechaNotificacionHasta;
                objPlantillaNotificacionAux.Titulo = objPlantillaNotificacion.Titulo;
                objPlantillaNotificacionAux.Mensaje = objPlantillaNotificacion.Mensaje.Replace("{fechahora}", abono.dFechaAbono).Replace("{importe}", "$"+abono.nImporteAbono.ToString()).Replace("{ccliente}", abono.cCliente);
                objPlantillaNotificacionAux.Mensaje = objPlantillaNotificacionAux.Mensaje.Replace("{folioabono}", abono.sFolioAbono);
                bNotificacion = objNotificaciones.EnviarNotificacion(objPlantillaNotificacionAux.Titulo, objPlantillaNotificacionAux.Mensaje, abono.cCliente);
                if (!bNotificacion)
                    break;
                
            }

            //actualizamos la fecha de notifiacion de abono
            if (bNotificacion)
                objNotificaciones.ActualizarUltimaSincronizacion(dFechaNotificacionHasta, 1);

            //Esperamos a que el hilo de notificacion de compras termine y terminamos el programa
            Hilo.Join();

           
        }

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
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }
            return conn;
        }

        private int obtenerIntervalo()
        {
            int iIntervalo = 0;

            try
            {
                using (SqlConnection conn = this.obtenerConexion())
                {
                    SqlCommand query = new SqlCommand("select intervalo from sAppMayoristas.SyncNotificacionesMayoristas where id = 1", conn);

                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        reader.Read();
                        iIntervalo = reader["intervalo"] == System.DBNull.Value ? 0 : Convert.ToInt32(reader["intervalo"].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }
            

            return iIntervalo;
        }


        public static void NotificacionesCompras()
        {
            bool bNotificacion = false;
            Cnotificaciones objNotificaciones = new Cnotificaciones();

            CplantillaNotificacion objPlantillaNotificacion = new CplantillaNotificacion();
            CplantillaNotificacion objPlantillaNotificacionAux = new CplantillaNotificacion();
            CnotificacionCompra objNotificacionCompra = new CnotificacionCompra();

            objPlantillaNotificacion = objNotificacionCompra.plantilla();//Llenamos la plantilla de notificacion para el abono
            List<CnotificacionCompra> lstNotificacionCompra = new List<CnotificacionCompra>();

            lstNotificacionCompra = objNotificacionCompra.obtenerClientes();//Traemos los clientes a ser notificados de compras

            string dFechaNotificacionHasta = string.Empty;//variable auxiliar para retener la fecha de ultima notificacion
            objPlantillaNotificacion = objNotificacionCompra.plantilla();//Llenamos la plantilla de notificacion para el abono

            //mandamos notificacion una por una para cada cliente
            foreach (var compra in lstNotificacionCompra)
            {
                dFechaNotificacionHasta = compra.dFechaNotificacionComprasHasta;
                objPlantillaNotificacionAux.Titulo = objPlantillaNotificacion.Titulo;
                objPlantillaNotificacionAux.Mensaje = objPlantillaNotificacion.Mensaje.Replace("{fechahora}", compra.dFechaCompra).Replace("{importe}", compra.Importe.ToString()).Replace("{csucursal}", compra.cSucursal).Replace("{nombreSucursal}", compra.cNombreSucursal).Replace("{csubcliente}", compra.cSubCliente);
                objPlantillaNotificacionAux.Mensaje = objPlantillaNotificacionAux.Mensaje.Replace("{cnombreSubcliente}", compra.cNombreSubCliente).Replace("{cvale}", compra.Credivales);
                bNotificacion = objNotificaciones.EnviarNotificacion(objPlantillaNotificacionAux.Titulo, objPlantillaNotificacionAux.Mensaje, compra.cCliente);
                if (!bNotificacion)
                    break;
            }

            //actualizamos la fecha de notifiacion de compra
            if (bNotificacion)
                objNotificaciones.ActualizarUltimaSincronizacion(dFechaNotificacionHasta, 2);
        }


        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
