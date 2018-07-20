using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    class CrequestNotification
    {
        private string _titulo;
        private string _descripcion;
        private string _tipo;
        private string _grupos;
        private string _plataforma;

        public string titulo
        {
            get { return _titulo; }
            set { _titulo = value; }
        }

        public string descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }


        public string tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }


        public string grupos
        {
            get { return _grupos; }
            set { _grupos = value; }
        }

        public Cusuario[] usuarios { get; set; }



        public string plataforma
        {
            get { return _plataforma; }
            set { _plataforma = value; }
        }

        public CrequestNotification(int iTotalUsuarios)
        {
            usuarios = new Cusuario[iTotalUsuarios];

            for (int i = 0; i < iTotalUsuarios; i++)
            {
                usuarios[i] = new Cusuario();
            }

            this._tipo = "api-usuarios";
            this._grupos = null;
            this._plataforma = "Todos";
        }
    }
}
