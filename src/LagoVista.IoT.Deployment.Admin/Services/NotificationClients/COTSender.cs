using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceMessaging.Models.Cot;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.MediaServices.Interfaces;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class COTSender : ICOTSender
    {
        private readonly IMediaServicesManager _mediaSerivces;
        private readonly IAdminLogger _admminLogger;
        private readonly ISecureStorage _secureStorage;
        private readonly ITagReplacementService _tagReplacer;
        private readonly IOrganizationManager _orgManager;

        public COTSender(IMediaServicesManager mediaSerivces, ISecureStorage secureStorage, IAdminLogger adminLogger, ITagReplacementService tagReplacer, IOrganizationManager orgManager)
        {
            _mediaSerivces = mediaSerivces ?? throw new ArgumentNullException(nameof(mediaSerivces));
            _admminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager)); 
        }

        public async Task<InvokeResult> SendAsync(CursorOnTarget cot, Device device, OrgLocation location, EntityHeader org, EntityHeader user)
        {
            var package = await _mediaSerivces.GetResourceMediaAsync(cot.DataPackageFile.Id, org, user);
            var client = new TakClient(_admminLogger);
            client.IgnoreCertificateErrors = cot.IgnoreCertificateErrors;
            using (var ms = new MemoryStream(package.ImageBytes))
            {
                var initResponse = await client.InitAsync(ms);
                if (!initResponse.Successful) return initResponse;

                var msg = new Event();
                msg.Type = cot.NotificationType;
                



                if (cot.UseCustomCertificate)
                {
                    var cert = await _mediaSerivces.GetResourceMediaAsync(cot.CustomRootCert.Id, org, user);
                    var connectResponse = await client.ConnectAsync(cert.ImageBytes);
                    if (!connectResponse.Successful) return connectResponse;
                }
                else
                {
                    var connectResponse = await client.ConnectAsync();
                    if (!connectResponse.Successful) return connectResponse;
                }

                var sendResponse = await client.SendAsync(new Message() { Event = msg });
                if (!sendResponse.Successful) return sendResponse;

                var disconnectResponse = await client.DisconnectAsync();
                if (!disconnectResponse.Successful) return disconnectResponse;

                return InvokeResult.Success;
            }
        }
    }
}
