//#define WEBSERVERSIDECHECK

using LagoVista.Core;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admins;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceAdmin.Interfaces.Managers;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Pipeline.Admin;
using LagoVista.IoT.Pipeline.Admin.Managers;
using LagoVista.IoT.Pipeline.Admin.Models;
using LagoVista.UserAdmin;
using LagoVista.UserAdmin.Interfaces.Managers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly ISecureStorage _secureStorage;
        private readonly IAdminLogger _adminLogger;
        private readonly IDeploymentInstanceRepo _instanceRepo;
        private readonly ISolutionManager _solutionManager;
        private readonly IDeploymentHostManager _hostManager;
        private readonly IPipelineModuleManager _pipelineModuleManager;
        private readonly IDeploymentHostRepo _hostRepo;
        private readonly IAppConfig _appConfig;
        private readonly IContainerRepositoryManager _containerRepoMgr;
        private readonly ISolutionVersionRepo _solutionVersionRepo;
        private readonly IDeploymentConnectorService _connector;
        private readonly IDataStreamManager _dataStreamManager;
        private readonly IDeviceRepositoryManager _deviceRepoManager;
        private readonly IDeploymentActivityQueueManager _deploymentActivityQueueManager;
        private readonly IDeploymentInstanceStatusRepo _deploymentInstanceStatusRepo;
        private readonly IProxyFactory _proxyFactory;
        private readonly IUserManager _userManager;
        private readonly IDeviceTypeManager _deviceTypeManager;
        private readonly IInstanceAccountsRepo _instanceAccountRepo;
        private readonly IOrgUtils _orgUtils;
        private readonly ICacheProvider _cacheProvider;
        private readonly IRemoteServiceManager _remoteListenerServiceManager;
        private readonly LagoVista.IoT.DeviceManagement.Core.IDeviceManager _deviceManager;
        private readonly IDeviceStatusRepo _deviceStatusRepo;
        #endregion

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IUserManager userManager, IDataStreamManager dataStreamManager, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISecureStorage secureStorage, ISolutionVersionRepo solutionVersionRepo,
                    IContainerRepositoryManager containerRepoMgr, IPipelineModuleManager pipelineModuleManager, IDeviceTypeManager deviceTypeManager, IInstanceAccountsRepo instanceAccountRepo, IOrgUtils orgUtils, LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager,
                    IDeviceStatusRepo deviceStatusRepo, ICacheProvider cacheProvider, IRemoteServiceManager remoteListenerServiceManager) :
            base(hostManager, instanceRepo, deviceManagerRepo, secureStorage, deploymentStatusInstanceRepo, userManager, logger, appConfig, depmanager, security)
        {
            _hostManager = hostManager ?? throw new ArgumentNullException(nameof(hostManager));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _adminLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            _instanceRepo = instanceRepo ?? throw new ArgumentNullException(nameof(instanceRepo));
            _solutionManager = solutionManager ?? throw new ArgumentNullException(nameof(solutionManager));
            _deploymentActivityQueueManager = deploymentActivityQueueManager ?? throw new ArgumentNullException(nameof(deploymentActivityQueueManager));
            _solutionVersionRepo = solutionVersionRepo ?? throw new ArgumentNullException(nameof(solutionVersionRepo));
            _connector = connector ?? throw new ArgumentNullException(nameof(connector));
            _deviceRepoManager = deviceRepoManager ?? throw new ArgumentNullException(nameof(deviceRepoManager));
            _hostRepo = hostRepo ?? throw new ArgumentNullException(nameof(hostRepo));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _deploymentInstanceStatusRepo = deploymentStatusInstanceRepo ?? throw new ArgumentNullException(nameof(deploymentStatusInstanceRepo));
            _containerRepoMgr = containerRepoMgr ?? throw new ArgumentNullException(nameof(containerRepoMgr));
            _dataStreamManager = dataStreamManager ?? throw new ArgumentNullException(nameof(dataStreamManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _pipelineModuleManager = pipelineModuleManager ?? throw new ArgumentNullException(nameof(pipelineModuleManager));
            _deviceTypeManager = deviceTypeManager ?? throw new ArgumentNullException(nameof(deviceTypeManager));
            _instanceAccountRepo = instanceAccountRepo ?? throw new ArgumentNullException(nameof(instanceAccountRepo));
            _orgUtils = orgUtils ?? throw new ArgumentNullException(nameof(orgUtils));
            _remoteListenerServiceManager = remoteListenerServiceManager ?? throw new ArgumentNullException(nameof(remoteListenerServiceManager));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _deviceStatusRepo = deviceStatusRepo ?? throw new ArgumentNullException(nameof(deviceStatusRepo));
        
        }

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IUserManager userManager, IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IDataStreamManager dataStreamManager, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security, ISolutionVersionRepo solutionVersionRepo, IContainerRepositoryManager containerRepoMgr, ISecureStorage secureStorage,
                    IDeviceStatusRepo deviceStatusRepo, IProxyFactory proxyFactory, IPipelineModuleManager pipelineModuleManager, IDeviceTypeManager deviceTypeManager, IInstanceAccountsRepo instanceAccountRepo, IOrgUtils orgUtils, 
                    LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, ICacheProvider cacheProvider, IRemoteServiceManager remoteListenerServiceManager) :
            this(deviceRepoManager, connector, hostManager, deviceManagerRepo, deploymentActivityQueueManager, instanceRepo, solutionManager, hostRepo, deploymentStatusInstanceRepo, userManager,
                dataStreamManager, logger, appConfig, depmanager, security, secureStorage, solutionVersionRepo, containerRepoMgr, pipelineModuleManager, deviceTypeManager, instanceAccountRepo, orgUtils, deviceManager, deviceStatusRepo, cacheProvider, remoteListenerServiceManager)
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

        private async Task CheckOwnershipOrSysAdminAsync(IOwnedEntity entity, EntityHeader org, EntityHeader user, [CallerMemberName] string actionType = "")
        {
            if (entity.OwnerOrganization.Id != org.Id)
            {
                var sysUser = await _userManager.FindByIdAsync(user.Id);
                if (sysUser.IsSystemAdmin)
                    await LogEntityActionAsync(entity.Id, entity.GetType().Name, $"sys_admin=>{actionType}", org, user);
                else
                    await AuthorizeAsync(entity, AuthorizeResult.AuthorizeActions.Read, user, org, actionType);
            }
            else
                await AuthorizeAsync(entity, AuthorizeResult.AuthorizeActions.Read, user, org, actionType);
        }

        protected bool IsRpc(DeploymentHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (host.HostType.Value == HostTypes.Shared)
                return true;

            //"v1.5" and up == rpc
            var version = Version.Parse(host.ContainerTag.Text.ToLower().Replace("v", ""));
            return version.Major > 1 || (version.Major == 1 && version.Minor >= 5);
        }

        protected override async Task<InvokeResult> PerformActionAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user, DeploymentActivityTaskTypes activityType, int timeoutSeconds = 120)
        {
            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(timeoutSeconds));

            await CheckOwnershipOrSysAdminAsync(instance, org, user, $"{activityType}Instance");

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
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

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
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            await CheckOwnershipOrSysAdminAsync(instance, org, user);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            await UpdateInstanceStatusAsync(id, DeploymentInstanceStates.Offline, false, null, org, user, "Forcing to offline");
            await _hostManager.UpdateDeploymentHostStatusAsync(instance.PrimaryHost.Id, HostStatus.Offline, null, org, user, "Forcing to offline.");

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (IsRpc(host))
            {
                await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));
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
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (IsRpc(host))
            {
                await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));
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
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            await CheckOwnershipOrSysAdminAsync(instance, org, user, "RestartHost");

            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartHost);
        }

        public async Task<InvokeResult> RestartContainerAsync(string id, EntityHeader org, EntityHeader user)
        {
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);
            await CheckOwnershipOrSysAdminAsync(instance, org, user, "RestartContainer");

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartContainer, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartContainer);
        }

        public async Task<InvokeResult> UpdateRuntimeAsync(string id, EntityHeader org, EntityHeader user)
        {
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

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

            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

            if (IsRpc(host))
            {
                await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentInstance));
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

        public async Task<InvokeResult> UpgradeInstanceAsync(string instanceId, string imageId, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(instanceId)) throw new ArgumentNullException(nameof(instanceId));
            if (EntityHeader.IsNullOrEmpty(org)) throw new ArgumentNullException(nameof(org));
            if (EntityHeader.IsNullOrEmpty(user)) throw new ArgumentNullException(nameof(user));

            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(instanceId));

            var instance = await _instanceRepo.GetInstanceAsync(instanceId);

            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var repo = await _containerRepoMgr.GetContainerRepoAsync(instance.ContainerRepository.Id, org, user);
            var tag = repo.Tags.Where(tg => tg.Id == imageId).FirstOrDefault();
            if (tag == null)
            {
                throw new RecordNotFoundException(nameof(TaggedContainer), imageId);
            }

            instance.ContainerTag = EntityHeader.Create(tag.Id, tag.Name);
            await _instanceRepo.UpdateInstanceAsync(instance);

            //TODO: This may not be correct, on checking, for transition to DestoryHost for upgraded.
            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DestroyHost, org, user);
            return !transitionResult.Successful
                ? transitionResult
                : await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.UpdateRuntime);
        }

        public async Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await CheckOwnershipOrSysAdminAsync(instance, org, user);
            await _cacheProvider.RemoveAsync(DeviceConfigurationManager.GetDeviceConfigMetaDataKey(id));

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

        public async Task<InvokeResult<string>> GetRemoteMonitoringURIForDeviceWithPINAsync(string channel, string repoId, string id, string pin, string verbosity, EntityHeader org, EntityHeader user)
        {
            var repo = await _deviceRepoManager.GetDeviceRepositoryWithSecretsAsync(repoId, org, user, pin);

            // if we can load it, we know we have a good PIN, which is what we care about.
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user, false);
            await AuthorizeAsync(user, org, $"wsrequest.{channel}", id);
            var notificationHost = await _hostManager.GetNotificationsHostAsync(org, user);
            return await GetConnector(notificationHost, org.Id, id).GetRemoteMonitoringUriAsync(notificationHost, channel, id, verbosity, org, user);
        }


        public async Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            await CheckOwnershipOrSysAdminAsync(instance.Result, org, user);

            MapInstanceProperties(instance.Result);
            var host = await _hostManager.GetDeploymentHostAsync(instance.Result.PrimaryHost.Id, org, user);
            return await GetConnector(host, org.Id, instanceId).GetInstanceDetailsAsync(host, instanceId, org, user);
        }

        public async Task<ListResponse<DeviceStatus>> GetWatchdogConnectedDevicesAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            await CheckOwnershipOrSysAdminAsync(instance.Result, org, user);
            
            return await _deviceStatusRepo.GetDeviceStatusForInstanceAsync(listRequest, instance.Result);
        }

        public async Task<InvokeResult> SetSilenceAlarmAsync(string instanceId, string id, bool silenced, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            await CheckOwnershipOrSysAdminAsync(instance.Result, org, user);          
            var deviceStatus = await _deviceStatusRepo.GetDeviceStatusAsync(instance.Result, id);
            deviceStatus.SilenceAlarm = silenced;
            await _deviceStatusRepo.UpdateDeviceStatusAsync(instance.Result, deviceStatus);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<DeviceStatus>> GetTimedoutDevicesAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            await CheckOwnershipOrSysAdminAsync(instance.Result, org, user);
            return await _deviceStatusRepo.GetTimedOutDeviceStatusForInstanceAsync(listRequest, instance.Result);
        }

        public async Task<ListResponse<DeviceStatus>> GetStatusHistoryForDeviceAsync(string instanceId, string deviceUniqueId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);
            await CheckOwnershipOrSysAdminAsync(instance.Result, org, user);
            return await _deviceStatusRepo.GetDeviceStatusHistoryForDeviceAsync(listRequest, instance.Result, deviceUniqueId);
        }

        public  Task<ListResponse<WatchdogMessageStatus>> GetWatchdogMessageStatusAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var proxy = _proxyFactory.Create<IMessageWatchdogService>(new ProxySettings
            {
                OrganizationId = org.Id ?? throw new ArgumentNullException(nameof(org)),
                InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId))
            });

            return proxy.GetWatchdogMessageStatusAsync(listRequest);
        }

        public Task<ListResponse<WatchdogMessageStatus>> GetTimedOutWatchdogMessageStatusAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var proxy = _proxyFactory.Create<IMessageWatchdogService>(new ProxySettings
            {
                OrganizationId = org.Id ?? throw new ArgumentNullException(nameof(org)),
                InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId))
            });

            return proxy.GetTimedOutWatchdogMessageStatusAsync(listRequest);
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(instance);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetAllInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest)
        {
            var sysUser = await _userManager.FindByIdAsync(user.Id);
            if (!sysUser.IsSystemAdmin)
            {
                throw new NotAuthenticatedException($"Attempt to access all instances by non sys admin user {user.Id} - {user.Text} from {org.Text}");
            }
            else
            {
                await AuthorizeAsync(user, org, "sysadmin-get-all_instances");
            }

            return await this._instanceRepo.GetAllInstances(listRqeuest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetInstancesAsync(String orgId, EntityHeader org, EntityHeader user, ListRequest listRqeuest)
        {
            var sysUser = await _userManager.FindByIdAsync(user.Id);
            if (!sysUser.IsSystemAdmin)
            {
                throw new NotAuthenticatedException($"Attempt to access all instances by non sys admin user {user.Id} - {user.Text} from {org.Text}");
            }
            else
            {
                await AuthorizeAsync(user, org, "sysadmin-get-instances-for-org", $"OrgId: {orgId}");
            }

            return await this._instanceRepo.GetInstancesForOrgAsync(orgId, listRqeuest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetActiveInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest)
        {
            var sysUser = await _userManager.FindByIdAsync(user.Id);
            if (!sysUser.IsSystemAdmin)
            {
                throw new NotAuthenticatedException($"Attempt to access all instances by non sys admin user {user.Id} - {user.Text} from {org.Text}");
            }
            else
            {
                await AuthorizeAsync(user, org, "sysadmin-get-all_instances");
            }

            return await this._instanceRepo.GetAllActiveInstancesAsync(listRqeuest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> SysAdminFailedInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest)
        {
            var sysUser = await _userManager.FindByIdAsync(user.Id);
            if (!sysUser.IsSystemAdmin)
            {
                throw new NotAuthenticatedException($"Attempt to access all instances by non sys admin user {user.Id} - {user.Text} from {org.Text}");
            }
            else
            {
                await AuthorizeAsync(user, org, "sysadmin-get-all_instances");
            }

            return await this._instanceRepo.GetAllFailedInstancesAsync(listRqeuest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentInstance));
            return await _instanceRepo.GetInstancesForOrgAsync(orgId, listRequest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstanceForOrgAsync(NuvIoTEditions edition, string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentInstance));
            return await _instanceRepo.GetInstancesForOrgAsync(edition, orgId, listRequest);
        }

        public async Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await CheckOwnershipOrSysAdminAsync(instance, org, user);
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

            foreach (var stream in instance.DataStreams)
            {
                stream.Value = await _dataStreamManager.GetDataStreamAsync(stream.Id, org, user);
                if (!String.IsNullOrEmpty(stream.Value.DBPasswordSecureId))
                {
                    var result = await _secureStorage.GetSecretAsync(org, stream.Value.DBPasswordSecureId, user);
                    if (!result.Successful) return InvokeResult<DeploymentInstance>.FromInvokeResult(result.ToInvokeResult());

                    stream.Value.DbPassword = result.Result;
                }

                if (!String.IsNullOrEmpty(stream.Value.RedisPasswordSecureId))
                {
                    var result = await _secureStorage.GetSecretAsync(org, stream.Value.RedisPasswordSecureId, user);
                    if (!result.Successful) return InvokeResult<DeploymentInstance>.FromInvokeResult(result.ToInvokeResult());

                    stream.Value.RedisPassword = result.Result;
                }

                if (!String.IsNullOrEmpty(stream.Value.AzureAccessKeySecureId))
                {
                    var result = await _secureStorage.GetSecretAsync(org, stream.Value.AzureAccessKeySecureId, user);
                    if (!result.Successful) return InvokeResult<DeploymentInstance>.FromInvokeResult(result.ToInvokeResult());

                    stream.Value.AzureAccessKey = result.Result;
                }

                if (!String.IsNullOrEmpty(stream.Value.AWSSecretKeySecureId))
                {
                    var result = await _secureStorage.GetSecretAsync(org, stream.Value.AWSSecretKeySecureId, user);
                    if (!result.Successful) return InvokeResult<DeploymentInstance>.FromInvokeResult(result.ToInvokeResult());

                    stream.Value.AwsSecretKey = result.Result;
                }
            }

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

            var result = await GetInstanceAsync(instanceId, org, user);
            var instance = result.Result;

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
        public async Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigurationForRepoAsync(string repoId, EntityHeader org, EntityHeader user)
        {
            var repo = await _deviceRepoManager.GetDeviceRepositoryAsync(repoId, org, user);
            return await GetDefaultListenerConfigurationAsync(repo.Instance.Id, org, user);
        }

        public async Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigurationAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var sw = Stopwatch.StartNew();
            var instance = await GetInstanceAsync(instanceId, org, user);
            if (instance == null)
            {
                throw new RecordNotFoundException(nameof(DeploymentInstance), instanceId);
            }

            Console.WriteLine($"Got instance: {sw.ElapsedMilliseconds}ms");

            var solution = await _solutionManager.GetSolutionAsync(instance.Result.Solution.Id, org, user);
            if (solution == null)
            {
                throw new RecordNotFoundException(nameof(Solution), instance.Result.Solution.Id);
            }

            Console.WriteLine($"Got solution: {sw.ElapsedMilliseconds}ms");

            if (EntityHeader.IsNullOrEmpty(solution.DefaultListener))
            {
                return InvokeResult<ListenerConfiguration>.FromError("Solution that this instance belongs does not have a default listener associated with it.");
            }

            var listenerConfiguration = await _pipelineModuleManager.GetListenerConfigurationAsync(solution.DefaultListener.Id, org, user);
            if (listenerConfiguration.ListenerType.Value == ListenerTypes.MQTTListener ||
                listenerConfiguration.ListenerType.Value == ListenerTypes.Rest ||
                listenerConfiguration.ListenerType.Value == ListenerTypes.FTP)
            {
                listenerConfiguration.HostName = instance.Result.DnsHostName;
            }

            Console.WriteLine($"Got listener configuration: {sw.ElapsedMilliseconds}ms");

            if (!String.IsNullOrEmpty(listenerConfiguration.SecureAccessKeyId))
            {
                var getSecretResult = await _secureStorage.GetSecretAsync(org, listenerConfiguration.SecureAccessKeyId, user);
                if (!getSecretResult.Successful)
                {
                    return InvokeResult<ListenerConfiguration>.FromInvokeResult(getSecretResult.ToInvokeResult());
                }

                listenerConfiguration.AccessKey = getSecretResult.Result;
            }

            Console.WriteLine($"Got access key: {sw.ElapsedMilliseconds}ms");

            if (!String.IsNullOrEmpty(listenerConfiguration.SecurePasswordId))
            {
                var getSecretResult = await _secureStorage.GetSecretAsync(org, listenerConfiguration.SecurePasswordId, user);
                if (!getSecretResult.Successful)
                {
                    return InvokeResult<ListenerConfiguration>.FromInvokeResult(getSecretResult.ToInvokeResult());
                }

                listenerConfiguration.Password = getSecretResult.Result;
            }

            Console.WriteLine($"Got password: {sw.ElapsedMilliseconds}ms");

            return InvokeResult<ListenerConfiguration>.Create(listenerConfiguration);
        }

        public async Task<ListResponse<DeviceTypeSummary>> GetDeviceTypesForInstanceAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var deviceTypes = new List<DeviceTypeSummary>();

            var instance = await GetInstanceAsync(instanceId, org, user);
            if (instance == null)
            {
                throw new RecordNotFoundException(nameof(DeploymentInstance), instanceId);
            }

            var solution = await _solutionManager.GetSolutionAsync(instance.Result.Solution.Id, org, user);
            if (solution == null)
            {
                throw new RecordNotFoundException(nameof(Solution), instance.Result.Solution.Id);
            }

            foreach (var deviceConfig in solution.DeviceConfigurations)
            {
                var deviceConfigTypes = await _deviceTypeManager.GetDeviceTypesForDeviceConfigOrgAsync(deviceConfig.Id, new ListRequest() { PageSize = 1000 }, org, user);
                deviceTypes.AddRange(deviceConfigTypes.Model);
            }

            return ListResponse<DeviceTypeSummary>.Create(deviceTypes.OrderBy(dt=>dt.Name));
        }

        public async Task<InvokeResult<InstanceAccount>> CreateInstanceAccountAsync(string instanceId, string userName, EntityHeader org, EntityHeader user)
        {
            var reEx = new Regex("^[a-z0-9]{3,30}$");
            if (!reEx.Match(userName).Success)
            {
                return InvokeResult<InstanceAccount>.FromError("The key must use only letters and numbers, and must be lowercase and be a minimum of 3 and a maximum of 30 characters");
            }

            var exists = await _instanceAccountRepo.DoesUserNameExistsAsync(instanceId, userName);
            if (exists)
            {
                return InvokeResult<InstanceAccount>.FromError($"A user name with {userName} already exists for this instance, please select a unique user id.");
            }

            // For security.
            var instance = await GetInstanceAsync(instanceId, org, user);
            if (instance == null)
            {
                throw new RecordNotFoundException(nameof(DeploymentInstance), instanceId);
            }

            var orgNamespace = await _orgUtils.GetOrgNamespaceAsync(org.Id);
            if (!orgNamespace.Successful)
            {
                return InvokeResult<InstanceAccount>.FromError($"Could not find organization name space for organization: {org.Id}.");
            }

            var account = new InstanceAccount()
            {
                UserName = $"{orgNamespace.Result}.{instance.Result.Key}.{userName}",
                AccessKey1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                AccessKey2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            };

            foreach (var srvr in instance.Result.ServiceHosts)
                await _remoteListenerServiceManager.AddInstanceAccountAsync(org.Id, srvr.HostId, instanceId, account);

            await _instanceAccountRepo.AddInstanceAccountAsync(instanceId, account);
            account.AccessKeyHash1 = null;
            account.AccessKeyHash2 = null;
            return InvokeResult<InstanceAccount>.Create(account);
        }

        public async Task<InvokeResult<InstanceAccount>> RegneerateInstanceAccountKeyAsync(string instanceId, string userName, string keyName, EntityHeader org, EntityHeader user)
        {

            var instance = await GetInstanceAsync(instanceId, org, user);
            if (instance == null)
            {
                throw new RecordNotFoundException(nameof(DeploymentInstance), instanceId);
            }

            var account = await _instanceAccountRepo.GetInstanceAccountAsync(instanceId, userName);

            var newKey = Guid.NewGuid().ToByteArray();
            var accessKey = Convert.ToBase64String(newKey);
            switch (keyName)
            {
                case "key1": account.AccessKey1 = accessKey; break;
                case "key2": account.AccessKey2 = accessKey; break;
                default: return InvokeResult<InstanceAccount>.FromError("Key index must be key1 or key.");
            }

            await _instanceAccountRepo.UpdateInstanceAccountAsync(instanceId, account);

            foreach (var srvr in instance.Result.ServiceHosts)
                await _remoteListenerServiceManager.UpdateInstanceAccountAsync(org.Id, srvr.HostId, instanceId, account);

            switch (keyName)
            {
                case "key1": account.AccessKey1 = accessKey; break;
                case "key2": account.AccessKey2 = accessKey; break;
            }

            account.AccessKeyHash1 = null;
            account.AccessKeyHash2 = null;

            return InvokeResult<InstanceAccount>.Create(account);
        }

        public async Task<InvokeResult> RemoveInstanceAccountAsync(string instanceId, string instanceAccountId, EntityHeader org, EntityHeader user)
        {
            // For security.
            var instance = await GetInstanceAsync(instanceId, org, user);
            if (instance == null)
            {
                throw new RecordNotFoundException(nameof(DeploymentInstance), instanceId);
            }

            await _instanceAccountRepo.RemoveInstanceAccountAsync(instanceId, instanceAccountId);

            return InvokeResult.Success;
        }

        public Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            return _instanceAccountRepo.GetInstanceAccountsAsync(instanceId);
        }

        public async Task<InvokeResult<WiFiConnectionProfile>> GetWiFiConnectionProfileAsync(string instanceId, string profileId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user);

            var profile = instance.Result.WiFiConnectionProfiles.SingleOrDefault(wcp => wcp.Id == profileId);

            var getSecretResult = await _secureStorage.GetSecretAsync(org, profile.PasswordSecretId, user);
            if (!getSecretResult.Successful) return InvokeResult<WiFiConnectionProfile>.FromError(getSecretResult.Errors.First().ToString());
            profile.Password = getSecretResult.Result;

            return InvokeResult<WiFiConnectionProfile>.Create(profile);
        }

        public async Task<List<WiFiConnectionProfile>> GetWiFiConnectionProfilesByDeviceRepoAsync(string repoId, EntityHeader org, EntityHeader user)
        {
            var repo = await _deviceRepoManager.GetDeviceRepositoryAsync(repoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance)) throw new InvalidOperationException($"no attached instance for {repo.Name} repository.");

            var result = await GetInstanceAsync(repo.Instance.Id, org, user);
            var profiles = new List<WiFiConnectionProfile>();

            foreach (var profile in result.Result.WiFiConnectionProfiles)
            {
                var getSecretResult = await _secureStorage.GetSecretAsync(org, profile.PasswordSecretId, user);
                profile.Password = getSecretResult.Result;
                profiles.Add(profile);
            }

            return profiles;
        }

        public async Task<InvokeResult<InstanceService>> AllocateInstanceServiceAsync(string instanceId, HostTypes hostType, bool replaceExisting, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            var host = await _hostManager.FindHostServiceAsync(hostType);
            if (host == null)
            {
                return InvokeResult<InstanceService>.FromError($"Could not find host of type {hostType} to allocate.");
            }

            var instance = await GetInstanceAsync(instanceId);

            var instanceService = instance.Result.ServiceHosts.FirstOrDefault(hst => hst.HostType.Value == hostType);
            if (instanceService != null && replaceExisting)
            {
                instance.Result.ServiceHosts.Remove(instanceService);
                instanceService = null;
            }

            if (instanceService == null)
            {
                var orgNameSpaceResult = await _orgUtils.GetOrgNamespaceAsync(orgEntityHeader.Id);
                if (!orgNameSpaceResult.Successful)
                {
                    return InvokeResult<InstanceService>.FromError($"Coould not determine org namespace from {orgEntityHeader.Id}");
                }

                instanceService = new InstanceService()
                {
                    Id = Guid.NewGuid().ToId(),
                    HostType = EntityHeader<HostTypes>.Create(hostType),
                    HostId = host.Id,
                    OwnerOrg = host.OwnerOrganization,
                    AllocatedTimeStamp = DateTime.UtcNow.ToJSONString(),
                    Url = host.DnsHostName,
                    ServiceAccount = $"{orgNameSpaceResult.Result}.{instance.Result.Key}.{hostType.ToString().ToLower()}",
                    ServiceAccountPassword = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                };

                var addSecretResult = await _secureStorage.AddSecretAsync(orgEntityHeader, instanceService.ServiceAccountPassword);
                if (!addSecretResult.Successful)
                {
                    return InvokeResult<InstanceService>.FromError($"Coould not determine org namespace from {orgEntityHeader.Id}");
                }

                instanceService.ServiceAccountSecretId = addSecretResult.Result;

                // need to have a copy of this w/o the password to save with the instance, the password will be returned to the caller
                // so it can be used to authenticate any remote clients to this service.
                var instanceServiceCopy = new InstanceService()
                {
                    Id = instanceService.Id,
                    Url = instanceService.Url,
                    AllocatedTimeStamp = instanceService.AllocatedTimeStamp,
                    HostId = instanceService.HostId,
                    HostType = instanceService.HostType,
                    OwnerOrg = instanceService.OwnerOrg,
                    ServiceAccount = instanceService.ServiceAccount,
                    ServiceAccountSecretId = instanceService.ServiceAccountSecretId,
                };

                instance.Result.ServiceHosts.Add(instanceServiceCopy);
                await UpdateInstanceAsync(instance.Result, orgEntityHeader, userEntityHeader);
            }
            else
            {
                var pwdResult = await _secureStorage.GetSecretAsync(orgEntityHeader, instanceService.ServiceAccountSecretId, userEntityHeader);
                if(!pwdResult.Successful)
                {
                    return InvokeResult<InstanceService>.FromError($"Coould not find secret from secret id.");
                }

                instanceService.ServiceAccountPassword = pwdResult.Result;
            }



            if (hostType == HostTypes.MultiTenantMQTT)
            {
                var accounts = await _instanceAccountRepo.GetInstanceAccountsAsync(instanceId);
                var result = await _remoteListenerServiceManager.ProvisionInstanceAsync(orgEntityHeader.Id, instanceService.HostId, instanceId, instanceService, accounts);
                if (!result.Successful)
                    return InvokeResult<InstanceService>.FromInvokeResult(result);
            }

            return InvokeResult<InstanceService>.Create(instanceService);
        }

        public async Task<InvokeResult> RemoveInstanceServiceAsync(string instanceId, string instanceServiceId, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            var instance = await GetInstanceAsync(instanceId);
            var existing = instance.Result.ServiceHosts.FirstOrDefault(hst => hst.Id == instanceServiceId);
            if (existing == null)
                return InvokeResult.FromError($"Could not find existing instance id with id {instanceServiceId}.");

            instance.Result.ServiceHosts.Remove(existing);

            await UpdateInstanceAsync(instance.Result, orgEntityHeader, userEntityHeader);

            if (existing.HostType.Value == HostTypes.MultiTenantMQTT)
            {
                var result = await _remoteListenerServiceManager.RemoveInstanceAsync(orgEntityHeader.Id, existing.HostId, instanceId);
                if (!result.Successful)
                    return result;
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SetContainerRepoAsync(string instanceId, string containerRepoId, string tagId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId);
            await AuthorizeAsync(user, org, "setcontainerimage", instanceId);

            var image = await _containerRepoMgr.GetContainerRepoAsync(containerRepoId, org, user);
            var tag = image.Tags.SingleOrDefault(tg => tg.Id == tagId);
            if (tag == null)
                throw new RecordNotFoundException("ImageTag", tagId);

            instance.Result.ContainerRepository = image.ToEntityHeader();
            instance.Result.ContainerTag = tag.ToEntityHeader();
            await UpdateInstanceAsync(instance.Result, org, user);
            return await UpdateRuntimeAsync(instanceId, org, user);
        }
    }
}
