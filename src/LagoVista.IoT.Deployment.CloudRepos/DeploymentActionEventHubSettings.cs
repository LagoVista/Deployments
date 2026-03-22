using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using Microsoft.Extensions.Configuration;

namespace LagoVista.IoT.Deployment.Admin
{
    public class DeploymentActionEventHubSettings : IDeploymentActionEventHubSettings
    {
         public IConnectionSettings DeploymentActivityEventHubConnection { get; }
    
        public DeploymentActionEventHubSettings(IConfiguration configuration)
        {
            var mcpSection = configuration.GetRequiredSection("MCPEventHub");

            DeploymentActivityEventHubConnection = new ConnectionSettings()
            {
                AccountId = mcpSection["AccountId"],
                UserName = mcpSection["PolicyName"],
                AccessKey = mcpSection["AccessKey"],
                ResourceName = mcpSection["HubName"]
            };
        }
    }
}
