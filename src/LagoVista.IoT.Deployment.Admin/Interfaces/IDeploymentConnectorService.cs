﻿using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentConnectorService
    {
        Task<InvokeResult> StartAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> PauseAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> UpdateAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StopAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RemoveAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> GetRemoteMonitoringUriAsync(DeploymentHost host, string channel, string instanceId, string verbosity, EntityHeader org, EntityHeader user);
        Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user);
    }
}
