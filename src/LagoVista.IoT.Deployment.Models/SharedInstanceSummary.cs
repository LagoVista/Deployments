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
    }
}
