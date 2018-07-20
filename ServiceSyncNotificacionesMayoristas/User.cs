using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public last_login last_login { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string cliente_id { get; set; }
        public int solicitar { get; set; }
        public string deleted_at { get; set; }
        public string cliente { get; set; }

        public User()
        {
            this.last_login = new last_login();
        }
    }

    class last_login
    {
        public string date { get; set; }
        public string timezone_type { get; set; }
        public string timezone { get; set; }
    }
}
