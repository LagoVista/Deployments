// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a061edbc8d51684aedea4b42a0fcf9d7ddf80deedc7842a745a3544c9a10736c
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;

namespace LagoVista.IoT.Deployment.Models
{
    public class SharedInstanceSummary
    {
        public EntityHeader Instance { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader<DeploymentInstanceStates> Status {get;set;}
        public string DnsHostName { get; set; }
    
        public string SharedAccessKey1 { get; set; }
        public string SharedAccessKey2 { get; set; }
    }
}
