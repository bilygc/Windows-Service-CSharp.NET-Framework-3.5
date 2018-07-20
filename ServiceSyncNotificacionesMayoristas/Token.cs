using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    class Token
    {
        public int sub { get; set; }
        public string iss { get; set; }
        public uint iat { get; set; }
        public uint exp { get; set; }
        public uint nbf { get; set; }
        public string jti { get; set; }
    }
}
