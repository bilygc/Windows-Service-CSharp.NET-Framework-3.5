using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    class CplantillaNotificacion
    {
        private string _Titulo;
        private string _Mensaje;

        public string Titulo
        {
            get { return _Titulo; }
            set { _Titulo = value; }
        }

        public string Mensaje
        {
            get { return _Mensaje; }
            set { _Mensaje = value; }
        }

        public CplantillaNotificacion()
        {
            this.Titulo = string.Empty;
            this.Mensaje = string.Empty;
        }
    }
}
