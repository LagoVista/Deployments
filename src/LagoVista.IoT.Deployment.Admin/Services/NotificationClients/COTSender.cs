using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceMessaging.Models.Cot;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.MediaServices.Interfaces;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class COTSender : ICOTSender
    {
        private readonly IMediaServicesManager _mediaSerivces;
        private readonly IAdminLogger _admminLogger;
        private readonly ITagReplacementService _tagReplacer;

        public COTSender(IMediaServicesManager mediaSerivces,  IAdminLogger adminLogger, ITagReplacementService tagReplacer)
        {
            _mediaSerivces = mediaSerivces ?? throw new ArgumentNullException(nameof(mediaSerivces));
            _admminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
        }

        public async Task<InvokeResult> SendAsync(CursorOnTarget cot, Device device, OrgLocation location, EntityHeader org, EntityHeader user)
        {
           var package = await _mediaSerivces.GetResourceMediaAsync(cot.DataPackageFile.Id, org, user);

            var client = new TakClient(_admminLogger);

            using(var ms = new MemoryStream(package.ImageBytes))
            {
                var initResponse = await client.InitAsync(ms);
                if (!initResponse.Successful) return initResponse;

                var msg = new Event();
                msg.Type = cot.NotificationType;

                var connectResponse = await client.ConnectAsync();
                if (!connectResponse.Successful) return connectResponse;

                var sendResponse = await client.SendAsync(new Message() { Event = msg });
                if (!sendResponse.Successful) return sendResponse;

                var disconnectResponse = await client.DisconnectAsync();
                if (!disconnectResponse.Successful) return disconnectResponse;

                return InvokeResult.Success;
             }
        }
    }
}
