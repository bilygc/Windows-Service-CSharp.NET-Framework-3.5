using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ServiceSyncNotificacionesMayoristas
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            
#if (DEBUG)
            ServiceSyncNotificacionesMayoristas.syncNotificaciones(null);

#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ServiceSyncNotificacionesMayoristas() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
