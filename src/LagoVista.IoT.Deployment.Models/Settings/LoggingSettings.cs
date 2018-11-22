using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models.Settings
{
    public class LoggingSettings
    {
        public ConnectionSettings ErrorLogger { get; set; }
        public ConnectionSettings EventLogger { get; set; }
    }
}
