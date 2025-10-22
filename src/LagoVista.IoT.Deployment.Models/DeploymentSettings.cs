// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 13ee921f35a45e4eab007029f3745ee474b02b81133ad6ad83d892e25cd8b80f
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment
{
    public class DeploymentSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
        
        public string InstanceId { get; set; }
        public string HostId { get; set; }
        public string OrgId { get; set; }

        public string SharedAccessKey1 { get; set; }
        public string SharedAccessKey2 { get; set; }

        public string DockerCommandLine { get; set; }


        public EntityHeader<NuvIoTEditions> NuvIoTEdition { get; set; }
        public EntityHeader<WorkingStorage> WorkingStorage { get; set; }
        public EntityHeader<DeploymentTypes> DeploymentType { get; set; }
    }
}
