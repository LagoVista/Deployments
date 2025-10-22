// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cd93ef94228e40043a2869d348100b72309ee10ee12888eccb76ff4ac8485ec1
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class DeployedInstanceVersion : TableStorageEntity
    {
        public string Name { get; set; }
        public string InstanceId { get; set; }
        public double Version { get; set; }
        public string TimeStamp { get; set; }
        public string Status { get; set; }
        public string ReleaseNotes { get; set; }
        public string Uri { get; set; }
    }
}
