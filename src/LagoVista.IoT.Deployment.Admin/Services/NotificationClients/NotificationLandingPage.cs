// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cc1b1630968e3290a6e331e4c0d6ded84d2a51d7a0fbe1482cee7c659aff20f2
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationLandingPage : INotificationLandingPage
    {
        private readonly ITagReplacementService _tagReplacer;
        private readonly ILogger _logger;
        private readonly IAppConfig _appConfig;
        private readonly IStaticPageStorage _staticPageStorage;

        public NotificationLandingPage(IAdminLogger logger, IAppConfig appConfig, ITagReplacementService tagReplacer, IStaticPageStorage staticPageStorage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _staticPageStorage = staticPageStorage ?? throw new ArgumentNullException(nameof(staticPageStorage));
        }

        public async Task<InvokeResult<NotificationLinks>> PreparePage(string raisedNotificationId, string errorId, DeviceNotification notification, bool testMode, Device device, OrgLocation location, EntityHeader org, EntityHeader user)
        {
            var links = new NotificationLinks();

            if (notification.IncludeLandingPage)
            {
                _logger.Trace("[NotificationSender__RaiseNotificationAsync] - Including Landindg page");
                var template = await _tagReplacer.ReplaceTagsAsync(notification.LandingPageContent, true, device, location);
                if (testMode)
                    template = $"<h1>TESTING - TESTING</h1> {template}";

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Tags replaced in template");

                var storageResult = await _staticPageStorage.StorePageAsync(org.Id, "devnotif", template);
                if (!storageResult.Successful) return InvokeResult<NotificationLinks>.FromInvokeResult(storageResult.ToInvokeResult());

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Static page stored page id: {storageResult.Result}");

                links.PageId = storageResult.Result;
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                links.FullLandingPageLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotificationId}/{org.Id}/[RecipientId]/{links.PageId}";
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Including Landindg page - {links.FullLandingPageLink}");
            }
            else
            {
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                links.AcknowledgeLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotificationId}/{org.Id}/[RecipientId]/acknowledge";
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No Landindg page, just acknowledge link - {links.AcknowledgeLink}");
            }

            if (String.IsNullOrEmpty(errorId))
                links.SilenceLink = $"{_appConfig.WebAddress}/devicemgmt/device/{device.Id}/[NotificationHistoryId]/silence";
            else
            {
                links.SilenceLink = $"{_appConfig.WebAddress}/devicemgmt/device/{device.Id}/[NotificationHistoryId]/errors/{errorId}/silence";
                links.ClearErrorLink = $"{_appConfig.WebAddress}/devicemgmt/device/{device.Id}/[NotificationHistoryId]/errors/{errorId}/clear";
            }
            return InvokeResult<NotificationLinks>.Create(links);
        }

    }
}
