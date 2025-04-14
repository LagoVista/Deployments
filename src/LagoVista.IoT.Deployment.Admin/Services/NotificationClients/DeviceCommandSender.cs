using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class DeviceCommandSender : IDeviceCommandSender
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly IAdminLogger _adminLogger;
        private readonly ITagReplacementService _tagReplacer;
        private readonly IBackgroundServiceTaskQueue _backgroundServiceTaskQueue;

        private string _command;
        private string _body;

        public DeviceCommandSender(IProxyFactory proxyFactory, IAdminLogger adminLogger, IBackgroundServiceTaskQueue backgroundServiceTaskQueue, ITagReplacementService tagReplacer)
        {

            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _backgroundServiceTaskQueue = backgroundServiceTaskQueue ?? throw new ArgumentNullException(nameof(backgroundServiceTaskQueue));
        }

        public async Task<InvokeResult> PrepareMessage(DeviceNotification notification, bool testMode, Device device, OrgLocation location)
        {
            _command = notification.Key;

            if (!String.IsNullOrEmpty(notification.ForwardToParentDeviceBody))
                _body = await _tagReplacer.ReplaceTagsAsync(notification.ForwardToParentDeviceBody, false, device, location);
            else if(!String.IsNullOrEmpty(notification.SmsContent))
                _body = await _tagReplacer.ReplaceTagsAsync(notification.ForwardToParentDeviceBody, false, device, location);
            else
                _body = "[no content]";

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SendAsync(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            await _backgroundServiceTaskQueue.QueueBackgroundWorkItemAsync(async (token) =>
            {
                var propertyManager = _proxyFactory.Create<IRemotePropertyNamanager>(new ProxySettings()
                {
                    InstanceId = instanceId,
                    OrganizationId = org.Id
                });

                var result = await propertyManager.SendCommandAsync(id, _command, new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>()
                        { new System.Collections.Generic.KeyValuePair<string, string>("body", _body) });

                if (result.Successful)
                    _adminLogger.Trace("[DeviceCommandSender__SendAsync] Success Send Command");
                else
                {
                    _adminLogger.Trace("[DeviceCommandSender__SendAsync] Failed Send Command");
                    _adminLogger.AddError("[DeviceCommandSender__SendAsync]", result.ErrorMessage);
                }
            });

            _adminLogger.Trace("[DeviceCommandSender__SendAsync] Queued send command");

            return InvokeResult.Success;
        }
    }
}
