using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ServiceSyncNotificacionesMayoristas
{
    class CnotificacionAbono : Cnotificaciones
    {
        private string _cCliente;

        public string cCliente
        {
            get { return _cCliente; }
            set { _cCliente = value; }
        }

        private string _dFechaAbono;

        public string dFechaAbono
        {
            get { return _dFechaAbono; }
            set { _dFechaAbono = value; }
        }
        private double _nImporteAbono;

        public double nImporteAbono
        {
            get { return _nImporteAbono; }
            set { _nImporteAbono = value; }
        }
        private string _cSucursal;

        public string cSucursal
        {
            get { return _cSucursal; }
            set { _cSucursal = value; }
        }
        private string _cNombreSucursal;

        public string cNombreSucursal
        {
            get { return _cNombreSucursal; }
            set { _cNombreSucursal = value; }
        }

        private string _sFolioAbono;

        public string sFolioAbono
        {
            get { return _sFolioAbono; }
            set { _sFolioAbono = value; }
        }

        private string _dFechaNotificacionHasta;

        public string dFechaNotificacionHasta
        {
            get { return _dFechaNotificacionHasta; }
            set { _dFechaNotificacionHasta = value; }
        }



        public List<CnotificacionAbono> obtenerClientes()
        {
            List<CnotificacionAbono> lClientes = new List<CnotificacionAbono>();

            try
            {
                using (SqlConnection conn = this.obtenerConexion())
                {

                    SqlCommand query = new SqlCommand("exec [sAppMayoristas].[procNotificaciones_Abonos]", conn);


                    using (SqlDataReader reader = query.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var objCliente = new CnotificacionAbono();
                            objCliente.cCliente = reader["cCliente"] == System.DBNull.Value ? " " : reader["cCliente"].ToString();
                            objCliente.dFechaAbono = reader["dFechaAbono"] == System.DBNull.Value ? " " : reader["dFechaAbono"].ToString();
                            objCliente.nImporteAbono = reader["nImporteAbono"] == System.DBNull.Value ? 0 : Convert.ToDouble(reader["nImporteAbono"]);
                            objCliente.cSucursal = reader["cSucursal"] == System.DBNull.Value ? " " : reader["cSucursal"].ToString();
                            objCliente.cNombreSucursal = reader["cNombreSucursal"] == System.DBNull.Value ? " " : reader["cNombreSucursal"].ToString();
                            objCliente.sFolioAbono = reader["cFolioAbonoCredito"] == System.DBNull.Value ? " " : reader["cFolioAbonoCredito"].ToString();
                            objCliente.dFechaNotificacionHasta = reader["dFechaNotificacionHasta"] == System.DBNull.Value ? " " : reader["dFechaNotificacionHasta"].ToString();
                            lClientes.Add(objCliente);
                        }
                    }
                }
            }
            catch(SqlException ex){
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }
            
            return lClientes;
        }

        public CplantillaNotificacion plantilla()
        {
            CplantillaNotificacion objPlantilla = new CplantillaNotificacion();

            try
            {

                using (SqlConnection conn = this.obtenerConexion())
                {
                    SqlCommand query = new SqlCommand("select cSubject,cNotificacion from [sAppMayoristas].[ctl_Notificaciones_plt] where idTipo= 1", conn);


                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        reader.Read();
                        objPlantilla.Titulo = reader["cSubject"] == System.DBNull.Value ? " " : reader["cSubject"].ToString();
                        objPlantilla.Mensaje = reader["cNotificacion"] == System.DBNull.Value ? " " : reader["cNotificacion"].ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }

            return objPlantilla;
        }
    }
}
