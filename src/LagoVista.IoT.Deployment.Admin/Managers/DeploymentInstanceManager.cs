//#define WEBSERVERSIDECHECK

using LagoVista.Core;
using LagoVista.Core.Exceptions;
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
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManager : DeploymentInstanceManagerCore, IDeploymentInstanceManager
    {
        #region const
        public const string DeploymentAction_Deploy = "Instance.Deploy";
        public const string DeploymentAction_Start = "Instance.Start";
        public const string DeploymentAction_Pause = "Instance.Pause";
        public const string DeploymentAction_Stop = "Instance.Stop";
        public const string DeploymentAction_Remove = "Instance.Remove";
        public const string DeploymentAction_Monitor = "Instance.Monitor";
        #endregion

        #region private fields
        private ISecureStorage _secureStorage;
        private IDeploymentInstanceRepo _instanceRepo;
        private ISolutionManager _solutionManager;
        private IDeploymentHostManager _hostManager;
        private IDeploymentHostRepo _hostRepo;
        private IAppConfig _appConfig;
        private ISolutionVersionRepo _solutionVersionRepo;
        private readonly IDeploymentConnectorService _connector;
        private IDeviceRepositoryManager _deviceRepoManager;
        private IDeploymentActivityQueueManager _deploymentActivityQueueManager;
        private IDeploymentInstanceStatusRepo _deploymentInstanceStatusRepo;
        private readonly IProxyFactory _proxyFactory;
        #endregion

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISecureStorage secureStorage, ISolutionVersionRepo solutionVersionRepo) :
            base(hostManager, instanceRepo, deviceManagerRepo, deploymentStatusInstanceRepo, logger, appConfig, depmanager, secureStorage, security)
        {
            _hostManager = hostManager;
            _appConfig = appConfig;
            _instanceRepo = instanceRepo;
            _solutionManager = solutionManager;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _solutionVersionRepo = solutionVersionRepo;
            _connector = connector;
            _deviceRepoManager = deviceRepoManager;
            _hostRepo = hostRepo;
            _secureStorage = secureStorage;
            _deploymentInstanceStatusRepo = deploymentStatusInstanceRepo;
        }

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISolutionVersionRepo solutionVersionRepo, ISecureStorage secureStorage, IProxyFactory proxyFactory) :
            this(deviceRepoManager, connector, hostManager, deviceManagerRepo, deploymentActivityQueueManager, instanceRepo, solutionManager, hostRepo, deploymentStatusInstanceRepo, logger, appConfig, depmanager, security, secureStorage, solutionVersionRepo)
        {
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        protected IDeploymentConnectorService GetConnector(DeploymentHost host, string organizationId, string instanceId)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            return IsRpc(host)
                ? _proxyFactory.Create<IDeploymentConnectorService>(new ProxySettings
                {
                    OrganizationId = organizationId ?? throw new ArgumentNullException(nameof(organizationId)),
                    InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId))
                })
                : _connector;
        }

        protected bool IsRpc(DeploymentHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            //"v1.5" and up == rpc
            var version = Version.Parse(host.ContainerTag.Text.ToLower().Replace("v", ""));
            return version.Major > 1 || (version.Major == 1 && version.Minor >= 5);
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
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DeployHost, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.DeployHost);
        }

        public async Task<InvokeResult> StartAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (IsRpc(host))
            {
                return !transitionResult.Successful
                ? transitionResult
                : await GetConnector(host, org.Id, id).StartAsync(host, id, org, user);
            }
            else
            {

                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Start);
            }
        }

        public async Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (IsRpc(host))
            {
                return !transitionResult.Successful
                ? transitionResult
                : await GetConnector(host, org.Id, id).PauseAsync(host, id, org, user);
            }
            else
            {
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Pause);
            }
        }

        public async Task<InvokeResult> StopAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (IsRpc(host))
            {
                return !transitionResult.Successful
                ? transitionResult
                : await GetConnector(host, org.Id, id).StopAsync(host, id, org, user);
            }
            else
            {
                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Stop);
            }
        }

        public async Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartHost);
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
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.UpdateRuntime, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.UpdateRuntime);
        }

        public async Task<InvokeResult> ReloadSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.ReloadSolution, org, user);

            if (IsRpc(host))
            {
                return !transitionResult.Successful
                    ? transitionResult
                    : await GetConnector(host, org.Id, id).UpdateAsync(host, id, org, user);
            }
            else
            {

                return !transitionResult.Successful
                    ? transitionResult
                    : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.ReloadSolution);
            }
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
            return await GetConnector(notificationHost, org.Id, id).GetRemoteMonitoringUriAsync(notificationHost, channel, id, verbosity, org, user);
        }

        public async Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);

            var host = await _hostManager.GetDeploymentHostAsync(instance.PrimaryHost.Id, org, user);
            await AuthorizeAsync(user, org, "instanceRuntimeDetails", instanceId);
            return await GetConnector(host, org.Id, instanceId).GetInstanceDetailsAsync(host, instanceId, org, user);
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
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<IDeploymentInstanceStatusRepo>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetStatusHistoryForInstanceAsync(instanceId, listRequest);
            }
            else
            {
                return await _deploymentInstanceStatusRepo.GetStatusHistoryForInstanceAsync(instanceId, listRequest);
            }
        }

        public async Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            if (EntityHeader.IsNullOrEmpty(instance.DeploymentConfiguration))
            {
                instance.DeploymentConfiguration = EntityHeader<DeploymentConfigurations>.Create(DeploymentConfigurations.SingleInstance);
            }

            if (EntityHeader.IsNullOrEmpty(instance.DeploymentType))
            {
                instance.DeploymentType = EntityHeader<DeploymentTypes>.Create(DeploymentTypes.Managed);
            }

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

        public async Task<InvokeResult<DeploymentInstance>> LoadFullInstanceWithVersionAsync(string id, string version, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            instance.DeviceRepository.Value = await _deviceRepoManager.GetDeviceRepositoryWithSecretsAsync(instance.DeviceRepository.Id, org, user);

            var solution = await _solutionVersionRepo.GetSolutionVersionAsync(id, version);
            instance.Solution.Value = solution;
            instance.Solution.Id = instance.Solution.Value.Id;
            instance.Solution.Text = instance.Solution.Value.Name;
            return InvokeResult<DeploymentInstance>.Create(instance);
        }

        public async Task<InvokeResult<string>> GetKeyAsync(KeyRequest request, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(request.InstanceId);

            if (String.IsNullOrEmpty(request.Key))
            {
                throw new InvalidDataException("Missing Request Key");
            }

            if (request.Key.Length != 32)
            {
                throw new InvalidDataException("Invalid Request Key Length");
            }

            if (String.IsNullOrEmpty(request.InstanceId))
            {
                throw new InvalidDataException("Invalid Instance Key");
            }

            if (request.InstanceId.Length != 32)
            {
                throw new InvalidDataException("Invalid Instance Key");
            }

            var key1 = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKey1, user);
            if (key1.Result != request.InstanceAccessKey)
            {
                var key2 = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKeySecureId2, user);
                if (key2.Result != request.InstanceAccessKey)
                {
                    throw new UnauthorizedAccessException("Could not validate instance access key.");
                }
            }

            return await _secureStorage.GetSecretAsync(org, request.Key, user);
        }

        public async Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            var settings = new DeploymentSettings();

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org, "fullDeploymentSettings");

            var keyUpdated = false;

            if (String.IsNullOrEmpty(instance.SharedAccessKeySecureId1))
            {
                var key1 = GenerateRandomKey();
                var addKeyOneResult = await _secureStorage.AddSecretAsync(org, key1);
                if (!addKeyOneResult.Successful)
                {
                    return InvokeResult<DeploymentSettings>.FromError("Could generate access key 1, please try again later.");
                }

                instance.SharedAccessKeySecureId1 = key1;
                settings.SharedAccessKey1 = key1;
                keyUpdated = true;
            }
            else
            {
                var getKeyOneResult = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKey1, user);
                if (!getKeyOneResult.Successful)
                {
                    return InvokeResult<DeploymentSettings>.FromError("Could not get access key 1, please try again later.");
                }

                settings.SharedAccessKey1 = getKeyOneResult.Result;
            }

            if (String.IsNullOrEmpty(instance.SharedAccessKeySecureId2))
            {
                var key2 = GenerateRandomKey();
                var addKeyTwoResult = await _secureStorage.AddSecretAsync(org, key2);
                if (!addKeyTwoResult.Successful)
                {
                    return InvokeResult<DeploymentSettings>.FromError("Could generate access key 2, please try again later.");
                }

                instance.SharedAccessKeySecureId2 = key2;
                settings.SharedAccessKey2 = key2;
                keyUpdated = true;
            }
            else
            {
                var getKeyOneResult = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKey1, user);
                if (!getKeyOneResult.Successful)
                {
                    return InvokeResult<DeploymentSettings>.FromError("Could not get access key 2, please try again later.");
                }

                settings.SharedAccessKey2 = getKeyOneResult.Result;
            }

            if (keyUpdated)
            {
                await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Update, user, org, "Generate Shared Access Keys");

                instance.LastUpdatedBy = user;
                instance.LastUpdatedDate = DateTime.Now.ToJSONString();
                await _instanceRepo.UpdateInstanceAsync(instance);
            }

            settings.InstanceId = instance.Id;
            settings.HostId = instance.PrimaryHost?.Id;

            var solution = await _solutionManager.GetSolutionAsync(instance.Solution.Id, org, user);

            var cmd = new StringBuilder("docker run -d ");
            foreach (var listener in solution.Listeners)
            {
                if (listener.Value.ListenOnPort.HasValue)
                {
                    cmd.Append(listener.Value.ListenOnPort.ToString());
                }
            }

            cmd.Append($"-e InstanceId={instanceId} ");

            if (instance.PrimaryHost != null)
            {
                cmd.Append($"-e HostId={instance.PrimaryHost.Id} ");
            }
            cmd.Append($"-e Environment={_appConfig.Environment.ToString()} ");

            cmd.Append($"-e DeploymentType={instance.DeploymentType.Value.ToString()} ");
            cmd.Append($"-e DeploymentConfig={instance.DeploymentType.Value.ToString()} ");
            cmd.Append($"-e QueueType={instance.QueueType.Value.ToString()} ");
            cmd.Append($"--name={instance.Key} ");
            cmd.Append("--restart unless-stopped ");
            cmd.Append($"{instance.ContainerRepository.Id}:{instance.ContainerTag.Id}");

            settings.QueueType = instance.QueueType;
            settings.DeploymentType = instance.DeploymentType;
            settings.DeploymentConfiguration = instance.DeploymentConfiguration;
            settings.DockerCommandLine = cmd.ToString();

            return InvokeResult<DeploymentSettings>.Create(settings);
        }

        public string GenerateAccessKey()
        {
            return GenerateRandomKey();
        }
    }
}
