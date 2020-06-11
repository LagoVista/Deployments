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
