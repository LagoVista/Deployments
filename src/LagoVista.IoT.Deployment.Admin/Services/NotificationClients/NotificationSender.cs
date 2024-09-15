using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using System.Linq;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using LagoVista.UserAdmin.Models.Users;
using RingCentral;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationSender : INotificationSender
    {
        private readonly ILogger _logger;
        private readonly Interfaces.IEmailSender _emailSender;
        private readonly ISMSSender _smsSender;
        private readonly IDeviceNotificationTracking _notificationTracking;
        private readonly IDistributionListRepo _distroListRepo;
        private readonly IOrgLocationRepo _orgLocationRepo;
        private readonly LagoVista.IoT.DeviceManagement.Core.IDeviceManager _deviceManager;
        private readonly IStaticPageStorage _staticPageStorage;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IAppConfig _appConfig;
        private readonly IDeploymentInstanceRepo _deploymentRepo;
        private readonly ILinkShortener _linkShortener;
        private readonly IAppUserRepo _appUserRepo;
        private readonly IDeviceNotificationRepo _deviceNotificationRepo;
        private readonly ITagReplacementService _tagReplacementService;
        private readonly ICOTSender _cotSender;
        private readonly IMqttSender _mqttSender;
        private readonly IRestSender _restSender;
        private readonly INotificationLandingPage _landingPageBuilder;

        public NotificationSender(ILogger logger, IDistributionListRepo distroListRepo, IDeviceNotificationTracking notificationTracking, ITagReplacementService tagReplacementService, IDeviceNotificationRepo deviceNotificationRepo, IOrgLocationRepo orgLocationRepo, 
            LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, Interfaces.IEmailSender emailSender, ISMSSender smsSender, INotificationLandingPage landingPageBuilder,
                                  IAppUserRepo appUserRepo, IDeviceRepositoryManager repoManager, IStaticPageStorage staticPageStorage, ILinkShortener linkShortner, ICOTSender cotSender, IRestSender restSender, IMqttSender mqttSender,
                                  IDeploymentInstanceRepo deploymentRepo, IAppConfig appConfig)
        {
            _deviceNotificationRepo = deviceNotificationRepo ?? throw new ArgumentNullException(nameof(deviceNotificationRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _distroListRepo = distroListRepo ?? throw new ArgumentNullException(nameof(distroListRepo));
            _orgLocationRepo = orgLocationRepo ?? throw new ArgumentNullException(nameof(orgLocationRepo));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _linkShortener = linkShortner ?? throw new ArgumentNullException(nameof(linkShortner));
            _deploymentRepo = deploymentRepo ?? throw new ArgumentNullException(nameof(deploymentRepo));
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _staticPageStorage = staticPageStorage ?? throw new ArgumentNullException(nameof(staticPageStorage));
            _tagReplacementService = tagReplacementService ?? throw new ArgumentNullException(nameof(tagReplacementService));
            _notificationTracking = notificationTracking ?? throw new ArgumentNullException(nameof(notificationTracking));
            _restSender = restSender ?? throw new ArgumentNullException(nameof(restSender));
            _cotSender = cotSender ?? throw new ArgumentNullException(nameof(cotSender));
            _mqttSender = mqttSender ?? throw new ArgumentNullException(nameof(mqttSender));
            _landingPageBuilder = landingPageBuilder ?? throw new ArgumentNullException(nameof(landingPageBuilder));
        }

        private async Task<InvokeResult<List<NotificationRecipient>>> GetRecipientsAsync(RaisedDeviceNotification raisedNotification, Device device, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            var recipients = new List<NotificationRecipient>();

            var appUsers = new List<EntityHeader>();
            var externalContacts = new List<ExternalContact>();

            if (raisedNotification.AdditionalUsers != null)
                appUsers.AddRange(raisedNotification.AdditionalUsers);

            if (raisedNotification.AdditionalExternalContacts != null)
                externalContacts.AddRange(raisedNotification.AdditionalExternalContacts);

            if (!EntityHeader.IsNullOrEmpty(device.DistributionList))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(device.DistributionList.Id);
                if (!distroList.ExternalContacts.Any())
                {
                    _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Found distribution list, but no external contacts, will not attempt to send.");
                    return InvokeResult<List<NotificationRecipient>>.Create(recipients);
                }

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Distribution List {distroList.Name} with {distroList.ExternalContacts.Count} contacts");

                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);
            }
            else
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No distribution list on device, will not attempt to send.");
                return InvokeResult<List<NotificationRecipient>>.Create(recipients);
            }

            foreach (var appUser in appUsers)
            {
                if (!recipients.Any(rec => rec.Id == appUser.Id))
                {
                    var user = await _appUserRepo.FindByIdAsync(appUser.Id);
                    if (user == null)
                    {
                        _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendEmail__AppUser]",
                        $"Request to send notification to app user, but could not find app user with id: ({user.Id}) but email addess is empty");
                    }
                    else
                        recipients.Add(NotificationRecipient.FromAppUser(user));
                }
            }

            recipients.AddRange(externalContacts.Select(eu => NotificationRecipient.FromExternalContext(eu)));

            return InvokeResult<List<NotificationRecipient>>.Create(recipients);
        }

        private async Task<InvokeResult<Device>> GetDeviceAsync(RaisedDeviceNotification raisedNotification, DeviceRepository repo, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            if (!String.IsNullOrEmpty(raisedNotification.DeviceId))
                return await _deviceManager.GetDeviceByDeviceIdAsync(repo, raisedNotification.DeviceId, orgEntityHeader, userEntityHeader);
            else if (!string.IsNullOrEmpty(raisedNotification.DeviceUniqueId))
                return await _deviceManager.GetDeviceByIdAsync(repo, raisedNotification.DeviceUniqueId, orgEntityHeader, userEntityHeader);
            else
                return InvokeResult<Device>.FromError($"Must provide either DeviceId or DeviceUniqueId.");
        }

        public async Task<InvokeResult> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Starting - Test Mode {raisedNotification.TestMode}");

            var result = raisedNotification.Validate();
            if (!result.Successful)
            {
                _logger.AddException($"[NotificationSender__RaiseNotificationAsync]", new Exception($"Validation Error on RaiseDeviceNotification {result.ErrorMessage}"));
                return result;
            }

            var notification = await _deviceNotificationRepo.GetNotificationByKeyAsync(orgEntityHeader.Id, raisedNotification.NotificationKey);

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(raisedNotification.DeviceRepositoryId, orgEntityHeader, userEntityHeader);
            if (repo == null) return InvokeResult.FromError($"Could not locate device repository {raisedNotification.DeviceRepositoryId}");
            if (EntityHeader.IsNullOrEmpty(repo.Instance)) return InvokeResult.FromError($"Device does not have a deployment instance for device repository: ${repo.Name}");

            var deployment = await _deploymentRepo.GetInstanceAsync(repo.Instance.Id);
            if (deployment == null) return InvokeResult.FromError($"Could not locate deployment {repo.Instance.Text} - {repo.Instance.Id}");
            OrgLocation location = null;

            var device = await GetDeviceAsync(raisedNotification, repo, orgEntityHeader, userEntityHeader);           
            if (!device.Successful) return InvokeResult.FromError($"Could not locate device {(String.IsNullOrEmpty(raisedNotification.DeviceId) ? raisedNotification.DeviceId : raisedNotification.DeviceUniqueId)} in device repository {raisedNotification.DeviceRepositoryId}");
            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Sending notification {notification.Name} for device {device.Result.Name} in {repo.Name} repository");

            if (!EntityHeader.IsNullOrEmpty(device.Result.Location))
            {
                location = await _orgLocationRepo.GetLocationAsync(device.Result.Location.Id);
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - found location {location.Name} on device, can not append location information");
            }
            else
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No location set on device, can not append location information");

           var pageResult = await _landingPageBuilder.PreparePage(raisedNotification, notification, deployment.TestMode, device.Result, location, orgEntityHeader, userEntityHeader);

            var page = pageResult.Result;

            await _smsSender.PrepareMessage(notification, device.Result, location);
            await _emailSender.PrepareMessage(notification, deployment.TestMode, device.Result, location);

            var recipientsResult = await GetRecipientsAsync(raisedNotification, device.Result, orgEntityHeader, userEntityHeader);

            if (!recipientsResult.Successful)
                return result.ToInvokeResult();

            var recipients = recipientsResult.Result;

            foreach (var recipient in recipients)
            {
                await _smsSender.SendAsync(recipient, page, orgEntityHeader, userEntityHeader);
                await _emailSender.SendAsync(notification, recipient, page, orgEntityHeader, userEntityHeader);

                await _notificationTracking.AddHistoryAsync(new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-{recipient.Id}")
                {
                    UserId = recipient.Id,
                    UserName = $"{recipient.FirstName} {recipient.LastName}",
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    SentEmail = notification.SendEmail && recipient.SendEmail,
                    SentSMS = notification.SendSMS && recipient.SendSMS
                });
            }

            // these could fail but the individual sender will track that.  Also don't want to abort if one of these fails.
            foreach (var cot in notification.CotNotifications)
            {
                await _cotSender.SendAsync(cot, device.Result, location, orgEntityHeader, userEntityHeader);
            }

            foreach (var mqtt in notification.MqttNotifications)
            {
                await _mqttSender.SendAsync(mqtt, device.Result, location, orgEntityHeader, userEntityHeader);
            }

            foreach (var rest in notification.RestNotifications)
            {
                await _restSender.SendAsync(rest, device.Result, location, orgEntityHeader, userEntityHeader);
            }

            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Completed");
            _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__RaiseNotificationAsync]", $"Completed",
                raisedNotification.TestMode.ToString().ToKVP("testMode"),
                raisedNotification.Id.ToKVP("id"),
                _emailSender.EmailsSent.ToString().ToKVP("emailsSent"),
                _smsSender.TextMessagesSent.ToString().ToKVP("textSent"));

            return InvokeResult.Success;
        }
    }

    public class NotificationRecipient
    {
        public bool IsAppUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }

        public static NotificationRecipient FromAppUser(AppUser appUser)
        {
            return new NotificationRecipient()
            {
                IsAppUser = true,
                Id = appUser.Id,
                Phone = appUser.PhoneNumber,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                SendEmail = true,
                SendSMS = true,
            };
        }

        public static NotificationRecipient FromExternalContext(ExternalContact externalContact)
        {
            return new NotificationRecipient()
            {
                IsAppUser = false,
                Id = externalContact.Id,
                FirstName = externalContact.FirstName,
                LastName = externalContact.LastName,
                Email = externalContact.Email,
                Phone = externalContact.Phone,
                SendEmail = externalContact.SendEmail,
                SendSMS = externalContact.SendSMS,
            };
        }

    }
}
