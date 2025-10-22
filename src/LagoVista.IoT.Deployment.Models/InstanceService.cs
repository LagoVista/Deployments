// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9ea86b7cb0f7dadb34c15ca27396080733de6742791fa7025639f8eb1d640c84
// IndexVersion: 0
// --- END CODE INDEX META ---
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
