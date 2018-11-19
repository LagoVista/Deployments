using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models.Settings
{
    public class RPCSettings
    {
        public ConnectionSettings Receiver { get; set; }
        public ConnectionSettings Transmitter { get; set; }
    }
}
