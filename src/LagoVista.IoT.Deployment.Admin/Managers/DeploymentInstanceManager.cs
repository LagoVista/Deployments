//#define WEBSERVERSIDECHECK

using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManager : DeploymentInstanceManagerCore, IDeploymentInstanceManager
    {
        public const string DeploymentAction_Deploy = "Instance.Deploy";
        public const string DeploymentAction_Start = "Instance.Start";
        public const string DeploymentAction_Pause = "Instance.Pause";
        public const string DeploymentAction_Stop = "Instance.Stop";
        public const string DeploymentAction_Remove = "Instance.Remove";
        public const string DeploymentAction_Monitor = "Instance.Monitor";
        private IDeploymentInstanceRepo _instanceRepo;
        private ISolutionManager _solutionManager;
        private IDeploymentHostManager _hostManager;
        private IDeploymentHostRepo _hostRepo;
        private readonly IDeploymentConnectorService _connector;
        private IDeviceRepositoryManager _deviceRepoManager;
        private IDeploymentActivityQueueManager _deploymentActivityQueueManager;
        private IDeploymentInstanceStatusRepo _deploymentInstanceStatusRepo;
        private readonly IProxyFactory _proxyFactory;

        protected IDeploymentConnectorService GetConnector(DeploymentHost host, EntityHeader org)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (org == null)
            {
                throw new ArgumentNullException(nameof(org));
            }

            if (host.DedicatedInstance == null)
            {
                throw new ArgumentNullException(nameof(host.DedicatedInstance));
            }

            return IsRpc(host)
                ? _proxyFactory.Create<IDeploymentConnectorService>(new ProxySettings
                {
                    OrganizationId = org.Id,
                    InstanceId = host.DedicatedInstance.Id
                })
                : _connector;
        }

        protected bool IsRpc(DeploymentHost host)
        {
            //"v1.5" and up == rpc
            var version = Version.Parse(host.ContainerTag.Text.ToLower().Replace("v", ""));
            return version.Major > 1 || (version.Major == 1 && version.Minor >= 5);
        }

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security,
                    IProxyFactory proxyFactory) : base(hostManager, instanceRepo, deviceManagerRepo, deploymentStatusInstanceRepo, logger, appConfig, depmanager, security)
        {
            _hostManager = hostManager;
            _instanceRepo = instanceRepo;
            _solutionManager = solutionManager;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _connector = connector;
            _deviceRepoManager = deviceRepoManager;
            _hostRepo = hostRepo;
            _deploymentInstanceStatusRepo = deploymentStatusInstanceRepo;
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        private async Task<InvokeResult> PerformActionAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user, DeploymentActivityTaskTypes activityType, int timeoutSeconds = 120)
        {
            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(timeoutSeconds));

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, $"{activityType}Instance");

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Instance, instance.Id, activityType)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
                TimeoutTimeStamp = timeout.ToJSONString()
            });

            return InvokeResult.Success;
        }

        private InvokeResult CanTransitionToState(DeploymentHost host, DeploymentInstance instance, DeploymentActivityTaskTypes taskType, EntityHeader org, EntityHeader user)
        {
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                return await GetConnector(host, org).DeployAsync(host, id, org, user);
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DeployHost, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.DeployHost);
            }
        }

        public async Task<InvokeResult> StartAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                return await GetConnector(host, org).StartAsync(host, id, org, user);
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Start);
            }
        }

        public async Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                return await GetConnector(host, org).PauseAsync(host, id, org, user);
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Pause, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Pause);
            }
        }

        public async Task<InvokeResult> StopAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                return await GetConnector(host, org).StopAsync(host, id, org, user);
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Stop, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Stop);
            }
        }

        public async Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                var response = await GetConnector(host, org).StopAsync(host, id, org, user);
                return response.Successful
                    ? await GetConnector(host, org).StartAsync(host, id, org, user)
                    : response;
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartHost, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartHost);
            }
        }

        public async Task<InvokeResult> RestartContainerAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartContainer, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartContainer);
        }

        public async Task<InvokeResult> UpdateRuntimeAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            if (IsRpc(host))
            {
                return await GetConnector(host, org).UpdateAsync(host, id, org, user);
            }
            else
            {
                var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.UpdateRuntime, org, user);
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.UpdateRuntime);
            }
        }

        public async Task<InvokeResult> ReloadSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.ReloadSolution, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.ReloadSolution);
        }

        public async Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DestroyHost, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.DestroyHost);
        }

        public async Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(user, org, $"wsrequest.{channel}", id);
            var notificationHost = await _hostManager.GetNotificationsHostAsync(org, user);
            return await GetConnector(notificationHost, org).GetRemoteMonitoringUriAsync(notificationHost, channel, id, verbosity, org, user);
        }

        public async Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            var host = await _hostManager.GetDeploymentHostAsync(instance.PrimaryHost.Id, org, user);
            await AuthorizeAsync(user, org, "instanceRuntimeDetails", instanceId);
            return await GetConnector(host, org).GetInstanceDetailsAsync(host, instanceId, org, user);
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(instance);
        }

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentInstance));
            return await _instanceRepo.GetInstanceForOrgAsync(orgId);
        }

        public async Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance), Actions.Read);
            return await _deploymentInstanceStatusRepo.GetStatusHistoryForInstanceAsync(instanceId, listRequest);
        }

        public async Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            instance.DeviceRepository.Value = await _deviceRepoManager.GetDeviceRepositoryWithSecretsAsync(instance.DeviceRepository.Id, org, user);

            var solutionResult = await _solutionManager.LoadFullSolutionAsync(instance.Solution.Id, org, user);
            if (solutionResult.Successful)
            {
                instance.Solution.Value = solutionResult.Result;
                instance.Solution.Id = instance.Solution.Value.Id;
                instance.Solution.Text = instance.Solution.Value.Name;
                return InvokeResult<DeploymentInstance>.Create(instance);
            }
            else
            {
                return InvokeResult<DeploymentInstance>.FromErrors(solutionResult.Errors.ToArray());
            }
        }
    }
}
