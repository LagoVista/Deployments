// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b0ba21684041da038bf36d5a4f3dc4ae55243cd78bc8f9a0cad2579127f0b2f
// IndexVersion: 0
// --- END CODE INDEX META ---
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
