// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bbf50e9b810a51e00160ae074add756510e66835c0b114cd4e777b6d66bc245c
// IndexVersion: 2
// --- END CODE INDEX META ---
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
