﻿using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
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
using System.Diagnostics;
using LagoVista.Core.Exceptions;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using System.Diagnostics.Metrics;
using Prometheus;
using RingCentral;
using ProtoBuf.WellKnownTypes;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class NotificationSender : INotificationSender
    {
        protected static readonly Counter SentNotifications = Metrics.CreateCounter("nuviot_send_notification", "Raise notification to be sent..", "entity");

        private readonly ILogger _logger;
        private readonly Interfaces.IEmailSender _emailSender;
        private readonly ISMSSender _smsSender;
        private readonly IDeviceNotificationTracking _notificationTracking;
        private readonly IDistributionListRepo _distroListRepo;
        private readonly IOrgLocationRepo _orgLocationRepo;
        private readonly LagoVista.IoT.DeviceManagement.Core.IDeviceManager _deviceManager;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IDeploymentInstanceRepo _deploymentRepo;
        private readonly IAppUserRepo _appUserRepo;
        private readonly IDeviceNotificationRepo _deviceNotificationRepo;
        private readonly ICOTSender _cotSender;
        private readonly IMqttSender _mqttSender;
        private readonly IRestSender _restSender;
        private readonly INotificationLandingPage _landingPageBuilder;
        private readonly IDeviceConfigHelper _deviceConfigHelper;
        private readonly ITimeZoneServices _timeZoneService;
        private readonly IOrganizationRepo _orgRepo;
        private readonly IBackgroundServiceTaskQueue _taskQueue;
        private readonly IRaisedNotificationHistoryRepo _raisedNotificationHistoryRepo;
        private readonly IDeviceCommandSender _deviceCommandSender;

        public NotificationSender(ILogger logger, IDistributionListRepo distroListRepo, IDeviceNotificationTracking notificationTracking, IDeviceNotificationRepo deviceNotificationRepo, IOrgLocationRepo orgLocationRepo,
            LagoVista.IoT.DeviceManagement.Core.IDeviceManager deviceManager, Interfaces.IEmailSender emailSender, ISMSSender smsSender, INotificationLandingPage landingPageBuilder, IOrganizationRepo orgRepo,
                                  IRaisedNotificationHistoryRepo raisedNotificationHistory, IBackgroundServiceTaskQueue taskQueue, IDeviceConfigHelper deviceConfigHelper, IAppUserRepo appUserRepo, IDeviceRepositoryManager repoManager,
                                  ICOTSender cotSender, IRestSender restSender, IMqttSender mqttSender, IDeviceCommandSender deviceCommandSender, IDeploymentInstanceRepo deploymentRepo, ITimeZoneServices timeZoneService)
        {
            _deviceNotificationRepo = deviceNotificationRepo ?? throw new ArgumentNullException(nameof(deviceNotificationRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _distroListRepo = distroListRepo ?? throw new ArgumentNullException(nameof(distroListRepo));
            _orgLocationRepo = orgLocationRepo ?? throw new ArgumentNullException(nameof(orgLocationRepo));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _deploymentRepo = deploymentRepo ?? throw new ArgumentNullException(nameof(deploymentRepo));
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
            _notificationTracking = notificationTracking ?? throw new ArgumentNullException(nameof(notificationTracking));
            _restSender = restSender ?? throw new ArgumentNullException(nameof(restSender));
            _cotSender = cotSender ?? throw new ArgumentNullException(nameof(cotSender));
            _mqttSender = mqttSender ?? throw new ArgumentNullException(nameof(mqttSender));
            _landingPageBuilder = landingPageBuilder ?? throw new ArgumentNullException(nameof(landingPageBuilder));
            _timeZoneService = timeZoneService ?? throw new ArgumentNullException(nameof(timeZoneService));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _raisedNotificationHistoryRepo = raisedNotificationHistory ?? throw new ArgumentNullException(nameof(raisedNotificationHistory));
            _taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(raisedNotificationHistory));
            _deviceConfigHelper = deviceConfigHelper ?? throw new ArgumentNullException(nameof(deviceConfigHelper));
            _deviceCommandSender = deviceCommandSender ?? throw new ArgumentNullException(nameof(deviceCommandSender));
        }

        private async Task<InvokeResult<List<NotificationRecipient>>> GetRecipientsAsync(RaisedDeviceNotification raisedNotification, Device device, EntityHeader orgEntityHeader, EntityHeader userEntityHeader, params EntityHeader[] additionalDistroLists)
        {
            var timings = new List<ResultTiming>();
            var recipients = new List<NotificationRecipient>();

            var appUsers = new List<EntityHeader>();
            var externalContacts = new List<ExternalContact>();

            if (raisedNotification.AdditionalUsers != null)
                appUsers.AddRange(raisedNotification.AdditionalUsers);

            if (raisedNotification.AdditionalExternalContacts != null)
                externalContacts.AddRange(raisedNotification.AdditionalExternalContacts);

            externalContacts.AddRange(device.NotificationContacts);

            foreach(var distro in additionalDistroLists)
            {
                if (distro != null)
                {
                    var dsw = Stopwatch.StartNew();
                    var distroList = await _distroListRepo.GetDistroListAsync(distro.Id, true);

                    _logger.Trace($"[NotificationSender__GetRecipientsAsync] - Distribution List {distroList.Name} with {distroList.ExternalContacts.Count} contacts", orgEntityHeader.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));

                    appUsers.AddRange(distroList.AppUsers);
                    externalContacts.AddRange(distroList.ExternalContacts);

                    timings.Add(new ResultTiming() { Key = "add distro list", Ms = dsw.Elapsed.TotalMilliseconds });
                }
            }

            if (!EntityHeader.IsNullOrEmpty(device.DistributionList))
            {
                var dsw = Stopwatch.StartNew();
                var distroList = await _distroListRepo.GetDistroListAsync(device.DistributionList.Id, true);
               
                _logger.Trace($"[NotificationSender__GetRecipientsAsync] - Distribution List {distroList.Name} with {distroList.ExternalContacts.Count} contacts", orgEntityHeader.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));

                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);

                timings.Add(new ResultTiming() { Key = "adddistrofromdevice", Ms = dsw.Elapsed.TotalMilliseconds });
            }
            else
            {
                timings.Add(new ResultTiming() { Key = "nodistroondevice", Ms = 0 });
            }

            foreach (var appUser in appUsers)
            {
                if (!recipients.Any(rec => rec.Id == appUser.Id))
                {
                    var usw = Stopwatch.StartNew();
                    var user = await _appUserRepo.FindByIdAsync(appUser.Id);
                    if (user == null)
                    {
                        _logger.AddCustomEvent(LogLevel.Warning, $"[NotificationSender__RaiseNotificationAsync__SendEmail__AppUser]",
                        $"Request to send notification to app user, but could not find app user with id: ({user.Id}).");
                    }
                    else
                        recipients.Add(NotificationRecipient.FromAppUser(user));

                    timings.Add(new ResultTiming() { Key = $"get user: {appUser.Text}", Ms = usw.Elapsed.TotalMilliseconds });
                }
            }

            recipients.AddRange(externalContacts.Select(eu => NotificationRecipient.FromExternalContext(eu)));
            recipients.EnsureUniqueNotifications();

            var uniuqueEmail = recipients.Count(recp => !String.IsNullOrEmpty(recp.Email));
            var uniuqueSms = recipients.Count(recp => !String.IsNullOrEmpty(recp.Phone));
            _logger.Trace($"[NotificationSender__GetRecipients] Total Count {recipients.Count} Email: {uniuqueEmail} SMS: {uniuqueSms}.", orgEntityHeader.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));

            var result = InvokeResult<List<NotificationRecipient>>.Create(recipients);
            result.Timings.AddRange(timings);
            return result;
        }

        private async Task<InvokeResult<Device>> GetDeviceAsync(RaisedDeviceNotification raisedNotification, DeviceRepository repo, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            if (!string.IsNullOrEmpty(raisedNotification.DeviceUniqueId))
                return await _deviceManager.GetDeviceByIdAsync(repo, raisedNotification.DeviceUniqueId, orgEntityHeader, userEntityHeader);
            else if (!String.IsNullOrEmpty(raisedNotification.DeviceId))
                return await _deviceManager.GetDeviceByDeviceIdAsync(repo, raisedNotification.DeviceId, orgEntityHeader, userEntityHeader);
            else
                return InvokeResult<Device>.FromError($"Must provide either DeviceId or DeviceUniqueId.");
        }

        public Task<InvokeResult> HandleNotificationTimeout(string originalNotificationHistoryId, string originalNotificationHistoryPartitionKey, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
         //   var originalNotification = await _raisedNotificationHistoryRepo.GetRaisedNotificationHistoryAsync(originalNotificationHistoryId, originalNotificationHistoryPartitionKey);
            
           
        
        }

        public async Task<InvokeResult<string>> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            var result = InvokeResult<string>.Create(String.Empty);
            var sw = Stopwatch.StartNew();
            var fullSw = Stopwatch.StartNew();
            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] Starting - Key: {raisedNotification.NotificationKey}. Test Mode {raisedNotification.TestMode}, Org: {orgEntityHeader.Text}, Repo: {raisedNotification.DeviceRepositoryId}, Device: {raisedNotification.DeviceId}", orgEntityHeader.Text.ToKVP("org"), 
                raisedNotification.NotificationKey.ToKVP("notification"),  orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));

            var validationResult = raisedNotification.Validate();
            if (!validationResult.Successful)
            {
                _logger.AddException($"[NotificationSender__RaiseNotificationAsync]", new Exception($"Validation Error on RaiseDeviceNotification {validationResult.ErrorMessage}"));
                return InvokeResult<string>.FromError(validationResult.ErrorMessage);
            }

            result.Timings.Add(new ResultTiming() { Key = "validated", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(raisedNotification.DeviceRepositoryId, orgEntityHeader, userEntityHeader);
            if (repo.OwnerOrganization.Id != orgEntityHeader.Id) throw new NotAuthorizedException("Org missmatch.");
            if (repo == null) return InvokeResult<string>.FromError($"Could not locate device repository {raisedNotification.DeviceRepositoryId}");            
            result.Timings.Add(new ResultTiming() { Key = "loadrepo", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            if (EntityHeader.IsNullOrEmpty(repo.Instance)) return InvokeResult<string>.FromError($"Device does not have a deployment instance for device repository: ${repo.Name}");

            var device = await GetDeviceAsync(raisedNotification, repo, orgEntityHeader, userEntityHeader);
            if (!device.Successful) return InvokeResult<string>.FromError($"Could not locate device {(String.IsNullOrEmpty(raisedNotification.DeviceId) ? raisedNotification.DeviceId : raisedNotification.DeviceUniqueId)} in device repository {raisedNotification.DeviceRepositoryId}");
            _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Sending notification {raisedNotification.NotificationKey} for device {device.Result.Name} in {repo.Name} repository", orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));
            result.Timings.AddRange(device.Timings);
            result.Timings.Add(new ResultTiming() { Key = "getdevice", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            if(device.Result.TestingMode)
            {
                raisedNotification.TestMode = true;
            }

            var notification = await _deviceNotificationRepo.GetNotificationByKeyAsync(orgEntityHeader.Id, device.Result.Customer?.Id, raisedNotification.NotificationKey);
            result.Timings.Add(new ResultTiming() { Key = "loadnotification", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            if (raisedNotification.Escalate && !EntityHeader.IsNullOrEmpty(notification.EscalationNotification))
            {
                _logger.Trace("[NotificationSender__RaiseNotificationAsync] - Will escaltate with notification:");
                notification = await _deviceNotificationRepo.GetNotificationAsync(notification.EscalationNotification.Id);
            }

            var recipientsResult = await GetRecipientsAsync(raisedNotification, device.Result, orgEntityHeader, userEntityHeader, notification.DistroList);

            if (!recipientsResult.Successful)
                return InvokeResult<String>.FromInvokeResult(result.ToInvokeResult());

            result.Timings.AddRange(recipientsResult.Timings);
            
            if (!recipientsResult.Result.Any())
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No recipients, will not send notification, but may forward to other devices - {notification.Name} for device {device.Result.Name} in {repo.Name} repository , spent {fullSw.Elapsed.TotalMilliseconds}ms", orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));
                result.Warnings.Add(new ErrorMessage($"No recipients, will not send notification, but may forward to other devices - {notification.Name} for device {device.Result.Name} in {repo.Name} repository"));
            }
            else
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - Has {recipientsResult.Result.Count} recipients, will send notification - {notification.Name} for device {device.Result.Name} in {repo.Name} repository", orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));

            var deployment = await _deploymentRepo.GetReadOnlyInstanceAsync(repo.Instance.Id);
            if (deployment == null) return InvokeResult<string>.FromError($"Could not locate deployment {repo.Instance.Text} - {repo.Instance.Id}");
            result.Timings.Add(new ResultTiming() { Key = "getinstance", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            OrgLocation location = null;
            if (!EntityHeader.IsNullOrEmpty(device.Result.Location))
            {
                location = await _orgLocationRepo.GetLocationAsync(device.Result.Location.Id);
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - found location {location.Name} on device, will append location information", orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));
                result.Timings.Add(new ResultTiming() { Key = "loadlandingpage", Ms = sw.Elapsed.TotalMilliseconds });
                sw.Restart();
            }
            else
            {
                _logger.Trace($"[NotificationSender__RaiseNotificationAsync] - No location set on device, can not append location information", orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"));
                result.Timings.Add(new ResultTiming() { Key = "nolandingpage", Ms = sw.Elapsed.TotalMilliseconds });
                sw.Restart();
            }

            var pageResult = await _landingPageBuilder.PreparePage(raisedNotification.Id, raisedNotification.DeviceErrorId, notification, deployment.TestMode, device.Result, location, orgEntityHeader, userEntityHeader);
            result.Timings.Add(new ResultTiming() { Key = "preparelandingpage", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            var page = pageResult.Result;

            await _smsSender.PrepareMessage(notification, deployment.TestMode, device.Result, location);
            result.Timings.Add(new ResultTiming() { Key = "preparesms", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            await _emailSender.PrepareMessage(notification, deployment.TestMode, device.Result, location);
            result.Timings.Add(new ResultTiming() { Key = "prepareemail", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            await _deviceCommandSender.PrepareMessage(notification, deployment.TestMode, device.Result, location);
            result.Timings.Add(new ResultTiming() { Key = "preparedevicecommand", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            var recipients = recipientsResult.Result;
            result.Timings.AddRange(recipientsResult.Timings);
            result.Timings.Add(new ResultTiming() { Key = "getrecipients", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            var hist = new RaisedNotificationHistory(device.Result.DeviceRepository.Id)
            {
                OrgId = device.Result.OwnerOrganization.Id,
                RaisedNotificationId = raisedNotification.Id,
                DeviceId = device.Result.DeviceId,
                DeviceUniqueId = device.Result.Id,
                DeviceRepoId = device.Result.DeviceRepository.Id,
                Notification = notification.Name,
                NotificationId = notification.Id,
                TestMode = deployment.TestMode,
                DryRun = raisedNotification.DryRun,
                TimeStamp = DateTime.UtcNow.ToJSONString(),
                Customer = device.Result.Customer?.Text,
                CustomerId = device.Result.Customer?.Id,
                SmsText = notification.SmsContent,
                EmailText = notification.EmailContent,
            };

            await _raisedNotificationHistoryRepo.AddHistoryAsync(hist);
            result.Result = hist.RowKey;

            result.Timings.Add(new ResultTiming() { Key = "addnotifications", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            foreach (var recipient in recipients)
            {
                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-{recipient.Id}")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    UserId = recipient.Id,
                    UserName = $"{recipient.FirstName} {recipient.LastName}",
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    SendEmail = notification.SendEmail && recipient.SendEmail,
                    SendSMS = notification.SendSMS && recipient.SendSMS,
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                };
               
                if (notificationHistory.SendSMS)
                {
                    notificationHistory.Phone = recipient.Phone;

                    if (!raisedNotification.DryRun)
                    {
                        var sendSmsResult = await _smsSender.SendAsync(notificationHistory.RowKey, recipient, page, !String.IsNullOrEmpty(raisedNotification.DeviceErrorId), orgEntityHeader, userEntityHeader);
                        notificationHistory.SentSMS = sendSmsResult.Successful;
                        if (!sendSmsResult.Successful)
                        {
                            notificationHistory.Errors += $"SMS Error: {sendSmsResult.ErrorMessage}; ";
                            _logger.AddCustomEvent(LogLevel.Error, $"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact]",
                                $"[NotificationSender__RaiseNotificationAsync__SendSms__ExternalContact] - Error sending SMS to {recipient.FirstName} {recipient.LastName} {recipient.Phone} - {sendSmsResult.ErrorMessage}");
                        }
                    }
                }

                if (notificationHistory.SendEmail)
                {
                    notificationHistory.Email = recipient.Email;

                    if (!raisedNotification.DryRun)
                    {
                        var sendEmailResult = await _emailSender.SendAsync(notificationHistory.RowKey, notification, recipient, !String.IsNullOrEmpty(raisedNotification.DeviceErrorId), page, orgEntityHeader, userEntityHeader);
                        notificationHistory.SentEmail = sendEmailResult.Successful;
                        if (!sendEmailResult.Successful)
                        {
                            notificationHistory.Errors += $"Email Error: {sendEmailResult.ErrorMessage}; ";
                            _logger.AddCustomEvent(LogLevel.Error, $"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact]",
                                $"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact] - Error sending email to {recipient.FirstName} {recipient.LastName} {recipient.Email} - {sendEmailResult.ErrorMessage}");
                        }
                    }
                }

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });
            }

            result.Timings.Add(new ResultTiming() { Key = "sendemailandsms", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            if (notification.ForwardToParentDevice && !EntityHeader.IsNullOrEmpty(device.Result.ParentDevice))
            {
                if (!raisedNotification.DryRun)
                {
                    await _deviceCommandSender.SendAsync(repo.Instance.Id, device.Result.ParentDevice.Id, orgEntityHeader, userEntityHeader);

                    result.Timings.Add(new ResultTiming() { Key = "queuedevicecommand", Ms = sw.Elapsed.TotalMilliseconds });
                    sw.Restart();
                }

                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-forward-parent")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                    ForwardDevice = device.Result.ParentDevice.Text,
                    ForwardDeviceUniqueId = device.Result.ParentDevice.Id,
                };

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });
            }

            if (!EntityHeader.IsNullOrEmpty(notification.ForwardDevice))
            {
                if(!raisedNotification.DryRun)
                {
                    var forwardResult = await _deviceCommandSender.SendAsync(repo.Instance.Id, notification.ForwardDevice.Id, orgEntityHeader, userEntityHeader);
                    if (!forwardResult.Successful)
                    {
                        _logger.AddCustomEvent(LogLevel.Error, $"[NotificationSender__RaiseNotificationAsync__ForwardContact]",
                            $"[NotificationSender__RaiseNotificationAsync__SendEmail__ExternalContact] - Error sending email to {notification.ForwardDevice.Text} {notification.ForwardDevice.Id} - {forwardResult.ErrorMessage}");

                        return InvokeResult<string>.FromError($"Could not locate device {notification.ForwardDevice.Text} in device repository {raisedNotification.DeviceRepositoryId}");
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__RaiseNotificationAsync__ForwardContact]",
                          $"[NotificationSender__RaiseNotificationAsync__ForwardContact] - Forwarded message to {notification.ForwardDevice.Text} {notification.ForwardDevice.Id} - {forwardResult.ErrorMessage}");
                    }
                }
                else
                {
                    _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__RaiseNotificationAsync__ForwardContact]",
                      $"[NotificationSender__RaiseNotificationAsync__ForwardContact] - Dry Run not sending {notification.ForwardDevice.Text} {notification.ForwardDevice.Id}");
                }

                result.Timings.Add(new ResultTiming() { Key = "forwarddevice", Ms = sw.Elapsed.TotalMilliseconds });
                sw.Restart();

                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-forward-parent")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                    ForwardDevice = notification.ForwardDevice.Text,
                    ForwardDeviceUniqueId = notification.ForwardDevice.Id,
                };

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });
            }


            // these could fail but the individual sender will track that.  Also don't want to abort if one of these fails.
            foreach (var cot in notification.CotNotifications)
            {
                if(!raisedNotification.DryRun)
                    await _cotSender.SendAsync(cot, device.Result, location, orgEntityHeader, userEntityHeader);

                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-forward-parent")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                    ForwardToServer = cot.Name,
                    ForwardToServerType = "COT Server"
                };

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });
            }

            foreach (var mqtt in notification.MqttNotifications)
            {
                if (!raisedNotification.DryRun)
                    await _mqttSender.SendAsync(mqtt, device.Result, location, orgEntityHeader, userEntityHeader);

                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-forward-parent")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                    ForwardToServer = mqtt.Name,
                    ForwardToServerType = "MQTT Server"
                };

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });
            }

            foreach (var rest in notification.RestNotifications)
            {
                if (!raisedNotification.DryRun)
                    await _restSender.SendAsync(rest, device.Result, location, orgEntityHeader, userEntityHeader);

                var notificationHistory = new DeviceNotificationHistory(device.Result.Id, $"{raisedNotification.Id}-forward-parent")
                {
                    RaisedNotificationId = raisedNotification.Id,
                    OrgId = orgEntityHeader.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = raisedNotification.TestMode,
                    DryRun = raisedNotification.DryRun,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    DeviceId = device.Result.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Result.Customer?.Text,
                    CustomerId = device.Result.Customer?.Id,
                    ForwardToServer = rest.Name,
                    ForwardToServerType = "REST Server"
                };

                await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                {
                    await _notificationTracking.AddHistoryAsync(notificationHistory);
                });

            }

            sw = Stopwatch.StartNew();
            var deviceStates = await _deviceConfigHelper.GetCustomDeviceStatesAsync(device.Result.DeviceConfiguration.Id, orgEntityHeader, userEntityHeader);
            if (deviceStates != null && deviceStates.HasValue)
            {
                var newDeviceState = deviceStates.Value.States.Where(st => st.Key.ToLower() == raisedNotification.NotificationKey).FirstOrDefault();
                if (newDeviceState != null)
                {
                    device.Result.CustomStatus = EntityHeader.Create(newDeviceState.Key, newDeviceState.Name);
                    await _deviceManager.UpdateDeviceAsync(repo, device.Result, orgEntityHeader, userEntityHeader);
                    result.Timings.Add(new ResultTiming() { Key = "updated custom status", Ms = sw.Elapsed.TotalMilliseconds });
                }
            }

            _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__RaiseNotificationAsync]", $"[NotificationSender__RaiseNotificationAsync] - Completed in {fullSw.Elapsed.TotalMilliseconds}ms",
                raisedNotification.TestMode.ToString().ToKVP("testMode"),
                raisedNotification.Id.ToKVP("id"),
                fullSw.Elapsed.TotalMilliseconds.ToString().ToKVP("msElapsed"),
                orgEntityHeader.Id.ToKVP("orgId"), raisedNotification.DeviceId.ToKVP("deviceId"),
                _emailSender.EmailsSent.ToString().ToKVP("emailsSent"),
                _smsSender.TextMessagesSent.ToString().ToKVP("textSent"));

            result.Timings.Add(new ResultTiming() { Key = "commpleted", Ms = fullSw.Elapsed.TotalMilliseconds });

            SentNotifications.Inc();

            return result;
        }

        public async Task<InvokeResult> SendDeviceOnlineNotificationAsync(Device device, string lastContact, bool testMode, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(device.DeviceRepository.Id, org, user);
            if (repo == null) return InvokeResult.FromError($"Could not locate device repository {device.DeviceRepository.Id}");

            if (repo.DisableWatchdog)
                return InvokeResult.Success;

            var notification = (!EntityHeader.IsNullOrEmpty(repo.DeviceOnlinNotification)) ?
            await _deviceNotificationRepo.GetNotificationAsync(repo.DeviceOnlinNotification.Id) :
            new DeviceNotification()
            {
                SendEmail = true,
                SendSMS = true,
                Name = "Device Online",
                EmailContent = $"<h4>Device {device.Name} came online.</h4><p>Last Contact [LastContactTime]</p>",
                SmsContent = $"Device{device.Name} came online, Last Contact [LastContactTime]",
                EmailSubject = $"Device {device.Name} came online",
            };

            return await SendNotification(device, repo, false, lastContact, notification, testMode, org, user);
        }

        public async Task<InvokeResult> SendDeviceOfflineNotificationAsync(Device device, string lastContact, bool testMode, EntityHeader org, EntityHeader user)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(device.DeviceRepository.Id, org, user);
            if (repo == null) return InvokeResult.FromError($"Could not locate device repository {device.DeviceRepository.Id}");
            if (repo.DisableWatchdog)
                return InvokeResult.Success;

            var notification = (!EntityHeader.IsNullOrEmpty(repo.DeviceOfflinNotification)) ?
            await _deviceNotificationRepo.GetNotificationAsync(repo.DeviceOfflinNotification.Id) :
            new DeviceNotification()
            {
                SendEmail = true,
                SendSMS = true,
                Name = "Device Offline",
                EmailContent = $"<h4>Device Offline {device.Name}</h4><p>Last Contact [LastContactTime]</p>",
                SmsContent = $"Device Offline {device.Name}, Last Contact [LastContactTime]",
                EmailSubject = $"Device Offline {device.Name}",
            };

            return await SendNotification(device, repo, true, lastContact, notification, testMode, org, user);
        }

        private async Task<InvokeResult> SendNotification(Device device, DeviceRepository repo, bool allowSilence, string lastContact, DeviceNotification notification, bool testMode, EntityHeader org, EntityHeader user)
        {
            var sw = Stopwatch.StartNew();

            _logger.Trace($"[NotificationSender__SendNotification] Starting - Key: {notification.Name}, Test Mode {testMode}, Org: {org.Text}, Repo: {repo.Name}, Device: {device.DeviceId}",  org.Text.ToKVP("org"), notification.Key.ToKVP("notiifcation"), org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));

            var contacts = new List<NotificationRecipient>();
            contacts.AddRange(device.NotificationContacts.Select(cnt => NotificationRecipient.FromExternalContext(cnt)));

            if (!EntityHeader.IsNullOrEmpty(device.WatchdogNotificationUser))
            {
                var watchDogNotifUser = await _appUserRepo.FindByIdAsync(device.WatchdogNotificationUser.Id);
                contacts.Add(NotificationRecipient.FromAppUser(watchDogNotifUser));
            }

            if (!EntityHeader.IsNullOrEmpty(repo.WatchdogNotificationUser))
            {
                var watchDogNotifUser = await _appUserRepo.FindByIdAsync(repo.WatchdogNotificationUser.Id);
                contacts.Add(NotificationRecipient.FromAppUser(watchDogNotifUser));
            }           

            if (!EntityHeader.IsNullOrEmpty(repo.OfflineDistributionList))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(repo.OfflineDistributionList.Id, true);
                contacts.AddRange(distroList.ExternalContacts.Select(cnt => NotificationRecipient.FromExternalContext(cnt)));

                foreach (var userEh in distroList.AppUsers)
                {
                    var appUser = await _appUserRepo.FindByIdAsync(userEh.Id);
                    contacts.Add(NotificationRecipient.FromAppUser(appUser));
                }
            }

            if (!contacts.Any())
            {
                _logger.Trace($"[NotificationSender__SendNotification] - No recipients, will not send notification - {notification.Name} for device {device.Name} in {repo.Name} repository, spent {sw.Elapsed.TotalMilliseconds}ms", org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));
                return InvokeResult.Success;
            }
            else
            {
                _logger.Trace($"[NotificationSender__SendNotification] - Found {contacts.Count}, will continue to send notification - {notification.Name} for device {device.Name} in {repo.Name} repository", org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));
            }

            OrgLocation location = null;
            if (!EntityHeader.IsNullOrEmpty(device.Location))
            {
                location = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
                _logger.Trace($"[NotificationSender__SendNotification] - found location {location.Name} on device, loaded location and will append.", org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));
            }
            else
                _logger.Trace($"[NotificationSender__SendNotification] - No location set on device, can not append location information", org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"));


            var tz = TimeZoneInfo.Local;
            if (device.TimeZone.Id != "UTC")
            {
                tz = _timeZoneService.GetTimeZoneById(device.TimeZone.Id);
            }
            else if (location != null && location.TimeZone.Id != "UTC")
            {
                tz = _timeZoneService.GetTimeZoneById(location.TimeZone.Id);
            }
            else if (!EntityHeader.IsNullOrEmpty(repo.Instance))
            {
                var instance = await _deploymentRepo.GetReadOnlyInstanceAsync(repo.Instance.Id);
                tz = _timeZoneService.GetTimeZoneById(instance.TimeZone.Id);
            }
            else
            {
                var orgInfo = await _orgRepo.GetOrganizationAsync(org.Id);
                if (orgInfo.TimeZone.Id != "UTC")
                {
                    tz = _timeZoneService.GetTimeZoneById(orgInfo.TimeZone.Id);
                }
            }

            var deltaStr = "N/A";
            var lastContactStr = "Never";
            if (!String.IsNullOrEmpty(lastContact))
            {
                var dateTime = lastContact.ToDateTime();
                var delta = DateTime.UtcNow - dateTime;
                deltaStr = delta.ToDescription();

                var lastContactDT = TimeZoneInfo.ConvertTime(lastContact.ToDateTime(), tz);
                lastContactStr = $"{lastContactDT.ToShortDateString()} {lastContactDT.ToShortTimeString()} {tz.Id}";
            }

            var notifId = DateTime.UtcNow.ToInverseTicksRowKey();
            var pageResult = await _landingPageBuilder.PreparePage(notifId, null, notification, testMode, device, location, org, user);
            var page = pageResult.Result;

            if (notification.SendEmail && !String.IsNullOrEmpty(notification.EmailContent))
            {
                notification.EmailContent = notification.EmailContent.Replace("[LastContactTime]", lastContactStr);
                notification.EmailContent = notification.EmailContent.Replace("[TimeSinceLastContact]", deltaStr);
            }

            if (notification.SendEmail && !String.IsNullOrEmpty(notification.EmailSubject))
            {
                notification.EmailSubject = notification.EmailSubject.Replace("[LastContactTime]", lastContactStr);
                notification.EmailSubject = notification.EmailSubject.Replace("[TimeSinceLastContact]", deltaStr);
            }

            if (notification.SendSMS && !String.IsNullOrEmpty(notification.SmsContent))
            {
                notification.SmsContent = notification.SmsContent.Replace("[LastContactTime]", lastContactStr);
                notification.SmsContent = notification.SmsContent.Replace("[TimeSinceLastContact]", deltaStr);
            }

            await _smsSender.PrepareMessage(notification, testMode, device, location);
            await _emailSender.PrepareMessage(notification, testMode, device, location);
            await _deviceCommandSender.PrepareMessage(notification, testMode, device, location);
            contacts.EnsureUniqueNotifications();

            await _raisedNotificationHistoryRepo.AddHistoryAsync(new RaisedNotificationHistory(device.DeviceRepository.Id)
            {
                OrgId = device.OwnerOrganization.Id,
                DeviceId = device.DeviceId,
                DeviceUniqueId = device.Id,
                DeviceRepoId = device.DeviceRepository.Id,
                Notification = notification.Name,
                NotificationId = notification.Id,
                TestMode = testMode,
                TimeStamp = DateTime.UtcNow.ToJSONString()
            });

            foreach (var recipient in contacts)
            {
                var notificationHistory = new DeviceNotificationHistory(device.Id, $"{notifId}-{recipient.Id}")
                {
                    UserId = recipient.Id,
                    UserName = $"{recipient.FirstName} {recipient.LastName}",
                    OrgId = org.Id,
                    Notification = notification.Name,
                    NotificationId = notification.Id,
                    StaticPageId = page.PageId,
                    TestMode = testMode,
                    SentTimeStamp = DateTime.UtcNow.ToJSONString(),
                    SendEmail = notification.SendEmail && recipient.SendEmail,
                    SendSMS = notification.SendSMS && recipient.SendSMS,
                    DeviceId = device.DeviceId,
                    DeviceRepoId = repo.Id,
                    Customer = device.Customer?.Text,
                    CustomerId = device.Customer?.Id,
                };

                if (notificationHistory.SendEmail)
                {
                    notificationHistory.Email = recipient.Email;
                    var sendEmailResult = await _emailSender.SendAsync(notificationHistory.RowKey, notification, recipient, allowSilence, page, org, user);
                    notificationHistory.SentEmail = sendEmailResult.Successful;
                    if (!notificationHistory.SentEmail)
                    {
                        notificationHistory.Errors += $"Email Error: {sendEmailResult.ErrorMessage}; ";
                        _logger.AddCustomEvent(LogLevel.Error, $"[NotificationSender__SendNotification__SendEmail]", $"[NotificationSender__SendNotification__SendEmail] Error sending email to {recipient.Email} - {sendEmailResult.ErrorMessage}",
                            org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"), recipient.Email.ToKVP("email"));
                    }
                }

                if (notificationHistory.SendSMS)
                {
                    notificationHistory.Phone = recipient.Phone;
                    var sendSmsResult = await _smsSender.SendAsync(notificationHistory.RowKey, recipient, page, allowSilence, org, user);
                    notificationHistory.SentSMS = sendSmsResult.Successful;
                    if(!notificationHistory.SentSMS)
                    {
                        notificationHistory.Errors += $"SMS Error: {sendSmsResult.ErrorMessage}; ";
                        _logger.AddCustomEvent(LogLevel.Error, $"[NotificationSender__SendNotification__SendSMS]", $"[NotificationSender__SendNotification__SendSMS] Error sending sms to {recipient.Phone} - {sendSmsResult.ErrorMessage}",
                            org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"), recipient.Phone.ToKVP("phone"));
                    }
                }

                await _notificationTracking.AddHistoryAsync(notificationHistory);
            }

            if(notification.ForwardToParentDevice && !EntityHeader.IsNullOrEmpty(device.ParentDevice))
            {
                await _deviceCommandSender.SendAsync(repo.Instance.Id, device.ParentDevice.Id, org, user);
            }

            foreach (var cot in notification.CotNotifications)
            {
                await _cotSender.SendAsync(cot, device, location, org, user);
            }

            foreach (var mqtt in notification.MqttNotifications)
            {
                await _mqttSender.SendAsync(mqtt, device, location, org, user);
            }

            foreach (var rest in notification.RestNotifications)
            {
                await _restSender.SendAsync(rest, device, location, org, user);
            }

            _logger.AddCustomEvent(LogLevel.Message, $"[NotificationSender__SendNotification]", $"[NotificationSender__SendNotification] - Completed in {sw.Elapsed.TotalMilliseconds}ms",
                testMode.ToString().ToKVP("testMode"),
                notification.Key.ToKVP("key"),
                org.Id.ToKVP("orgId"), device.DeviceId.ToKVP("deviceId"),
                sw.Elapsed.TotalMilliseconds.ToString().ToKVP("ms"),
                _emailSender.EmailsSent.ToString().ToKVP("emailsSent"),
                _smsSender.TextMessagesSent.ToString().ToKVP("textSent"));

            SentNotifications.Inc();

            return InvokeResult.Success;
        }
    }

    public static class NotificationExtensions
    {
        private static string CleanPhoneNumber(string phone)
        {
            if (String.IsNullOrEmpty(phone))
                return String.Empty;

            return phone.Replace("-", "").Replace("(", "").Replace(")", "");
        }

        public static void EnsureUniqueNotifications(this List<NotificationRecipient> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (!String.IsNullOrEmpty(recipient.Email))
                {
                    var distinctEmails = recipients.Where(recip => recip.Email?.ToUpper() == recipient.Email.ToUpper() && recip.NotificationRecipientId != recipient.NotificationRecipientId);
                    foreach (var item in distinctEmails)
                    {
                        item.Email = null;
                    }
                }

                if (!String.IsNullOrEmpty(recipient.Phone))
                {
                    var distinctEmails = recipients.Where(recip => CleanPhoneNumber(recip.Phone) == CleanPhoneNumber(recipient.Phone) && recip.NotificationRecipientId != recipient.NotificationRecipientId);
                    foreach (var item in distinctEmails)
                    {
                        item.Phone = null;
                    }
                }

            }

        }
    }
}
