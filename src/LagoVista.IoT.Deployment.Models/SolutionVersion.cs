// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 35c43ec6f678dd48346855f447e9d747163e0fddeeaa971411cd9bcd8e1f7226
// IndexVersion: 2
// --- END CODE INDEX META ---
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
