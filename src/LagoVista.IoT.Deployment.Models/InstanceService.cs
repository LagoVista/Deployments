using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class InstanceService
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string HostId { get; set; }
        public string AllocatedTimeStamp { get; set; }
        public EntityHeader OwnerOrg { get; set; }
        public EntityHeader<HostTypes> HostType { get; set; }
    
        public string ServiceAccount { get; set; }
        public string ServiceAccountPassword { get; set; }
        public string ServiceAccountSecretId { get; set; }
    }
}
