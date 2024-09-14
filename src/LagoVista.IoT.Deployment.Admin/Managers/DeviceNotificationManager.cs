using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using LagoVista.UserAdmin.Models.Orgs;
using Org.BouncyCastle.Bcpg;
using RingCentral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceNotificationManager : ManagerBase, IDeviceNotificationManager
    {
        private readonly IDeviceNotificationRepo _deviceNotificationRepo;
        private readonly IDistributionListRepo _distroListRepo;
        private readonly IOrgLocationRepo _orgLocationRepo;
        private readonly LagoVista.IoT.DeviceManagement.Core.IDeviceManager _deviceManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IStaticPageStorage _staticPageStorage;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IAppConfig _appConfig;
        private readonly ILinkShortener _linkShortener;
        private readonly IDeviceNotificationTracking _notificationTracking;
        private readonly IDeploymentInstanceRepo _deploymentRepo;
        private readonly ILogger _logger;
        private readonly IAppUserRepo _appUserRepo;
        private readonly ISecureStorage _secureStorage;

        public DeviceNotificationManager(IDeviceNotificationRepo deviceNotificationRepo, IDeviceRepositoryManager repoManager, ILinkShortener linkShortner, IDeviceNotificationTracking notificationTracking,
                IDistributionListRepo distroListRepo, IOrgLocationRepo orgLocationRepo, LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, IEmailSender emailSender, ISmsSender smsSender, ISecureStorage secureStorage,
                IAppUserRepo appUserRepo, IStaticPageStorage staticPageStorage, IDeploymentInstanceRepo deploymentRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
            base(logger, appConfig, dependencyManager, security)
        {
            _deviceNotificationRepo = deviceNotificationRepo ?? throw new ArgumentNullException(nameof(deviceNotificationRepo));
            _distroListRepo = distroListRepo ?? throw new ArgumentNullException(nameof(distroListRepo));
            _orgLocationRepo = orgLocationRepo ?? throw new ArgumentNullException(nameof(orgLocationRepo));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _linkShortener = linkShortner ?? throw new ArgumentNullException(nameof(linkShortner));
            _notificationTracking = notificationTracking ?? throw new ArgumentNullException(nameof(notificationTracking));
            _deploymentRepo = deploymentRepo ?? throw new ArgumentNullException(nameof(deploymentRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _staticPageStorage = staticPageStorage ?? throw new ArgumentNullException(nameof(staticPageStorage));
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        public async Task<InvokeResult> AddNotificationAsync(DeviceNotification notification, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(notification, Actions.Create);
            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Create, user, org);

            foreach (var mqtt in notification.MqttNotifications)
            {
                if (!String.IsNullOrEmpty(mqtt.Password))
                {
                    var result = await _secureStorage.AddSecretAsync(org, mqtt.Password);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.PasswordSecretId = result.Result;
                    mqtt.Password = null;
                }

                if (!String.IsNullOrEmpty(mqtt.Certificate))
                {
                    var result = await _secureStorage.AddSecretAsync(org, mqtt.Certificate);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.CertificateSecureId = result.Result;
                    mqtt.Certificate = null;
                }

                if (!String.IsNullOrEmpty(mqtt.CertificatePassword))
                {
                    var result = await _secureStorage.AddSecretAsync(org, mqtt.CertificatePassword);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.CertificatePasswordSecureId = result.Result;
                    mqtt.CertificatePassword = null;
                }
            }

            foreach (var rest in notification.RestNotifications)
            {
                if (!String.IsNullOrEmpty(rest.BasicAuthPassword))
                {
                    var result = await _secureStorage.AddSecretAsync(org, rest.BasicAuthPassword);
                    if (!result.Successful) return result.ToInvokeResult();

                    rest.BasicAuthPasswordSecretId = result.Result;
                    rest.BasicAuthPassword = null;
                }
            }

            await _deviceNotificationRepo.AddNotificationAsync(notification);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteNotificationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var notification = await _deviceNotificationRepo.GetNotificationAsync(id);
            await ConfirmNoDepenenciesAsync(notification);

            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _deviceNotificationRepo.DeleteNotificationAsync(id);

            return InvokeResult.Success;
        }

        public async Task<DeviceNotification> GetNotificationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var notification = await _deviceNotificationRepo.GetNotificationAsync(id);
            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Read, user, org);

            return notification;
        }

        public async Task<ListResponse<DeviceNotificationSummary>> GetNotificationsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceErrorCode));
            return await _deviceNotificationRepo.GetNotificationForOrgAsync(orgId, listRequest);
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

        public async Task<InvokeResult> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Starting - Test Mode {raisedNotification.TestMode}");

            var result = raisedNotification.Validate();
            if (!result.Successful)
            {
                _logger.AddException($"[DeviceNotificationManager__RaiseNotificationAsync]", new Exception($"Validation Error on RaiseDeviceNotification {result.ErrorMessage}"));
                return result;
            }

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(raisedNotification.DeviceRepositoryId, orgEntityHeader, userEntityHeader);
            if (repo == null) return InvokeResult.FromError($"Could not locate device repository {raisedNotification.DeviceRepositoryId}");

            if (EntityHeader.IsNullOrEmpty(repo.Instance)) return InvokeResult.FromError($"Device does not have a deployment instance for device repository: ${repo.Name}");

            InvokeResult<Device> device = null;

            if (!String.IsNullOrEmpty(raisedNotification.DeviceId))
                device = await _deviceManager.GetDeviceByDeviceIdAsync(repo, raisedNotification.DeviceId, orgEntityHeader, userEntityHeader);
            else if (!string.IsNullOrEmpty(raisedNotification.DeviceUniqueId))
                device = await _deviceManager.GetDeviceByIdAsync(repo, raisedNotification.DeviceUniqueId, orgEntityHeader, userEntityHeader);
            else
                return InvokeResult.FromError($"Must provide either DeviceId or DeviceUniqueId.");

            if (device == null) return InvokeResult.FromError($"Could not locate device {(String.IsNullOrEmpty(raisedNotification.DeviceId) ? raisedNotification.DeviceId : raisedNotification.DeviceUniqueId)} in device repository {raisedNotification.DeviceRepositoryId}");

            var notification = await _deviceNotificationRepo.GetNotificationByKeyAsync(orgEntityHeader.Id, raisedNotification.NotificationKey);

            _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Sending notification {notification.Name} for device {device.Result.Name} in {repo.Name} repository");

            var deployment = await _deploymentRepo.GetInstanceAsync(repo.Instance.Id);
            if (deployment == null) return InvokeResult.FromError($"Could not locate deployment {repo.Instance.Text} - {repo.Instance.Id}");
            OrgLocation location = null;

            var appUsers = new List<EntityHeader>();
            var externalContacts = new List<ExternalContact>();

            if (raisedNotification.AdditionalUsers != null)
                appUsers.AddRange(raisedNotification.AdditionalUsers);

            if (raisedNotification.AdditionalExternalContacts != null)
                externalContacts.AddRange(raisedNotification.AdditionalExternalContacts);

            if (!EntityHeader.IsNullOrEmpty(device.Result.DistributionList))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(device.Result.DistributionList.Id);
                if (!distroList.ExternalContacts.Any())
                {
                    _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Found distribution list, but no external contacts, will not attempt to send.");
                    return InvokeResult.Success;
                }

                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Distribution List {distroList.Name} with {distroList.ExternalContacts.Count} contacts");

                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);
            }
            else
            {
                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - No distribution list on device, will not attempt to send.");
                return InvokeResult.Success;
            }

            if (!EntityHeader.IsNullOrEmpty(device.Result.Location))
            {
                location = await _orgLocationRepo.GetLocationAsync(device.Result.Location.Id);
                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - found location {location.Name} on device, can not append location information");
            }
            else
                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - No location set on device, can not append location information");

            String fullLandingPageLink = null;
            String acknowledgeLink = null;
            String emailContent = null;
            String smsContent = null;
            String pageId = null;

            if (notification.IncludeLandingPage)
            {
                _logger.Trace("[DeviceNotificationManager__RaiseNotificationAsync] - Including Landindg page");
                var template = await ReplaceTags(_appConfig.WebAddress, notification.LandingPageContent, device.Result, location);
                if (deployment.TestMode || raisedNotification.TestMode)
                    template = $"<h1>TESTING - TESTING</h1> {template}";

                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Tags replaced in template");

                var storageResult = await _staticPageStorage.StorePageAsync(orgEntityHeader.Id, "devnotif", template);
                if (!storageResult.Successful) return storageResult.ToInvokeResult();

                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Static page stored page id: {storageResult.Result}");

                pageId = storageResult.Result;
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                fullLandingPageLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotification.Id}/{orgEntityHeader.Id}/[RecipientId]/{pageId}";
                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Including Landindg page - {fullLandingPageLink}");
            }
            else
            {
                // hand this off to an ASP.NET API Controller that will handle the request and return HTML as a static content.
                acknowledgeLink = $"{_appConfig.WebAddress}/device/notifications/{raisedNotification.Id}/{orgEntityHeader.Id}/[RecipientId]/acknowledge";
                _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Including Landindg page - {acknowledgeLink}");
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

            foreach (var appUser in appUsers)
            {
                var actualInk = String.IsNullOrEmpty(fullLandingPageLink) ? acknowledgeLink.Replace("[RecipientId]", appUser.Id)
                   : fullLandingPageLink.Replace("[RecipientId]", appUser.Id);

                var shortenedLink = await _linkShortener.ShortenLinkAsync(actualInk);
                if (!shortenedLink.Successful) return shortenedLink.ToInvokeResult();

                var linkLabel = String.IsNullOrEmpty(fullLandingPageLink) ? "Details" : "Acknowledge";

                var user = await _appUserRepo.FindByIdAsync(appUser.Id);

                if (user == null)
                {
                    _logger.AddCustomEvent(LogLevel.Warning, $"[DeviceNotificationManager__RaiseNotificationAsync__SendEmail__AppUser]",
                        $"Request to send notification to app user, but could not find app user with id: ({user.Id}) but email addess is empty");

                }
                else
                {
                    if (!String.IsNullOrEmpty(emailContent))
                    {
                        if (String.IsNullOrEmpty(user.Email))
                            _logger.AddCustomEvent(LogLevel.Warning, $"[DeviceNotificationManager__RaiseNotificationAsync__SendEmail__AppUser]",
                                $"Request to send email to {user.FirstName} {user.LastName} ({user.Id}) but email addess is empty");
                        else
                        {
                            _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync__AppUser] - Sending Email To {user.FirstName} {user.LastName} {user.Email} -  {shortenedLink.Result} ({actualInk})");
                            result = await _emailSender.SendAsync(user.Email, notification.EmailSubject, emailContent + $"<div style='font-size:16px'><a href='{shortenedLink.Result}'>{linkLabel}</a></div>", orgEntityHeader, userEntityHeader);
                            if (result.Successful)
                                emailsSent++;
                            else
                                _logger.AddException($"[DeviceNotificationManager__RaiseNotificationAsync__SendEmail__AppUser]", new Exception($"Error sending email to {user.Email} - {result.ErrorMessage}"));
                        }
                    }

                    if (String.IsNullOrEmpty(smsContent))
                    {
                        if (String.IsNullOrEmpty(user.PhoneNumber))
                            _logger.AddCustomEvent(LogLevel.Warning, $"[DeviceNotificationManager__RaiseNotificationAsync__SendSms__AppUser]",
                                $"Request to send email to {user.FirstName} {user.LastName} ({user.Id}) but phone number is empty");
                        else
                        {
                            _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync__AppUser] - Sending SMS To {user.FirstName} {user.LastName} {user.PhoneNumber} -  {shortenedLink.Result} ({actualInk})");
                            result = await _smsSender.SendAsync(user.PhoneNumber, $"{smsContent} {shortenedLink.Result}");
                            if (result.Successful)
                                textSent++;
                            else
                                _logger.AddException($"[DeviceNotificationManager__RaiseNotificationAsync__SendSms__AppUser", new Exception($"Error sending sms to {user.PhoneNumber} - {result.ErrorMessage}"));
                        }
                    }

                    await _notificationTracking.AddHistoryAsync(new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-{user.Id}")
                    {
                        UserId = user.Id,
                        UserName = $"{user.FirstName} {user.LastName}",
                        OrgId = orgEntityHeader.Id,
                        StaticPageId = pageId,
                        Notification = notification.Name,
                        NotificationId = notification.Id,
                        TestMode = raisedNotification.TestMode,
                        SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                        SentEmail = notification.SendEmail,
                        SentSMS = notification.SendSMS && !String.IsNullOrEmpty(user.PhoneNumber)
                    });
                }
            }

            foreach (var recpient in externalContacts)
            {
                var actualInk = String.IsNullOrEmpty(fullLandingPageLink) ? acknowledgeLink.Replace("[RecipientId]", recpient.Id)
                    : fullLandingPageLink.Replace("[RecipientId]", recpient.Id);

                var shortenedLink = await _linkShortener.ShortenLinkAsync(actualInk);
                if (!shortenedLink.Successful) return shortenedLink.ToInvokeResult();

                var linkLabel = String.IsNullOrEmpty(fullLandingPageLink) ? "Details" : "Acknowledge";

                if (recpient.SendEmail && !String.IsNullOrEmpty(emailContent))
                {
                    if (String.IsNullOrEmpty(recpient.Email))
                        _logger.AddCustomEvent(LogLevel.Warning, $"[DeviceNotificationManager__RaiseNotificationAsync__SendEmail__ExternalContact]",
                            $"Request to send email to {recpient.FirstName} {recpient.LastName} ({recpient.Id}) but email addess is empty");
                    else
                    {
                        _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync__ExternalContact] - Sending Email To {recpient.FirstName} {recpient.LastName} {recpient.Email} -  {shortenedLink.Result} ({actualInk})");
                        result = await _emailSender.SendAsync(recpient.Email, notification.EmailSubject, emailContent + $"<div style='font-size:16px'><a href='{shortenedLink.Result}'>{linkLabel}</a></div>", orgEntityHeader, userEntityHeader);
                        if (result.Successful)
                            emailsSent++;
                        else
                            _logger.AddException($"[DeviceNotificationManager__RaiseNotificationAsync__SendEmail__ExternalContact]", new Exception($"Error sending email to {recpient.Email} - {result.ErrorMessage}"));
                    }
                }

                if (recpient.SendSMS && !String.IsNullOrEmpty(smsContent))
                {
                    if (String.IsNullOrEmpty(recpient.Phone))
                        _logger.AddCustomEvent(LogLevel.Warning, $"[DeviceNotificationManager__RaiseNotificationAsync__SendSms__ExternalContact]",
                            $"Request to send email to {recpient.FirstName} {recpient.LastName} ({recpient.Id}) but phone number is empty");
                    else
                    {
                        _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync__ExternalContact] - Sending SMS To {recpient.FirstName} {recpient.LastName} {recpient.Phone} -  {shortenedLink.Result} ({actualInk})");
                        result = await _smsSender.SendAsync(recpient.Phone, $"{smsContent} {shortenedLink.Result}");
                        if (result.Successful)
                            textSent++;
                        else
                            _logger.AddException($"[DeviceNotificationManager__RaiseNotificationAsync__SendSms__ExternalContact]", new Exception($"Error sending sms to {recpient.Phone} - {result.ErrorMessage}"));
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

            _logger.Trace($"[DeviceNotificationManager__RaiseNotificationAsync] - Completed");
            _logger.AddCustomEvent(LogLevel.Message, $"[DeviceNotificationManager__RaiseNotificationAsync]", $"Completed",
                raisedNotification.TestMode.ToString().ToKVP("testMode"),
                raisedNotification.Id.ToKVP("id"),
                emailsSent.ToString().ToKVP("emailsSent"),
                textSent.ToString().ToKVP("textSent"));


            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateNotificationAsync(DeviceNotification notification, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(notification, Actions.Create);
            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Update, user, org);

            foreach (var mqtt in notification.MqttNotifications)
            {

                if (!String.IsNullOrEmpty(mqtt.Password))
                {
                    if (!String.IsNullOrEmpty(mqtt.PasswordSecretId))
                    {
                        await _secureStorage.RemoveSecretAsync(org, mqtt.PasswordSecretId);
                    }

                    var result = await _secureStorage.AddSecretAsync(org, mqtt.Password);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.PasswordSecretId = result.Result;
                    mqtt.Password = null;
                }

                if (!String.IsNullOrEmpty(mqtt.Certificate))
                {
                    if (!String.IsNullOrEmpty(mqtt.CertificateSecureId))
                    {
                        await _secureStorage.RemoveSecretAsync(org, mqtt.CertificateSecureId);
                    }

                    var result = await _secureStorage.AddSecretAsync(org, mqtt.Certificate);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.CertificateSecureId = result.Result;
                    mqtt.Certificate = null;
                }

                if (!String.IsNullOrEmpty(mqtt.CertificatePassword))
                {
                    if (!String.IsNullOrEmpty(mqtt.CertificatePasswordSecureId))
                    {
                        await _secureStorage.RemoveSecretAsync(org, mqtt.CertificatePasswordSecureId);
                    }

                    var result = await _secureStorage.AddSecretAsync(org, mqtt.CertificatePassword);
                    if (!result.Successful) return result.ToInvokeResult();

                    mqtt.CertificatePasswordSecureId = result.Result;
                    mqtt.CertificatePassword = null;
                }
            }

            foreach (var rest in notification.RestNotifications)
            {
                if (!String.IsNullOrEmpty(rest.BasicAuthPassword))
                {
                    if (!String.IsNullOrEmpty(rest.BasicAuthPasswordSecretId))
                    {
                        await _secureStorage.RemoveSecretAsync(org, rest.BasicAuthPasswordSecretId);
                    }

                    var result = await _secureStorage.AddSecretAsync(org, rest.BasicAuthPassword);
                    if (!result.Successful) return result.ToInvokeResult();

                    rest.BasicAuthPasswordSecretId = result.Result;
                    rest.BasicAuthPassword = null;
                }
            }

            await _deviceNotificationRepo.UpdateNotificationAsync(notification);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<string>> HandleNotificationAsync(string notifid, string orgid, string recipientid, string pageid)
        {
            var page = await _staticPageStorage.GetPageAsync(orgid, "devnotif", pageid);

            if (page.Successful)
                await _notificationTracking.MarkAsViewed($"{notifid}-{recipientid}");

            return page;
        }

        public async Task<InvokeResult> AcknowledgeNotificationAsync(string notifid, string recipientid)
        {
            await _notificationTracking.MarkAsViewed($"{notifid}-{recipientid}");
            return InvokeResult.Success;
        }

        public Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _notificationTracking.GetHistoryAsync(deviceid, listRequest);
        }
    }
}
