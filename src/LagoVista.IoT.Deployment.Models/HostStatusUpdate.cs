// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0782c9d8a726eb823c2bd33a8b7bc5b7ad5fa564f07deabb5bf35bc07d5a293e
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class HostStatusUpdate
    {
        public String Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HostStatus NewStatus {get; set;}

        public String Version { get; set; }
        public String Details { get; set; }
    }
}
