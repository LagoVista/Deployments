using LagoVista.IoT.Deployment.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class InstanceStatusUpdate
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentInstanceStates NewStatus { get; set; }
        public bool Deployed { get; set; }
        public string Version { get; set; }
        public string Details {get; set;}
    }
}
