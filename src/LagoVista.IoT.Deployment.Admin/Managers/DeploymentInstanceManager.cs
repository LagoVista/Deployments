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
using System.Linq;
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
        private IAdminLogger _adminLogger;
        private IDeploymentInstanceRepo _instanceRepo;
        private ISolutionManager _solutionManager;
        private IDeploymentHostManager _hostManager;
        private IDeploymentHostRepo _hostRepo;
        private IAppConfig _appConfig;
        private IContainerRepositoryManager _containerRepoMgr;
        private ISolutionVersionRepo _solutionVersionRepo;
        private readonly IDeploymentConnectorService _connector;
        private IDeviceRepositoryManager _deviceRepoManager;
        private IDeploymentActivityQueueManager _deploymentActivityQueueManager;
        private IDeploymentInstanceStatusRepo _deploymentInstanceStatusRepo;
        private readonly IProxyFactory _proxyFactory;
        #endregion

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISecureStorage secureStorage, ISolutionVersionRepo solutionVersionRepo,
                    IContainerRepositoryManager containerRepoMgr) :
            base(hostManager, instanceRepo, deviceManagerRepo, secureStorage, deploymentStatusInstanceRepo, logger, appConfig, depmanager, security)
        {
            _hostManager = hostManager;
            _appConfig = appConfig;
            _adminLogger = logger;
            _instanceRepo = instanceRepo;
            _solutionManager = solutionManager;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _solutionVersionRepo = solutionVersionRepo;
            _connector = connector;
            _deviceRepoManager = deviceRepoManager;
            _hostRepo = hostRepo;
            _secureStorage = secureStorage;
            _deploymentInstanceStatusRepo = deploymentStatusInstanceRepo;
            _containerRepoMgr = containerRepoMgr;
        }

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo, 
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISolutionVersionRepo solutionVersionRepo, IContainerRepositoryManager containerRepoMgr, ISecureStorage secureStorage, IProxyFactory proxyFactory) :
            this(deviceRepoManager, connector, hostManager, deviceManagerRepo, deploymentActivityQueueManager, instanceRepo, solutionManager, hostRepo, deploymentStatusInstanceRepo, logger, appConfig, depmanager, security, secureStorage, solutionVersionRepo, containerRepoMgr)
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

        public async Task<InvokeResult> ResetAppAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            await UpdateInstanceStatusAsync(id, DeploymentInstanceStates.Offline, false, null, org, user, "Forcing to offline");
            await _hostManager.UpdateDeploymentHostStatusAsync(instance.PrimaryHost.Id, HostStatus.Offline, null, org, user, "Forcing to offline.");

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

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
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

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
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartHost);
        }

        public async Task<InvokeResult> RestartContainerAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartContainer, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartContainer);
        }

        public async Task<InvokeResult> UpdateRuntimeAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.UpdateRuntime, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.UpdateRuntime);
        }

        public async Task<InvokeResult> ReloadSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

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
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));

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
            MapInstanceProperties(instance);
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

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(NuvIoTEditions edition, string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentInstance));
            return await _instanceRepo.GetInstanceForOrgAsync(edition, orgId);
        }

        public async Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance), Actions.Read);
         
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            MapInstanceProperties(instance);
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
            MapInstanceProperties(instance);
            if (EntityHeader.IsNullOrEmpty(instance.DeploymentConfiguration))
            {
                instance.DeploymentConfiguration = EntityHeader<DeploymentConfigurations>.Create(DeploymentConfigurations.SingleInstance);
            }

            if (EntityHeader.IsNullOrEmpty(instance.DeploymentType))
            {
                instance.DeploymentType = EntityHeader<DeploymentTypes>.Create(DeploymentTypes.Managed);
            }

            MapInstanceProperties(instance);

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            instance.DeviceRepository.Value = await _deviceRepoManager.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);

            var solutionResult = EntityHeader.IsNullOrEmpty(instance.Version) ?
                             await _solutionManager.LoadFullSolutionAsync(instance.Solution.Id, org, user) :
                             await _solutionVersionRepo.GetSolutionVersionAsync(instance.Solution.Id, instance.Version.Id);

            if (solutionResult.Successful)
            {
                instance.Solution.Value = solutionResult.Result;
                return InvokeResult<DeploymentInstance>.Create(instance);
            }
            else
            {
                return InvokeResult<DeploymentInstance>.FromErrors(solutionResult.Errors.ToArray());
            }
        }

        public async Task<InvokeResult<string>> GetKeyAsync(string keyId, EntityHeader instance, EntityHeader org)
        {
            if (EntityHeader.IsNullOrEmpty(org))
            {
                throw new InvalidDataException("Missing Org");
            }

            if (EntityHeader.IsNullOrEmpty(instance))
            {
                throw new InvalidDataException("Missing Instance");
            }

            if (String.IsNullOrEmpty(keyId))
            {
                throw new InvalidDataException("Missing Request Key");
            }

            if (keyId.Length != 32)
            {
                throw new InvalidDataException("Invalid Request Key Length");
            }

            await LogEntityActionAsync(keyId, "Key", "Access", org, instance);

            return await _secureStorage.GetSecretAsync(org, keyId, instance);
        }

        public async Task<InvokeResult<string>> RegenerateKeyAsync(string instanceId, string keyName, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(keyName))
            {
                return InvokeResult<string>.FromError("Key Name is a Required Field.");
            }

            keyName = keyName.ToLower();
            if (keyName != "key1" && keyName != "key2")
            {
                return InvokeResult<string>.FromError($"Currently only [key1] and [key2] are supported, you provided {keyName}.");
            }

            var instance = await GetInstanceAsync(instanceId, org, user);

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Update, user, org, $"RegenerateKey-{keyName}");

            var key = GenerateRandomKey();
            var keyAddResult = await _secureStorage.AddSecretAsync(org, key);
            if (!keyAddResult.Successful)
            {
                _adminLogger.AddError("DeploymentInstanceManager_RegenerateKeyAsync_Add", keyAddResult.Errors.First().Message);
                return InvokeResult<string>.FromError($"Could not add key to secure storage.");
            }

            switch (keyName)
            {
                case "key1":
                    {
                        if (!String.IsNullOrEmpty(instance.SharedAccessKeySecureId1))
                        {
                            //If this fails don't fail the entire method.
                            var removeKeyResult = await _secureStorage.RemoveSecretAsync(org, instance.SharedAccessKeySecureId1);
                            if (!removeKeyResult.Successful)
                            {
                                _adminLogger.AddError("DeploymentInstanceManager_RegenerateKeyAsync_Remove", removeKeyResult.Errors.First().Message);
                            }
                        }
                        instance.SharedAccessKeySecureId1 = keyAddResult.Result;
                    }
                    break;
                case "key2":
                    {
                        if (!String.IsNullOrEmpty(instance.SharedAccessKeySecureId2))
                        {
                            //If this fails don't fail the entire method.
                            var removeKeyResult = await _secureStorage.RemoveSecretAsync(org, instance.SharedAccessKeySecureId2);
                            if (!removeKeyResult.Successful)
                            {
                                _adminLogger.AddError("DeploymentInstanceManager_RegenerateKeyAsync_Remove", removeKeyResult.Errors.First().Message);
                            }
                        }
                        instance.SharedAccessKeySecureId2 = keyAddResult.Result;
                    }
                    break;
            }

            instance.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            instance.LastUpdatedBy = user;

            await _instanceRepo.UpdateInstanceAsync(instance);

            return InvokeResult<string>.Create(key);
        }


        public async Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            MapInstanceProperties(instance);
            var settings = new DeploymentSettings();
            settings.OrgId = org.Id;

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org, "fullDeploymentSettings");

            var keyUpdated = false;

            if (String.IsNullOrEmpty(instance.SharedAccessKeySecureId1))
            {
                var key1 = GenerateRandomKey();
                var addKeyOneResult = await _secureStorage.AddSecretAsync(org, key1);
                if (!addKeyOneResult.Successful)
                {
                    _adminLogger.AddError("DeploymentInstanceManager_GetDeploymentSettingsAsync_AddKeyOne", addKeyOneResult.Errors.First().Message);
                    return InvokeResult<DeploymentSettings>.FromError("Could generate access key 1, please try again later: " + addKeyOneResult.Errors.First().Message);
                }

                instance.SharedAccessKeySecureId1 = addKeyOneResult.Result;
                settings.SharedAccessKey1 = key1;
                keyUpdated = true;
            }
            else
            {
                var getKeyOneResult = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKeySecureId1, user);
                if (!getKeyOneResult.Successful)
                {
                    _adminLogger.AddError("DeploymentInstanceManager_GetDeploymentSettingsAsync_GetKeyOne", getKeyOneResult.Errors.First().Message);
                    return InvokeResult<DeploymentSettings>.FromError("Could not get access key 1, please try again later: " + getKeyOneResult.Errors.First().Message);
                }

                settings.SharedAccessKey1 = getKeyOneResult.Result;
            }

            if (String.IsNullOrEmpty(instance.SharedAccessKeySecureId2))
            {
                var key2 = GenerateRandomKey();
                var addKeyTwoResult = await _secureStorage.AddSecretAsync(org, key2);
                if (!addKeyTwoResult.Successful)
                {
                    _adminLogger.AddError("DeploymentInstanceManager_GetDeploymentSettingsAsync_AddKeyTwo", addKeyTwoResult.Errors.First().Message);
                    return InvokeResult<DeploymentSettings>.FromError("Could not add access key 2, please try again later: " + addKeyTwoResult.Errors.First().Message);
                }

                instance.SharedAccessKeySecureId2 = addKeyTwoResult.Result;
                settings.SharedAccessKey2 = key2;
                keyUpdated = true;
            }
            else
            {
                var getKeyTwoResult = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKeySecureId2, user);
                if (!getKeyTwoResult.Successful)
                {
                    _adminLogger.AddError("DeploymentInstanceManager_GetDeploymentSettingsAsync_GetKeyTwo", getKeyTwoResult.Errors.First().Message);
                    return InvokeResult<DeploymentSettings>.FromError("Could not get access key 2, please try again later: " + getKeyTwoResult.Errors.First().Message);
                }

                settings.SharedAccessKey2 = getKeyTwoResult.Result;
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

            var getSolutionRsult = await _solutionManager.LoadFullSolutionAsync(instance.Solution.Id, org, user);
            if (!getSolutionRsult.Successful)
            {
                _adminLogger.AddError("DeploymentInstanceManager_GetDeploymentSettingsAsync_LoadFullSolutionAsync", getSolutionRsult.Errors.First().Message);
                return InvokeResult<DeploymentSettings>.FromError("Could not get access key 2, please try again later: " + getSolutionRsult.Errors.First().Message);
            }

            if (instance.NuvIoTEdition.Value == NuvIoTEditions.Container)
            {
                var cmd = new StringBuilder("docker run -d ");
                foreach (var listener in getSolutionRsult.Result.Listeners)
                {
                    if (listener.Value.ListenOnPort.HasValue)
                    {
                        var port = listener.Value.ListenOnPort.Value;

                        cmd.Append($"-p {port}:{port} ");
                    }
                }

                cmd.Append($"-p {instance.InputCommandPort}:{instance.InputCommandPort} ");

                cmd.Append($"-e InstanceId={instanceId} ");
                cmd.Append($"-e OrgId={org.Id} ");

                if (!EntityHeader.IsNullOrEmpty(instance.Version))
                {
                    cmd.Append($"-e VersionId={instance.Version.Id} ");
                }

                if (instance.PrimaryHost != null)
                {
                    cmd.Append($"-e HostId={instance.PrimaryHost.Id} ");
                }

                cmd.Append($"-e Environment={_appConfig.Environment.ToString()} ");

                cmd.Append($"-e DeploymentType={instance.DeploymentType.Id} ");
                cmd.Append($"-e DeploymentConfig={instance.DeploymentConfiguration.Id} ");
                cmd.Append($"-e QueueType={instance.QueueType.Id} ");
                cmd.Append($"-e AccessKey={settings.SharedAccessKey1} ");
                cmd.Append($"--name={instance.Key} ");
                cmd.Append("--restart unless-stopped ");

                var containerRepo = await _containerRepoMgr.GetContainerRepoAsync(instance.ContainerRepository.Id, org, user);
                var imageName = $"{containerRepo.Namespace}/{containerRepo.RepositoryName}";

                cmd.Append($"{imageName}:{instance.ContainerTag.Text}");
                settings.DockerCommandLine = cmd.ToString();
            }

            settings.WorkingStorage = instance.WorkingStorage;
            settings.DeploymentType = instance.DeploymentType;
            settings.NuvIoTEdition = instance.NuvIoTEdition;
            
            settings.Name = instance.Name;
            settings.Key = instance.Key;


            return InvokeResult<DeploymentSettings>.Create(settings);
        }

        public string GenerateAccessKey()
        {
            return GenerateRandomKey();
        }
    }
}
