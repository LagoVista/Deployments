using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using LagoVista.Core;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    class RemoteConfigurationManager : ManagerBase, IRemoteConfigurationManager
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly IFirmwareManager _firmwareManager;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IAdminLogger _adminLogger;

        public RemoteConfigurationManager(IAdminLogger logger, IAppConfig appConfig,
            IFirmwareManager firmwareManager, IDependencyManager depmanager, IDeviceRepositoryManager repoManager, ISecurity security,
            IProxyFactory proxyFactory) : base(logger, appConfig, depmanager, security)
        {

            _adminLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _firmwareManager = firmwareManager ?? throw new ArgumentNullException(nameof(firmwareManager));
        }

        public async Task<InvokeResult> RestartDeviceAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            return await propertyManager.RestartDeviceAsync(deviceUniqueId);
        }

        public async Task<InvokeResult> QueryRemoteConfigurationAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            return await propertyManager.QueryRemoteConfigurationAsync(deviceUniqueId);
        }

        public async Task<InvokeResult> SendAllPropertiesAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            return await propertyManager.SendAllPropertiesAsync(deviceUniqueId);
        }

        public async Task<InvokeResult> SendPropertyAsync(string deviceRepoId, string deviceUniqueId, int propertyIndex, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            return await propertyManager.SendPropertyAsync(deviceUniqueId, propertyIndex);
        }


        public async Task<InvokeResult<string>> ApplyFirmwareAsync(string deviceRepoId, string deviceUniqueId, string firmwareId, string firmwareRevisionId, bool triggerRemotely, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(deviceRepoId)) throw new ArgumentNullException(deviceRepoId);
            if (String.IsNullOrEmpty(deviceUniqueId)) throw new ArgumentNullException(deviceUniqueId);
            if (String.IsNullOrEmpty(firmwareId)) throw new ArgumentNullException(firmwareId);
            if (String.IsNullOrEmpty(firmwareRevisionId)) throw new ArgumentNullException(firmwareRevisionId);

            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (repo == null) throw new RecordNotFoundException(nameof(DeviceRepository), deviceRepoId);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult<string>.FromError("Instance not deployed, can not set property.");
            }

            var firmwareRequest = await _firmwareManager.RequestDownloadLinkAsync(deviceRepoId, deviceUniqueId, firmwareId, firmwareRevisionId, org, user);
            if (!firmwareRequest.Successful)
            {
                return InvokeResult<string>.FromInvokeResult(firmwareRequest.ToInvokeResult());
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            // this will send a message to the remote instance to trigger an update to the device via MQTT
            // or an future HTTP request.  If this is not set, the calling method will be responsible for
            // kicking off the download.
            if (triggerRemotely)
            {
                var result = await propertyManager.SetFirmwareVersionAsync(deviceUniqueId, firmwareRequest.Result.DownloadId);
                if (result.Successful)
                    return InvokeResult<string>.Create(firmwareRequest.Result.DownloadId);
                else
                {

                }
                return InvokeResult<string>.FromInvokeResult(result);
            }
            else
                return InvokeResult<string>.Create(firmwareRequest.Result.DownloadId);
        }

        public async Task<InvokeResult> SendCommandAsync(string deviceRepoId, string deviceUniqueId, string commandId, List<KeyValuePair<string, string>> parameters, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            var extras = new List<KeyValuePair<string, string>>
                {
                    deviceUniqueId.ToKVP("deviceUniqueId"),
                    repo.Instance.Id.ToKVP("instanceId"),
                    repo.OwnerOrganization.Id.ToKVP("orgId"),
                    commandId.ToKVP("command")
                };

            foreach (var prm in parameters)
                extras.Add(prm);

            var result =  await propertyManager.SendCommandAsync(deviceUniqueId, commandId, parameters);
            if(result.Successful)
            {
                _adminLogger.Trace("[RemoteConfigurationManager__SendCommandAsync] Success Send Command:", extras.ToArray());
            }
            else
            {
                _adminLogger.AddError("[RemoteConfigurationManager__SendCommandAsync]","[RemoteConfigurationManager__SendCommandAsync] Success Send Command:", extras.ToArray());
            }
            return result;
        }

        public async Task<InvokeResult> SetDesiredConfigurationRevisionAsync(string deviceRepoId, string deviceUniqueId, int configurationRevision, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryAsync(deviceRepoId, org, user);
            if (EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                return InvokeResult.FromError("Instance not deployed, can not set property.");
            }

            var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
            {
                InstanceId = repo.Instance.Id,
                OrganizationId = repo.OwnerOrganization.Id
            });

            return await propertyManager.SetDesiredConfigurationRevisionAsync(deviceUniqueId, configurationRevision);
        }
    }
}
