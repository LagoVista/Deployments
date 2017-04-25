using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class DeploymentResources
    {
        public ConnectionSettings DeviceDataStorage { get; set; }
        public ConnectionSettings DeviceHistoryStorage { get; set; }


    }
}
