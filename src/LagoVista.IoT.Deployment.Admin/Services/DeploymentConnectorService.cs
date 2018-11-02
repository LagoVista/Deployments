using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeploymentConnectorService : ConnectorServiceBase, IDeploymentConnectorService
    {
        public DeploymentConnectorService(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger) : base(deploymentHostRepo, logger)
        {
        }

        public Task<InvokeResult<string>> GetRemoteMonitoringUriAsync(DeploymentHost host, string channel, string id, string verbosity, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/websocket/{channel}/{id}/{verbosity}";
            return GetAsync<string>(path, host, org, user);
        }

        public async Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/instances";
            var callResponse = await GetAsync<ListResponse<InstanceRuntimeSummary>>(path, host, org, user);

            if (callResponse.Successful)
            {
                if (callResponse.Result == null)
                {
                    var failedResponse = ListResponse<InstanceRuntimeSummary>.Create(null);
                    failedResponse.Errors.Add(new ErrorMessage("Null Response From Server."));
                    return failedResponse;
                }
                else
                {
                    return callResponse.Result;
                }
            }
            else
            {
                var failedResponse = ListResponse<InstanceRuntimeSummary>.Create(null);
                failedResponse.Concat(callResponse);
                return failedResponse;
            }
        }

        public Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/{instanceId}";
            return GetAsync<InstanceRuntimeDetails>(path, host, org, user);
        }

        public Task<InvokeResult> DeployAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/deploy/{instanceId}";
            return GetAsync(path, host, org, user);
        }

        public Task<InvokeResult> PauseAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/pause/{instanceId}";
            return GetAsync(path, host, org, user);
        }

        public Task<InvokeResult> RemoveAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/remove/{instanceId}";
            return GetAsync(path, host, org, user);
        }

        public Task<InvokeResult> StartAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/start/{instanceId}";
            return GetAsync(path, host, org, user);
        }

        public Task<InvokeResult> StopAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/stop/{instanceId}";
            return GetAsync(path, host, org, user);
        }

        public Task<InvokeResult> UpdateAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/update/{instanceId}";
            return GetAsync(path, host, org, user);
        }
    }
}