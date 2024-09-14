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
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using System.Linq;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using System.Text;
using LagoVista.UserAdmin.Models.Users;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationSender : INotificationSender
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
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

        public NotificationSender(ILogger logger, IDistributionListRepo distroListRepo, IDeviceNotificationTracking notificationTracking, IDeviceNotificationRepo deviceNotificationRepo, IOrgLocationRepo orgLocationRepo, LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, IEmailSender emailSender, ISmsSender smsSender,
                                  IAppUserRepo appUserRepo, IDeviceRepositoryManager repoManager, IStaticPageStorage staticPageStorage, ILinkShortener linkShortner, IDeploymentInstanceRepo deploymentRepo, IAppConfig appConfig)
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
            _notificationTracking = notificationTracking ?? throw new ArgumentNullException(nameof(notificationTracking));
        }

        private async Task<string> ReplaceTags(string site, string template, Device device, OrgLocation location)
        {
            //The following tags will be replaced in the generated content [DeviceName] [DeviceId] [DeviceLocation] [DeviceSummary] [NotificationTimeStamp]

            template = template.Replace("[DeviceName]", device.Name);
            template = template.Replace("[DeviceId]", device.DeviceId);
            if (location != null)
            {
                template = template.Replace("[DeviceLocation]", location.ToHTML(site));
                if (template.Contains("[Location_Admin_Contact]") && !EntityHeader.IsNullOrEmpty(location.AdminContact))
                {
                    var adminContact = await _appUserRepo.FindByIdAsync(location.AdminContact.Id);
                    if (adminContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(adminContact.PhoneNumber) ? String.Empty : $"<a href='tel:{adminContact.PhoneNumber}> ({adminContact.PhoneNumber})</a>";
                        template.Replace("[Location_Admin_Contact]", $"<div>{adminContact.Name} {phoneHtml}</div>");
                    }
                }

                if (template.Contains("[Location_Technical_Contact]") && !EntityHeader.IsNullOrEmpty(location.TechnicalContact))
                {
                    var technicalContact = await _appUserRepo.FindByIdAsync(location.TechnicalContact.Id);
                    if (technicalContact != null)
                    {
                        var phoneHtml = String.IsNullOrEmpty(technicalContact.PhoneNumber) ? String.Empty : $"<a href='tel:{technicalContact.PhoneNumber}> ({technicalContact.PhoneNumber})</a>";
                        template.Replace("[Location_Technical_Contact]", $"<div>{technicalContact.Name} {phoneHtml}</div>");
                    }
                }
            }
            else
                template = template.Replace("[DeviceLocation]", string.Empty);

            var sensorHtml = new StringBuilder("<h4>Sensors</h4>");

            if (device.SensorCollection != null)
            {
                foreach (var sensor in device.SensorCollection)
                {
                    sensorHtml.AppendLine($"<div>{sensor.Name}: {sensor.Value}</div>");
                }
            }
            template = template.Replace("[DeviceSensors]", sensorHtml.ToString());
            template = template.Replace("[DeviceSummary]", device.Summary);
            template = template.Replace("[NotificationTimeStamp]", DateTime.Now.ToString());

            return template;
        }

        private async Task<InvokeResult<List<Recipient>>> GetRecipientsAsync(RaisedDeviceNotification raisedNotification, Device device, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            var recipients = new List<Recipient>();

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
                    return InvokeResult<List<Recipient>>.Create(recipients);
                }

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Distribution List {distroList.Name} with {distroList.ExternalContacts.Count} contacts");

                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);
            }
            else
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No distribution list on device, will not attempt to send.");
                return InvokeResult<List<Recipient>>.Create(recipients);
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
                        recipients.Add(Recipient.FromAppUser(user));
                }
            }

            recipients.AddRange(externalContacts.Select(eu => Recipient.FromExternalContext(eu)));

            return InvokeResult<List<Recipient>>.Create(recipients);
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

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(raisedNotification.DeviceRepositoryId, orgEntityHeader, userEntityHeader);
            if (repo == null) return InvokeResult.FromError($"Could not locate device repository {raisedNotification.DeviceRepositoryId}");

            if (EntityHeader.IsNullOrEmpty(repo.Instance)) return InvokeResult.FromError($"Device does not have a deployment instance for device repository: ${repo.Name}");

            InvokeResult<Device> device;
            if (!String.IsNullOrEmpty(raisedNotification.DeviceId))
                device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, raisedNotification.DeviceId, orgEntityHeader, userEntityHeader);
            else if (!string.IsNullOrEmpty(raisedNotification.DeviceUniqueId))
                device = await _deviceManager.GetDeviceByIdAsync(repo, raisedNotification.DeviceUniqueId, orgEntityHeader, userEntityHeader);
            else
                return InvokeResult.FromError($"Must provide either DeviceId or DeviceUniqueId.");

            if (device == null) return InvokeResult.FromError($"Could not locate device {(String.IsNullOrEmpty(raisedNotification.DeviceId) ? raisedNotification.DeviceId : raisedNotification.DeviceUniqueId)} in device repository {raisedNotification.DeviceRepositoryId}");

            var notification = await _deviceNotificationRepo.GetNotificationByKeyAsync(orgEntityHeader.Id, raisedNotification.NotificationKey);

            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Sending notification {notification.Name} for device {device.Result.Name} in {repo.Name} repository");

            var deployment = await _deploymentRepo.GetInstanceAsync(repo.Instance.Id);
            if (deployment == null) return InvokeResult.FromError($"Could not locate deployment {repo.Instance.Text} - {repo.Instance.Id}");
            OrgLocation location = null;


            if (!EntityHeader.IsNullOrEmpty(device.Result.Location))
            {
                location = await _orgLocationRepo.GetLocationAsync(device.Result.Location.Id);
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - found location {location.Name} on device, can not append location information");
            }
            else
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No location set on device, can not append location information");

            String fullLandingPageLink = null;
            String acknowledgeLink = null;
            String emailContent = null;
            String smsContent = null;
            String pageId = null;

            if (notification.IncludeLandingPage)
            {
                _logger.Trace("[NotificationSender__RaiseNotificationAsync] - Including Landindg page");
                var template = await ReplaceTags(_appConfig.WebAddress, notification.LandingPageContent, device.Result, location);
                if (deployment.TestMode || raisedNotification.TestMode)
                    template = $"<h1>TESTING - TESTING</h1> {template}";

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Tags replaced in template");

                var storageResult = await _staticPageStorage.StorePageAsync(orgEntityHeader.Id, "devnotif", template);
                if (!storageResult.Successful) return storageResult.ToInvokeResult();

                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Static page stored page id: {storageResult.Result}");

                pageId = storageResult.Result;
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                fullLandingPageLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotification.Id}/{orgEntityHeader.Id}/[RecipientId]/{pageId}";
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Including Landindg page - {fullLandingPageLink}");
            }
            else
            {
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                acknowledgeLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotification.Id}/{orgEntityHeader.Id}/[RecipientId]/acknowledge";
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Including Landindg page - {acknowledgeLink}");
            }

            if (notification.SendEmail)
                emailContent = await ReplaceTags(_appConfig.WebAddress, notification.EmailContent, device.Result, location);

            if (notification.SendSMS)
                smsContent = await ReplaceTags(_appConfig.WebAddress, notification.SmsContent, device.Result, location);

            if (deployment.TestMode || raisedNotification.TestMode)
            {
                emailContent = $"<h1>TESTING - TESTING</h1> {emailContent}";
                smsContent = $"TESTING TESTING - {smsContent}";
            }

            var emailsSent = 0;
            var textSent = 0;

            var recipientsResult = await GetRecipientsAsync(raisedNotification, device.Result, orgEntityHeader, userEntityHeader);

            if (!recipientsResult.Successful)
                return result.ToInvokeResult();

            var recipients = recipientsResult.Result;
            
            foreach (var recpient in recipients)
            {
                var actualInk = String.IsNullOrEmpty(fullLandingPageLink) ? acknowledgeLink.Replace("[RecipientId]", recpient.Id)
                    : fullLandingPageLink.Replace("[RecipientId]", recpient.Id);

                var shortenedLink = await _linkShortener.ShortenLinkAsync(actualInk);
                if (!shortenedLink.Successful) return shortenedLink.ToInvokeResult();

                var linkLabel = String.IsNullOrEmpty(fullLandingPageLink) ? "Details" : "Acknowledge";

                if (recpient.SendEmail && !String.IsNullOrEmpty(emailContent))
                {
                    if (String.IsNullOrEmpty(recpient.Email))
                        _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact]",
                            $"Request to send email to {recpient.FirstName} {recpient.LastName} ({recpient.Id}) but email addess is empty");
                    else
                    {
                        _logger.Trace($"[NotificationSender__RaiseNotificationAsync__ExternalContact] - Sending Email To {recpient.FirstName} {recpient.LastName} {recpient.Email} -  {shortenedLink.Result} ({actualInk})");
                        result = await _emailSender.SendAsync(recpient.Email, notification.EmailSubject, emailContent + $"<div style='font-size:16px'><a href='{shortenedLink.Result}'>{linkLabel}</a></div>", orgEntityHeader, userEntityHeader);
                        if (result.Successful)
                            emailsSent++;
                        else
                            _logger.AddException($"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact]", new Exception($"Error sending email to {recpient.Email} - {result.ErrorMessage}"));
                    }
                }

                if (recpient.SendSMS && !String.IsNullOrEmpty(smsContent))
                {
                    if (String.IsNullOrEmpty(recpient.Phone))
                        _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact]",
                            $"Request to send email to {recpient.FirstName} {recpient.LastName} ({recpient.Id}) but phone number is empty");
                    else
                    {
                        _logger.Trace($"[NotificationSender__RaiseNotificationAsync__ExternalContact] - Sending SMS To {recpient.FirstName} {recpient.LastName} {recpient.Phone} -  {shortenedLink.Result} ({actualInk})");
                        result = await _smsSender.SendAsync(recpient.Phone, $"{smsContent} {shortenedLink.Result}");
                        if (result.Successful)
                            textSent++;
                        else
                            _logger.AddException($"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact]", new Exception($"Error sending sms to {recpient.Phone} - {result.ErrorMessage}"));
                    }
                }

                await _notificationTracking.AddHistoryAsync(new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-{recpient.Id}")
                {
                    UserId = recpient.Id,
                    UserName = $"{recpient.FirstName} {recpient.LastName}",
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = pageId,
                    TestMode = raisedNotification.TestMode,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    SentEmail = notification.SendEmail && recpient.SendEmail,
                    SentSMS = notification.SendSMS && recpient.SendSMS
                });
            }

            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Completed");
            _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__RaiseNotificationAsync]", $"Completed",
                raisedNotification.TestMode.ToString().ToKVP("testMode"),
                raisedNotification.Id.ToKVP("id"),
                emailsSent.ToString().ToKVP("emailsSent"),
                textSent.ToString().ToKVP("textSent"));

            return InvokeResult.Success;
        }

        internal class Recipient
        {
            public bool IsAppUser { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Id { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            public bool SendEmail { get; set; }
            public bool SendSMS { get; set; }

            public static Recipient FromAppUser(AppUser appUser)
            {
                return new Recipient()
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

            public static Recipient FromExternalContext(ExternalContact externalContact)
            {
                return new Recipient()
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
}
