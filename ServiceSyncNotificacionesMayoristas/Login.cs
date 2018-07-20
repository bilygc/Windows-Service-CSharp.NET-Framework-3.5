using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    class Login
    {

        public User User { get; set; }
        public string token { get; set; }

        public Login()
        {
            User = new User();
        }
    }
}
