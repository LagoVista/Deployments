using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class SolutionVersion : TableStorageEntity
    {
        public string Name { get; set; }
        public string SolutionId { get; set; }
        public double Version { get; set; }
        public string TimeStamp { get; set; }
        public string Status { get; set; }
        public string ReleaseNotes { get; set; }
        public string Uri { get; set; }
    }
}
