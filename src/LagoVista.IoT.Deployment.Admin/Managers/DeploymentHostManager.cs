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
using LagoVista.Core;

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
        IDeploymentHostStatusRepo _deploymentHostStatusRepo;

        public DeploymentHostManager(IDeploymentHostRepo deploymentHostRepo, IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentActivityRepo deploymentActivityRepo,
                IDeploymentConnectorService deploymentConnectorService, IDeploymentInstanceRepo deploymentInstanceRepo, IAdminLogger logger, IDeploymentHostStatusRepo deploymentHostStatusRepo,
                IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deploymentHostRepo = deploymentHostRepo;
            _deploymentInstanceRepo = deploymentInstanceRepo;
            _deploymentConnectorService = deploymentConnectorService;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _deploymentActivityRepo = deploymentActivityRepo;
            _deploymentHostStatusRepo = deploymentHostStatusRepo;
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

        public async Task<DeploymentHost> GetDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId, throwOnNotFound);
            if (host == null)
            {
                return host;
            }

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<InvokeResult> DeployHostAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "deploy");
            if (host.Status.Value != HostStatus.Offline)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStartNotStopped.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, hostId, DeploymentActivityTaskTypes.DeployHost)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeployContainerAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "deploycontainer");
            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CannotDeployContainerToNonRunningHost.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.InstanceContainer, hostId, DeploymentActivityTaskTypes.DeployContainer)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }
      

        public async Task<InvokeResult> RestartHostAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "reset");

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, hostId, DeploymentActivityTaskTypes.RestartHost)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
            });

            return InvokeResult.Success;
        }      

        public async Task<IEnumerable<DeploymentActivitySummary>> GetHostActivitesAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentActivitySummary), Actions.Read);

            return await _deploymentActivityRepo.GetForResourceIdAsync(id);
        }

        public async Task<InvokeResult> DestroyHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Perform, user, org, "destoryhost");
            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStopNotRunning.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.DestroyHost)
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

        public Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user)
        {
            return _deploymentHostRepo.GetMCPHostAsync();
        }

        public Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user)
        {
            return _deploymentHostRepo.GetNotificationsHostAsync();
        }

        public Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org)
        {
            return _deploymentHostRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> RegenerateAccessKeys(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.GenerateAccessKeys();
            host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            host.LastUpdatedBy = user;
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);

            var existingHost = await _deploymentHostRepo.GetDeploymentHostAsync(host.Id);

            if(host.Status.Value != existingHost.Status.Value)
            {
                await UpdateDeploymentHostStatusAsync(host.Id, host.Status.Value, org, user);
            }

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            host.LastUpdatedBy = user;
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeploymentHostStatusAsync(string hostId, HostStatus hostStatus, EntityHeader org, EntityHeader user, string details = "", string cpu = "", string memory = "")
        {
            var host = await GetDeploymentHostAsync(hostId, org, user);

            var hostStatusUpdate = Models.DeploymentHostStatus.Create(hostId, user);
            hostStatusUpdate.OldState = host.Status.Value.ToString();
            hostStatusUpdate.NewState = hostStatus.ToString();
            await _deploymentHostStatusRepo.AddDeploymentHostStatusAsync(hostStatusUpdate);

            host.Status = EntityHeader<HostStatus>.Create(hostStatus);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            host.LastUpdatedBy = user;

            if(string.IsNullOrEmpty(details)) host.StatusDetails = details;
            if (string.IsNullOrEmpty(memory)) host.AverageMemory = memory;
            if (string.IsNullOrEmpty(cpu)) host.AverageCPU = cpu;

            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }
    }
}
