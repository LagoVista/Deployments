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
