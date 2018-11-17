using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment
{
    public class DeploymentSettings
    {
        public string InstanceId { get; set; }
        public string HostId { get; set; }

        public string SharedAccessKey1 { get; set; }
        public string SharedAccessKey2 { get; set; }

        public string DockerCommandLine { get; set; }

        public EntityHeader<DeploymentConfigurations> DeploymentConfiguration { get; set; }
        public EntityHeader<DeploymentTypes> DeploymentType { get; set; }
        public EntityHeader<QueueTypes> QueueType { get; set; }
    }
}
