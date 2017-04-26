using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public interface IDeploymentConnectorService
    {
        Task<InvokeResult> StartAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> PauseAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> UpdateAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StopAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);
    
        Task<InvokeResult> DeployAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> RemoveAsync(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<Uri>> GetRemoteMonitoringUriAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);
    }
}
