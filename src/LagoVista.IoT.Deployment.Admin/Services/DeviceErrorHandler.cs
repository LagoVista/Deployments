using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using Npgsql.NameTranslation;
using RingCentral;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static LagoVista.Core.Networking.Models.uPnPDevice;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceErrorHandler : IDeviceErrorHandler
    {
        private readonly IAdminLogger _adminLogger;
        private readonly IServiceTicketCreator _ticketCreator;
        private readonly IDeviceManager _deviceManager;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IDeviceConfigurationManager _deviceConfigManager;
        private readonly UserAdmin.Interfaces.Managers.IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IDistributionManager _distroManager;
        private readonly IUserManager _userManager;
        private readonly IDeviceErrorCodesManager _errorCodeManager;
        private readonly IDeviceExceptionRepo _exceptionRepo;
        private readonly IIncidentManager _incidentManager;
        private readonly IIncidentProtocolManager _incidentProtocolManager;
        private readonly IDistributionListRepo _distroListRepo;
        private readonly INotificationSender _notificationSender;

        public DeviceErrorHandler(IServiceTicketCreator ticketCreator, IDeviceConfigurationManager deviceConfigManager, IAdminLogger adminLogger, IDeviceManager deviceManager, IDeviceRepositoryManager deviceRepoManager, IDeviceExceptionRepo exceptionRepo,
                                 IIncidentProtocolManager incidentProtocolManager, IIncidentManager incidentManager, IDeviceErrorCodesManager errorCodeManager, IDistributionListRepo distroListRepo, IDistributionManager distroManager, IUserManager userManager, UserAdmin.Interfaces.Managers.IEmailSender emailSender, 
                                 ISmsSender smsSender, INotificationSender notificationSender, IDeviceNotificationManager deviceNotificationManager)
        {
            _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));            
            _ticketCreator = ticketCreator ?? throw new ArgumentNullException(nameof(ticketCreator));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _repoManager = deviceRepoManager ?? throw new ArgumentNullException(nameof(deviceRepoManager));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _deviceConfigManager = deviceConfigManager ?? throw new ArgumentNullException(nameof(deviceConfigManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _distroManager = distroManager ?? throw new ArgumentNullException(nameof(distroManager));
            _errorCodeManager = errorCodeManager ?? throw new ArgumentNullException(nameof(errorCodeManager));
            _exceptionRepo = exceptionRepo ?? throw new ArgumentNullException(nameof(exceptionRepo));
            _incidentManager = incidentManager ?? throw new ArgumentNullException(nameof(incidentManager));
            _incidentProtocolManager = incidentProtocolManager ?? throw new ArgumentNullException(nameof(incidentProtocolManager));
            _distroListRepo = distroListRepo ?? throw new ArgumentNullException(nameof(distroListRepo));
        }


        private async Task<List<NotificationContact>> GetAllContacts(EntityHeader deviceErrorCodeDL, EntityHeader deviceRepoDL, EntityHeader deviceDL)
        {
            var contacts = new List<NotificationContact>();

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCodeDL))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(deviceErrorCodeDL.Id);
                contacts.AddRange(distroList.AppUsers.Select(ap => new NotificationContact { Name = ap.Text, AppUserId = ap.Id }));
                contacts.AddRange(distroList.ExternalContacts.Select(ap => new NotificationContact { Name =  $"{ap.FirstName} {ap.LastName}", Email = ap.SendEmail ? ap.Email.ToLower() : null, Phone = ap.SendSMS ? ap.Phone : null }));
            }

            if (!EntityHeader.IsNullOrEmpty(deviceRepoDL))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(deviceRepoDL.Id);
                contacts.AddRange(distroList.AppUsers.Select(ap => new NotificationContact { Name = ap.Text, AppUserId = ap.Id }));
                contacts.AddRange(distroList.ExternalContacts.Select(ap => new NotificationContact { Name = $"{ap.FirstName} {ap.LastName}", Email = ap.SendEmail ? ap.Email.ToLower() : null, Phone = ap.SendSMS ? ap.Phone : null }));
            }

            if (!EntityHeader.IsNullOrEmpty(deviceRepoDL))
            {
                var distroList = await _distroListRepo.GetDistroListAsync(deviceDL.Id);
                contacts.AddRange(distroList.AppUsers.Select(ap => new NotificationContact { Name = ap.Text, AppUserId = ap.Id }));
                contacts.AddRange(distroList.ExternalContacts.Select(ap => new NotificationContact { Name = $"{ap.FirstName} {ap.LastName}", Email = ap.SendEmail ? ap.Email.ToLower() : null, Phone = ap.SendSMS ? ap.Phone : null }));
            }

            var uniqueAppUsers = contacts.Where(p=>!String.IsNullOrEmpty(p.AppUserId)).GroupBy(p => p.AppUserId).Select(p => p.First()).ToList();
            foreach(var usr in uniqueAppUsers)
            {
                var appUser = await _userManager.FindByIdAsync(usr.AppUserId);
                if(appUser != null)
                {
                    foreach(var cnt in contacts)
                    {
                        if(usr.AppUserId == cnt.AppUserId)
                        {
                            cnt.Email = appUser.Email.ToLower();
                            cnt.Phone = appUser.PhoneNumber;
                        }
                    }
                }
            }

            return contacts
                .GroupBy(p => new { p.Email, p.Phone })
                .Select(g => g.First()).ToList();
        }

        private async Task SendEmailAndSMSNotification(DeviceErrorCode deviceErrorCode, Device device, DeviceException exception, List<NotificationContact> contacts, EntityHeader org, EntityHeader user)
        {
            var subject = String.IsNullOrEmpty(deviceErrorCode.EmailSubject) ? deviceErrorCode.Name : deviceErrorCode.EmailSubject.Replace("[DEVICEID]", device.DeviceId).Replace("[DEVICENAME]", device.Name);


            foreach (var contact in contacts)
            {
                if (deviceErrorCode.SendEmail && !String.IsNullOrEmpty(contact.Email))
                {
                    var body = $"The error  [{deviceErrorCode.Key}] was detected on the device {device.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
                    if (exception.AdditionalDetails.Any())
                    {
                        body += "<br>";
                        body += "<b>Additional Details:<br /><b>";
                        body += "<ul>";
                        foreach (var detail in exception.AdditionalDetails)
                            body += $"<li>{detail}</li>";

                        body += "</ul>";
                    }
                    await _emailSender.SendAsync(contact.Email, subject, body, org, user);
                }

                if (deviceErrorCode.SendSMS && !String.IsNullOrEmpty(contact.Phone))
                {
                    var body = $"Device {device.Name} generated error code [${deviceErrorCode.Key}] {deviceErrorCode.Description} {exception.Details}";
                    await _smsSender.SendAsync(contact.Phone, body);
                }
            }
        }

        private async Task SendDeviceNotification(DeviceErrorCode deviceErrorCode, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DeviceNotification))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Has Device Notification {deviceErrorCode.DeviceNotification.Text}", exception.DeviceId.ToKVP("deviceId"));

                var notification = new RaisedDeviceNotification()
                {
                    DeviceUniqueId = exception.DeviceUniqueId,
                    DeviceRepositoryId = exception.DeviceRepositoryId,
                    NotificationKey = deviceErrorCode.DeviceNotification.Key,
                };

                if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
                {
                    var distroList = await _distroManager.GetListAsync(deviceErrorCode.DistroList.Id, org, user);
                    notification.AdditionalUsers = distroList.AppUsers.Select(usr=>EntityHeader.Create(usr.Id, usr.Key, usr.Text)).ToList();
                    notification.AdditionalExternalContacts = distroList.ExternalContacts;
                }

                if((await _notificationSender.RaiseNotificationAsync(notification, org, user)).Successful) 
                    _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Sent Device Notification {deviceErrorCode.DeviceNotification.Text} {notification.AdditionalUsers.Count} additional users, {notification.AdditionalExternalContacts} additional external contacts", exception.DeviceId.ToKVP("deviceId"));
                else
                    _adminLogger.AddError($"[DeviceErrorHandler__SendDeviceNotification]", $"Did not send notification - {deviceErrorCode.DeviceNotification.Text}", exception.DeviceId.ToKVP("deviceId"));
            }
            else
            {
                _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Device Error Code: {deviceErrorCode.Key} does not have a notification configured, not sending.", exception.DeviceId.ToKVP("deviceId"));
            }
        }

        private async Task<InvokeResult> NotifyAsync(DeviceErrorCode deviceErrorCode, DeviceError deviceError, Device device, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if(!String.IsNullOrEmpty(deviceError.NextNotification) && (deviceError.NextNotification.ToDateTime().ToUniversalTime() > DateTime.UtcNow))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Not sending any notifications, will send notification at {deviceError.NextNotification.ToDateTime()}.", exception.DeviceId.ToKVP("deviceId"));
                return InvokeResult.Success;
            }

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
            {
          //      await SendEmailAndSMSNotification(deviceErrorCode, device, exception, org, user);
            }

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DeviceNotification))
            {
                await SendDeviceNotification(deviceErrorCode, exception, org, user);                
            }
            else
            {
                _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Device Error Code: {deviceErrorCode.Key} does not have a notification configured, not sending.", exception.DeviceId.ToKVP("deviceId"));
            }
                    
            if (EntityHeader.IsNullOrEmpty(deviceErrorCode.NotificationIntervalTimeSpan) || deviceErrorCode.NotificationIntervalTimeSpan.Value == TimeSpanIntervals.NotApplicable)
            {
                deviceError.NextNotification = null;
                _adminLogger.Trace("[DeviceErrorHandler__NotifyAsync] - Interval Time Span is null or set to not applicable.", exception.DeviceId.ToKVP("deviceId"));
            }
            else if (deviceErrorCode.NotificationIntervalQuantity.HasValue && deviceErrorCode.NotificationIntervalTimeSpan.Value != TimeSpanIntervals.NotApplicable)
            {
                deviceError.NextNotification = deviceErrorCode.NotificationIntervalTimeSpan.Value.AddTimeSpan(deviceErrorCode.NotificationIntervalQuantity.Value);
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Scheduled Next Notification for {deviceError.NextNotification}.", exception.DeviceId.ToKVP("deviceId"));
            }
            else
            {
                deviceError.NextNotification = null;
                _adminLogger.AddCustomEvent(Core.PlatformSupport.LogLevel.Error, "[DeviceErrorHandler__NotifyAsync]", "Invalid quantity on NotificationIntervalQuantity", device.DeviceId.ToKVP("deviceId"), exception.ErrorCode.ToKVP("errorCode"));
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Invalid data, will not schedule next notification.", exception.DeviceId.ToKVP("deviceId"));
            }
            
            return InvokeResult.Success;
        }

        private async Task<InvokeResult> CreateIncidentAsync(DeviceException exception, DeviceErrorCode errorCode, List<NotificationContact> contacts)
        {
            await Task.Delay(1);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> HandleDeviceExceptionAsync(DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));
           
            var timeStamp = DateTime.UtcNow.ToJSONString();

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(exception.DeviceRepositoryId, org, user);
            if (repo == null)
            {
                return InvokeResult.FromError($"FSLite - Handle Device Exception - Could not find repo for: {exception.DeviceUniqueId}");
            }

            await _exceptionRepo.AddDeviceExceptionAsync(repo, exception);

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Repo: {repo.Name} - {repo.OwnerOrganization.Text}", exception.DeviceId.ToKVP("deviceId"));

            var device = await _deviceManager.GetDeviceByIdAsync(repo, exception.DeviceUniqueId, org, user);
            if (device == null)
            {
                return InvokeResult.FromError($"FSLite - Handle Device Exception - Could not find device for: {exception.DeviceUniqueId}");
            }

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Device: {device.Result.Name} - {device.Result.OwnerOrganization.Text}", exception.DeviceId.ToKVP("deviceId"));

            var deviceConfig = await _deviceConfigManager.GetDeviceConfigurationAsync(device.Result.DeviceConfiguration.Id, org, user);

            if (deviceConfig == null)
            {
                return InvokeResult.FromError($"DeviceErrorHandler - Handle Device Exception - Could not find device configuration: {device.Result.DeviceConfiguration.Text}");
            }

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Device Configuration: {deviceConfig.Name} - {deviceConfig.OwnerOrganization.Text}", exception.DeviceId.ToKVP("deviceId"));

            var deviceErrorCode = deviceConfig.ErrorCodes.FirstOrDefault(err => err.Key == exception.ErrorCode);
            if (deviceErrorCode == null)
            {
                deviceErrorCode = await _errorCodeManager.GetErrorCodeByKeyAsync(exception.ErrorCode, org, user);
                if (deviceErrorCode == null)
                {
                    return InvokeResult.FromError($"DeviceErrorHandler - Could not find error code [{exception.ErrorCode}] on device configuration [{deviceConfig.Name}], nor in organization for device [{device.Result.Name}]");
                }
            }

            if(!EntityHeader.IsNullOrEmpty(deviceErrorCode.IncidentProtocol))
            {
                var protocol = await _incidentProtocolManager.GetIncidentProtocolAsync(deviceErrorCode.IncidentProtocol.Id, org, user);
                if (protocol != null)
                {
                    var incident = new Incident()
                    {
                        OpenedTimeStamp = timeStamp,
                    };
                }
            }

            var contacts = await GetAllContacts(deviceErrorCode.DistroList, repo.DistroList, device.Result.DistributionList);
            
            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Device Error Code: {deviceErrorCode.Name}", exception.DeviceId.ToKVP("deviceId"));

            var deviceError = device.Result.Errors.Where(err => err.DeviceErrorCode == exception.ErrorCode).FirstOrDefault();

            if (deviceError == null)
            {
                deviceError = new DeviceError()
                {
                    Count = 1,                   
                    DeviceErrorCode = exception.ErrorCode,
                    FirstSeen = timeStamp,
                    ErrorMessage = deviceErrorCode.Name,
                    ErrorDescription = deviceErrorCode.Description,
                    LastSeen = timeStamp,
                    LastDetails = exception.Details,
                    Timestamp = timeStamp,
                };

                device.Result.Errors.Add(deviceError);

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Does not have existing error on device: {deviceError.DeviceErrorCode}, creating device error.", exception.DeviceId.ToKVP("deviceId"));
            }
            else
            {
                deviceError.Count++;
                deviceError.Timestamp = timeStamp;
                deviceError.LastSeen = timeStamp;
                deviceError.ErrorDescription = deviceErrorCode.Description;

                if (!deviceError.Active)
                    deviceError.FirstSeen = timeStamp;

                if (deviceErrorCode.AutoexpireTimespanQuantity.HasValue)
                {
                    deviceError.Expires = deviceErrorCode.AutoexpireTimespan.Value.AddTimeSpan(deviceErrorCode.AutoexpireTimespanQuantity.Value);
                }

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Has existing device error on device {deviceError.DeviceErrorCode}, incrementing count - {deviceError.Count}.", exception.DeviceId.ToKVP("deviceId"));
            }

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.ServiceTicketTemplate))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync]- Generating service ticket (every occurrence.");
                var request = new CreateServiceTicketRequest()
                {
                    TemplateId = deviceErrorCode.ServiceTicketTemplate.Id,
                    DeviceId = device.Result.DeviceId,
                    Details = exception.Details,
                    DontCreateIfOpenForDevice = !deviceErrorCode.TriggerOnEachOccurrence,
                    RepoId = repo.Id
                };

                var result = await _ticketCreator.CreateServiceTicketAsync(request, org, user);
                if (!result.Successful) return result.ToInvokeResult();
            }
            else
            {
                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync]- No Service Ticket Template - will not generate ticket .");
            }

            await NotifyAsync(deviceErrorCode, deviceError, device.Result, exception, org, user);

            var errorCollection = device.Result.Errors;
            var sw = Stopwatch.StartNew();
            device = await _deviceManager.GetDeviceByIdAsync(repo, exception.DeviceUniqueId, org, user);
            device.Result.Errors = errorCollection;
            await _deviceManager.UpdateDeviceAsync(repo, device.Result, org, user);
            
            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Completed Error Code Processing - Error Count {device.Result.Errors.Count}, full update in {sw.Elapsed.TotalMilliseconds}ms", exception.DeviceId.ToKVP("deviceId"));

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> ClearDeviceExceptionAsync(DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(exception.DeviceRepositoryId, org, user);
            var device = await _deviceManager.GetDeviceByIdAsync(repo, exception.DeviceUniqueId, org, user);
            var deviceConfig = await _deviceConfigManager.GetDeviceConfigurationAsync(device.Result.DeviceConfiguration.Id, org, user);

            var deviceErrorCode = deviceConfig.ErrorCodes.FirstOrDefault(err => err.Key == exception.ErrorCode);
            if (deviceErrorCode == null)
            {
                deviceErrorCode = await _errorCodeManager.GetErrorCodeByKeyAsync(exception.ErrorCode, org, user);
                if (deviceErrorCode == null)
                {
                    return InvokeResult.FromError($"Could not find error code [{exception.ErrorCode}] on device configuration [{deviceConfig.Name}] for device [{device.Result.Name}]");
                }
            }

            _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Clear Device Exception {exception.ErrorCode}");

            var deviceError = device.Result.Errors.FirstOrDefault(err => err.DeviceErrorCode == exception.ErrorCode);
            if (deviceError != null) 
            {
                device.Result.Errors.Remove(deviceError);
                await _deviceManager.UpdateDeviceAsync(repo, device.Result, org, user);
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Removed Device Exception {exception.ErrorCode}, error count: {device.Errors.Count}", exception.DeviceId.ToKVP("deviceId"));
            }
            else
            {
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Could not find existing error to remove from device [{device.Result.Name}] code: [{exception.ErrorCode}]", exception.DeviceId.ToKVP("deviceId"));
            }
            
            await _exceptionRepo.AddDeviceExceptionClearedAsync(repo, exception);

            await _ticketCreator.ClearDeviceExceptionAsync(exception, org, user);

            if (deviceErrorCode.NotifyOnClear)
            {
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Will notify on clear {exception.ErrorCode}", exception.DeviceId.ToKVP("deviceId"));

                var appUsers = new List<EntityHeader>();
                var externalContacts = new List<ExternalContact>();

                if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
                {
                    var distroList = await _distroManager.GetListAsync(deviceErrorCode.DistroList.Id, org, user);
                    appUsers.AddRange(distroList.AppUsers);
                    externalContacts.AddRange(distroList.ExternalContacts);
                }

                if(!EntityHeader.IsNullOrEmpty(device.Result.DistributionList))
                {
                    var distroList = await _distroManager.GetListAsync(device.Result.DistributionList.Id, org, user);
                    appUsers.AddRange(distroList.AppUsers);
                    externalContacts.AddRange(distroList.ExternalContacts);
                }

                var subject = $"Error {deviceErrorCode.Name} cleared -" + (String.IsNullOrEmpty(deviceErrorCode.EmailSubject) ? deviceErrorCode.Name : deviceErrorCode.EmailSubject.Replace("[DEVICEID]", device.Result.DeviceId).Replace("[DEVICENAME]", device.Result.Name));

                foreach (var notificationUser in appUsers)
                {
                    var appUser = await _userManager.FindByIdAsync(notificationUser.Id);
                    if (deviceErrorCode.SendEmail)
                    {
                        var body = $"The error {deviceErrorCode.Name} was cleared on the device {device.Result.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
                        await _emailSender.SendAsync(appUser.Email, subject, body, org, user);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent Email to AppUser {appUser.Email}", exception.DeviceId.ToKVP("deviceId"));
                    }

                    if (deviceErrorCode.SendSMS)
                    {
                        var body = $"{device.Result.Name} reesolved {deviceErrorCode.Name} {deviceErrorCode.Description} {exception.Details}";
                        await _smsSender.SendAsync(appUser.PhoneNumber, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent SMS to AppUser {appUser.PhoneNumber}", exception.DeviceId.ToKVP("deviceId"));
                    }
                }

                foreach (var externalContact in externalContacts)
                {
                    if (externalContact.SendEmail)
                    {
                        var body = $"[{deviceErrorCode.Name}] resolved error {device.Result.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
                        await _emailSender.SendAsync(externalContact.Email, subject, body, org, user);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent Email to External Contact {externalContact.Email}", exception.DeviceId.ToKVP("deviceId"));
                    }

                    if (externalContact.SendSMS)
                    {
                        var body = $"{device.Result.Name} resolved error {deviceErrorCode.Name} {deviceErrorCode.Description} {exception.Details}";
                        await _smsSender.SendAsync(externalContact.Phone, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent SMS to External Contact {externalContact.Phone}", exception.DeviceId.ToKVP("deviceId"));
                    }
                }
            }

            _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Completed Processing");

            return InvokeResult.Success;
        }
    }
}
