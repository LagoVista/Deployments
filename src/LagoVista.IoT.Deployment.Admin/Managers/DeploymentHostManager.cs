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
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.Core.Exceptions;
using System.Runtime.CompilerServices;
using System.Linq;

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
        IUserManager _userManager;
        private readonly IAdminLogger _adminLogger;
        private readonly ISecureStorage _secureStorage;

        public DeploymentHostManager(IDeploymentHostRepo deploymentHostRepo, IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentActivityRepo deploymentActivityRepo,
                IDeploymentConnectorService deploymentConnectorService, ISecureStorage secureStorage, IDeploymentInstanceRepo deploymentInstanceRepo, IAdminLogger logger, IDeploymentHostStatusRepo deploymentHostStatusRepo,
                IUserManager userManager, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _adminLogger = logger;
            _deploymentHostRepo = deploymentHostRepo;
            _deploymentInstanceRepo = deploymentInstanceRepo;
            _deploymentConnectorService = deploymentConnectorService;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _deploymentActivityRepo = deploymentActivityRepo;
            _deploymentHostStatusRepo = deploymentHostStatusRepo;
            _secureStorage = secureStorage;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Create);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Create, user, org);

            var res1 = await _secureStorage.AddSecretAsync(org, host.HostAccessKey1);
            if (!res1.Successful)
                return res1.ToInvokeResult();

            host.HostAccessKey1SecretId = res1.Result;

            var res2 = await _secureStorage.AddSecretAsync(org, host.HostAccessKey2);
            if (!res2.Successful)
                return res2.ToInvokeResult();

            host.HostAccessKey2SecretId = res2.Result;

            await _deploymentHostRepo.AddDeploymentHostAsync(host);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteDeploymentHostAsync(String hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            if (host.HostType.Value == HostTypes.MCP) return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CanNotDeleteMCPHost.ToErrorMessage());
            if (host.HostType.Value == HostTypes.Notification) return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CanNotDeleteNotificationServerHost.ToErrorMessage());

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Delete, user, org);
            await ConfirmNoDepenenciesAsync(host);
            host.IsArchived = true;
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }

        public async Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await GetDeploymentHostAsync(hostId, org, user);
            await AuthorizeAsync(user, org, "hostDeployedInstances", hostId);
            return await _deploymentConnectorService.GetDeployedInstancesAsync(host, org, user);
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

        public async Task<DeploymentHost> GetDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId, throwOnNotFound);
            if (host == null)
            {
                return host;
            }

            await CheckOwnershipOrSysAdminAsync(host, org, user);
      
            return host;
        }

        public async Task<DeploymentHost> GetDeploymentHostAsync(string hostId)
        {
            return await _deploymentHostRepo.GetDeploymentHostAsync(hostId);            
        }

        public async Task<DeploymentHost> GetSecureDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId, throwOnNotFound);
            if (host == null)
            {
                return host;
            }

            if (String.IsNullOrEmpty(host.HostAccessKey1) && !String.IsNullOrEmpty(host.HostAccessKey1SecretId))
            {
                var res1 = await _secureStorage.GetSecretAsync(org, host.HostAccessKey1SecretId, user);
                host.HostAccessKey1 = res1.Result;
            }

            if (string.IsNullOrEmpty(host.HostAccessKey2) && !String.IsNullOrEmpty(host.HostAccessKey2SecretId))
            {
                var res2 = await _secureStorage.GetSecretAsync(org, host.HostAccessKey2SecretId, user);
                host.HostAccessKey2 = res2.Result;
            }

            await CheckOwnershipOrSysAdminAsync(host, org, user);

            return host;
        }

        public async Task<InvokeResult> DeployHostAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await CheckOwnershipOrSysAdminAsync(host, org, user);

            if (host.Status.Value != HostStatus.Offline)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CantStartNotStopped.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, hostId, DeploymentActivityTaskTypes.DeployHost)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = host.OwnerOrganization.Id,
                RequestedByOrganizationName = host.OwnerOrganization.Id,
            });

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeployContainerAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
            await CheckOwnershipOrSysAdminAsync(host, org, user);

            if (host.Status.Value != HostStatus.Running)
            {
                return InvokeResult.FromErrors(Resources.DeploymentErrorCodes.CannotDeployContainerToNonRunningHost.ToErrorMessage());
            }

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.InstanceContainer, hostId, DeploymentActivityTaskTypes.DeployContainer)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = host.OwnerOrganization.Id,
                RequestedByOrganizationName = host.OwnerOrganization.Id,
            });

            return InvokeResult.Success;
        }


        public async Task<InvokeResult> RestartHostAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(hostId);
          
            await CheckOwnershipOrSysAdminAsync(host, org, user);

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, hostId, DeploymentActivityTaskTypes.RestartHost)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = host.OwnerOrganization.Id,
                RequestedByOrganizationName = host.OwnerOrganization.Id,
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
            await CheckOwnershipOrSysAdminAsync(host, org, user);

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Server, instanceId, DeploymentActivityTaskTypes.DestroyHost)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = host.OwnerOrganization.Id,
                RequestedByOrganizationName = host.OwnerOrganization.Id,
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
            return _deploymentHostRepo.QueryHostKeyInUse(key, org.Id);
        }

        protected string GenerateRandomKey(byte len = 64)
        {
            var buffer = new byte[len];
            new Random().NextBytes(buffer);
            return Convert.ToBase64String(buffer);
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

            var instance = await GetDeploymentHostAsync(instanceId, org, user);

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
                        if (!String.IsNullOrEmpty(instance.HostAccessKey1SecretId))
                        {
                            //If this fails don't fail the entire method.
                            var removeKeyResult = await _secureStorage.RemoveSecretAsync(org, instance.HostAccessKey1SecretId);
                            if (!removeKeyResult.Successful)
                            {
                                _adminLogger.AddError("DeploymentInstanceManager_RegenerateKeyAsync_Remove", removeKeyResult.Errors.First().Message);
                            }
                        }
                        instance.HostAccessKey1SecretId = keyAddResult.Result;
                    }
                    break;
                case "key2":
                    {
                        if (!String.IsNullOrEmpty(instance.HostAccessKey2SecretId))
                        {
                            //If this fails don't fail the entire method.
                            var removeKeyResult = await _secureStorage.RemoveSecretAsync(org, instance.HostAccessKey2SecretId);
                            if (!removeKeyResult.Successful)
                            {
                                _adminLogger.AddError("DeploymentInstanceManager_RegenerateKeyAsync_Remove", removeKeyResult.Errors.First().Message);
                            }
                        }
                        instance.HostAccessKey2SecretId = keyAddResult.Result;
                    }
                    break;
                default:
                    throw new Exception($"Invalid key name: {keyName}");
            }

            instance.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            instance.LastUpdatedBy = user;

            await _deploymentHostRepo.UpdateDeploymentHostAsync(instance);

            return InvokeResult<string>.Create(key);
        }

        public async Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);

            var existingHost = await _deploymentHostRepo.GetDeploymentHostAsync(host.Id);

            if (host.Status.Value != existingHost.Status.Value)
            {
                await UpdateDeploymentHostStatusAsync(host.Id, host.Status.Value, host.ContainerTag.Text, org, user);
            }

            if(host.DnsHostName != existingHost.DnsHostName && host.HostType.Value == HostTypes.Shared)
            {
                if(string.IsNullOrEmpty(host.DnsHostName))
                {
                    return InvokeResult.FromError("DNS Host Name is a Required Field");
                }

                foreach(var instanceSummary in existingHost.DeployedInstances)
                {
                    var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceSummary.Instance.Id);
                    instanceSummary.DnsHostName = instanceSummary.DnsHostName.ToLower().Replace(existingHost.DnsHostName.ToLower(), host.DnsHostName.ToLower());
                    await _deploymentInstanceRepo.UpdateInstanceAsync(instance);
                }
            }
            
            await CheckOwnershipOrSysAdminAsync(host, org, user);

            if (!String.IsNullOrEmpty(host.HostAccessKey1))
            {
                if (!String.IsNullOrEmpty(host.HostAccessKey1SecretId))
                    await _secureStorage.RemoveSecretAsync(org, host.HostAccessKey1SecretId);
              
                var res1 = await _secureStorage.AddSecretAsync(org, host.HostAccessKey1);
                if (!res1.Successful)
                    return res1.ToInvokeResult();

                host.HostAccessKey1SecretId = res1.Result;
                host.HostAccessKey1 = null;
            }

            if (!String.IsNullOrEmpty(host.HostAccessKey2))
            {
                if (!String.IsNullOrEmpty(host.HostAccessKey2SecretId))
                    await _secureStorage.RemoveSecretAsync(org, host.HostAccessKey2SecretId);
                
                var res2 = await _secureStorage.AddSecretAsync(org, host.HostAccessKey2);
                if (!res2.Successful)
                    return res2.ToInvokeResult();

                host.HostAccessKey2SecretId = res2.Result;
                host.HostAccessKey2 = null;
            }

            host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            host.LastUpdatedBy = user;
            
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeploymentHostStatusAsync(string hostId, HostStatus hostStatus, string version, EntityHeader org, EntityHeader user, string statusDetails = "")
        {
            var host = await GetDeploymentHostAsync(hostId);

            if (host.Status.Value != hostStatus)
            {
                await CheckOwnershipOrSysAdminAsync(host, org, user);

                var hostStatusUpdate = Models.DeploymentHostStatus.Create(hostId, user);
                hostStatusUpdate.OldState = host.Status.Value.ToString();
                hostStatusUpdate.NewState = hostStatus.ToString();
                hostStatusUpdate.Details = statusDetails;
                hostStatusUpdate.Version = version ?? "-";

                host.Status = EntityHeader<HostStatus>.Create(hostStatus);
                host.StatusTimeStamp = DateTime.UtcNow.ToJSONString();
                host.StatusDetails = statusDetails;
                await _deploymentHostStatusRepo.AddDeploymentHostStatusAsync(hostStatusUpdate);

                host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                host.LastUpdatedBy = user;

                await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            }

            return InvokeResult.Success;
        }

        public async Task<ListResponse<DeploymentHostStatus>> GetDeploymentHostStatusHistoryAsync(string hostId, EntityHeader org, EntityHeader user, ListRequest listRequest)
        {
            var host = await GetDeploymentHostAsync(hostId, org, user);
            await CheckOwnershipOrSysAdminAsync(host, org, user);
            return await _deploymentHostStatusRepo.GetStatusHistoryForHostAsync(hostId, listRequest);
        }

        public Task<DeploymentHost> FindHostServiceAsync(HostTypes hostType)
        {
            return _deploymentHostRepo.FindSharedHostAsync(hostType);
        }
    }
}
