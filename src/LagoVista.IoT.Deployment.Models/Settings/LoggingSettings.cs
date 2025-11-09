// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 34933af643858cff8a00cc4473145d5ca7a8a104671747dea2bb0eb1affa37fa
// IndexVersion: 2
// --- END CODE INDEX META ---
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
