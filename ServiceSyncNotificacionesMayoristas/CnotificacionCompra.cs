using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ServiceSyncNotificacionesMayoristas
{
    class CnotificacionCompra : Cnotificaciones
    {
        private string _cCliente;

        public string cCliente
        {
            get { return _cCliente; }
            set { _cCliente = value; }
        }

        private string _cSubCliente;

        public string cSubCliente
        {
            get { return _cSubCliente; }
            set { _cSubCliente = value; }
        }

        private string _cNombreSubCliente;

        public string cNombreSubCliente
        {
            get { return _cNombreSubCliente; }
            set { _cNombreSubCliente = value; }
        }
        private string _dFechaCompra;

        public string dFechaCompra
        {
            get { return _dFechaCompra; }
            set { _dFechaCompra = value; }
        }
        private double _Importe;

        public double Importe
        {
            get { return _Importe; }
            set { _Importe = value; }
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
        private string _Credivales;

        public string Credivales
        {
            get { return _Credivales; }
            set { _Credivales = value; }
        }

        private string _dFechaNotificacionComprasHasta;

        public string dFechaNotificacionComprasHasta
        {
            get { return _dFechaNotificacionComprasHasta; }
            set { _dFechaNotificacionComprasHasta = value; }
        }



        public List<CnotificacionCompra> obtenerClientes()
        {
            List<CnotificacionCompra> lClientes = new List<CnotificacionCompra>();

            try
            {
                using (SqlConnection conn = this.obtenerConexion())
                {

                    SqlCommand query = new SqlCommand("exec [sAppMayoristas].[procNotificaciones_Compras]", conn);


                    using (SqlDataReader reader = query.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var objCliente = new CnotificacionCompra();
                            objCliente.dFechaCompra = reader["dFechaCompra"] == System.DBNull.Value ? " " : reader["dFechaCompra"].ToString();
                            objCliente.cCliente = reader["cCliente"] == System.DBNull.Value ? " " : reader["cCliente"].ToString();
                            objCliente.cSubCliente = reader["cSubCliente"] == System.DBNull.Value ? " " : reader["cSubCliente"].ToString();
                            objCliente.cNombreSubCliente = reader["cNombreSubCliente"] == System.DBNull.Value ? " " : reader["cNombreSubCliente"].ToString();
                            objCliente.Importe = reader["Importe"] == System.DBNull.Value ? 0 : Convert.ToDouble(reader["Importe"]);
                            objCliente.cSucursal = reader["cSucursal"] == System.DBNull.Value ? " " : reader["cSucursal"].ToString();
                            objCliente.cNombreSucursal = reader["cNombreSucursal"] == System.DBNull.Value ? " " : reader["cNombreSucursal"].ToString();
                            objCliente.Credivales = reader["Credivales"] == System.DBNull.Value ? " " : reader["Credivales"].ToString();
                            objCliente.dFechaNotificacionComprasHasta = reader["dFechaNotificacionComprasHasta"] == System.DBNull.Value ? " " : reader["dFechaNotificacionComprasHasta"].ToString();
                            lClientes.Add(objCliente);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //this.escribirLog(ex.Message, EventLogEntryType.Error);
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
                    SqlCommand query = new SqlCommand("select cSubject,cNotificacion from [sAppMayoristas].[ctl_Notificaciones_plt] where idTipo= 2", conn);


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
                //this.escribirLog(ex.Message, EventLogEntryType.Error);
                //Cnotificaciones.Log("ServiceSyncNotificacionesMayoristas.txt", ex.Message);
            }

            return objPlantilla;
        }
    }
}
