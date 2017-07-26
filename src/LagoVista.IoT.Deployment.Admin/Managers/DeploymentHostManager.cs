using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class InUseException : Exception { }

    public class DeploymentHostManager : ManagerBase, IDeploymentHostManager
    {
        IDeploymentHostRepo _deploymentHostRepo;
        IDeploymentInstanceRepo _deploymentInstanceRepo;
        IDeploymentConnectorService _deploymentConnectorService;
        IDeploymentActivityQueueManager _deploymentActivityQueueManager;
        IDeploymentActivityRepo _deploymentActivityRepo;

        public DeploymentHostManager(IDeploymentHostRepo deploymentHostRepo, IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentActivityRepo deploymentActivityRepo,
                IDeploymentConnectorService deploymentConnectorService, IDeploymentInstanceRepo deploymentInstanceRepo, IAdminLogger logger,
                IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deploymentHostRepo = deploymentHostRepo;
            _deploymentInstanceRepo = deploymentInstanceRepo;
            _deploymentConnectorService = deploymentConnectorService;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _deploymentActivityRepo = deploymentActivityRepo;
        }

        public async Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Create);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _deploymentHostRepo.AddDeploymentHostAsync(host);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteDeploymentHostAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(host);
            await _deploymentHostRepo.DeleteDeploymentHostAsync(instanceId);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await GetDeploymentHostAsync(hostId, org, user);
            await AuthorizeAsync(user, org, "hostDeployedInstances", hostId);
            return await _deploymentConnectorService.GetDeployedInstancesAsync(host, org, user);
        }

        public async Task<DeploymentHost> GetDeploymentHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<InvokeResult> DeployHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "deploy");
            if (host.Status.Value != HostStatus.Offline)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStartNotStopped.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.Create)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }

        public Task<InvokeResult> StartHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            return Task.FromResult(InvokeResult.FromErrors(new ErrorMessage("Start currently not supported, use Deploy, start will eventually be used to power up a VM that hasn't been destoryed.")));
            /*
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "start");
            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStartNotStopped.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.Start)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
            */
        }

        public async Task<InvokeResult> ResetHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "reset");

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.Reset)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }

        public Task<InvokeResult> StopHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            return Task.FromResult(InvokeResult.FromErrors(new ErrorMessage("Stop currently not supported, use Destroy, stop will eventually be used to power down a VM without destorying it.")));
            /*
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "stop");
            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStopNotRunning.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.Stop)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
            */
        }

        public async Task<IEnumerable<DeploymentActivity>> GetHostActivitesAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentHost), Actions.Read);

            return await _deploymentActivityRepo.GetForResourceIdAsync(id);
        }

        public async Task<InvokeResult> DestroyHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "destory");
            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStopNotRunning.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.Remove)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }

        public async Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentHost));
            return await _deploymentHostRepo.GetDeploymentsForOrgAsync(orgId);
        }

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(String hostId, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentHost));
            return await _deploymentInstanceRepo.GetInstanceForHostAsync(hostId);
        }

        public async Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetMCPHostAsync();
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org, "notifications");
            return host;
        }



        public async Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetNotificationsHostAsync();
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org, "mcp");
            return host;
        }


        public Task<DeploymentHost> LoadFullDeploymentHostAsync(string id)
        {
            return _deploymentHostRepo.GetDeploymentHostAsync(id);
        }

        public Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org)
        {
            return _deploymentHostRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> RegenerateAccessKeys(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.GenerateAccessKeys();
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }
    }
}
