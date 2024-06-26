﻿using LagoVista.Core;
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
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceErrorHandler : IDeviceErrorHandler
    {
        private readonly IAdminLogger _adminLogger;
        private readonly IServiceTicketCreator _ticketCreator;
        private readonly IDeviceManager _deviceManager;
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IDeviceConfigurationManager _deviceConfigManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IDistributionManager _distroManager;
        private readonly IUserManager _userManager;
        private readonly IDeviceNotificationManager _deviceNotificationManager;
        private readonly IDeviceErrorCodesManager _errorCodeManager;
        private readonly IDeviceExceptionRepo _exceptionRepo;


        public DeviceErrorHandler(IServiceTicketCreator ticketCreator, IDeviceConfigurationManager deviceConfigManager, IAdminLogger adminLogger, IDeviceManager deviceManager, IDeviceRepositoryManager deviceRepoManager, IDeviceExceptionRepo exceptionRepo,
                                  IDeviceErrorCodesManager errorCodeManager, IDistributionManager distroManager, IUserManager userManager, IEmailSender emailSender, ISmsSender smsSender, IDeviceNotificationManager deviceNotificationManager)
        {
            _ticketCreator = ticketCreator ?? throw new ArgumentNullException(nameof(ticketCreator));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _repoManager = deviceRepoManager ?? throw new ArgumentNullException(nameof(deviceRepoManager));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _deviceConfigManager = deviceConfigManager ?? throw new ArgumentNullException(nameof(deviceConfigManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _distroManager = distroManager ?? throw new ArgumentNullException(nameof(distroManager));
            _deviceNotificationManager = deviceNotificationManager ?? throw new ArgumentNullException(nameof(deviceNotificationManager));
            _errorCodeManager = errorCodeManager ?? throw new ArgumentNullException(nameof(errorCodeManager));
            _exceptionRepo = exceptionRepo ?? throw new ArgumentNullException(nameof(exceptionRepo));
        }

        private async Task SendEmailAndSMSNotification(DeviceErrorCode deviceErrorCode, Device device, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            var subject = String.IsNullOrEmpty(deviceErrorCode.EmailSubject) ? deviceErrorCode.Name : deviceErrorCode.EmailSubject.Replace("[DEVICEID]", device.DeviceId).Replace("[DEVICENAME]", device.Name);

            var appUsers = new List<EntityHeader>();
            var externalContacts = new List<ExternalContact>();

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
            {
                var distroList = await _distroManager.GetListAsync(deviceErrorCode.DistroList.Id, org, user);
                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);
            }

            if(!EntityHeader.IsNullOrEmpty(device.DistributionList))
            {
                var distroList = await _distroManager.GetListAsync(device.DistributionList.Id, org, user);
                appUsers.AddRange(distroList.AppUsers);
                externalContacts.AddRange(distroList.ExternalContacts);
            }

            foreach (var notificationUser in appUsers)
            {
                var appUser = await _userManager.FindByIdAsync(notificationUser.Id);
                if (deviceErrorCode.SendEmail)
                {
                    var body = $"The error [{deviceErrorCode.Name}] was detected on the device {device.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
                    if (exception.AdditionalDetails.Any())
                    {
                        body += "<br>";
                        body += "<b>Additional Details:<br /><b>";
                        body += "<ul>";
                        foreach (var detail in exception.AdditionalDetails)
                            body += $"<li>{detail}</li>";

                        body += "</ul>";
                    }
                    await _emailSender.SendAsync(appUser.Email, subject, body);
                }

                if (deviceErrorCode.SendSMS && String.IsNullOrEmpty(appUser.PhoneNumber))
                {
                    var body = $"Device {device.Name} generated error code [${deviceErrorCode.Key}] {deviceErrorCode.Description} {exception.Details}";
                    await _smsSender.SendAsync(appUser.PhoneNumber, body);
                }
            }

            foreach (var notificationUser in externalContacts)
            {
                if (deviceErrorCode.SendEmail && notificationUser.SendSMS)
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
                    await _emailSender.SendAsync(notificationUser.Email, subject, body);
                }

                if (deviceErrorCode.SendSMS && notificationUser.SendSMS)
                {
                    var body = $"Device {device.Name} generated error code [${deviceErrorCode.Key}] {deviceErrorCode.Description} {exception.Details}";
                    await _smsSender.SendAsync(notificationUser.Phone, body);
                }
            }
        }

        private async Task SendDeviceNotification(DeviceErrorCode deviceErrorCode, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DeviceNotification))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Has Device Notification {deviceErrorCode.DeviceNotification.Text}");

                var notification = new RaisedDeviceNotification()
                {
                    DeviceUniqueId = exception.DeviceUniqueId,
                    DeviceRepositoryId = exception.DeviceRepositoryId,
                    NotificationKey = deviceErrorCode.DeviceNotification.Key,
                };

                if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
                {
                    var distroList = await _distroManager.GetListAsync(deviceErrorCode.DistroList.Id, org, user);
                    notification.AdditionalUsers = distroList.AppUsers;
                    notification.AdditionalExternalContacts = distroList.ExternalContacts;
                }

                if((await _deviceNotificationManager.RaiseNotificationAsync(notification, org, user)).Successful) 
                    _adminLogger.Trace($"[DeviceErrorHandler__SendDeviceNotification] - Sent Device Notification {deviceErrorCode.DeviceNotification.Text}");
                else
                    _adminLogger.AddError($"[DeviceErrorHandler__SendDeviceNotification]", $"Did not send notification - {deviceErrorCode.DeviceNotification.Text}", exception.DeviceId.ToKVP("deviceId"));
            }
        }

        private async Task<InvokeResult> NotifyAsync(DeviceErrorCode deviceErrorCode, DeviceError deviceError, Device device, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if(!String.IsNullOrEmpty(deviceError.NextNotification) && (deviceError.NextNotification.ToDateTime().ToUniversalTime() > DateTime.UtcNow))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Not sending any notifiations, will send notification at {deviceError.NextNotification.ToDateTime()}.");
                return InvokeResult.Success;
            }

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DistroList))
            {
                await SendEmailAndSMSNotification(deviceErrorCode, device, exception, org, user);
            }

            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DeviceNotification))
            {
                await SendDeviceNotification(deviceErrorCode, exception, org, user);
            }
                    
            if (EntityHeader.IsNullOrEmpty(deviceErrorCode.NotificationIntervalTimeSpan) || deviceErrorCode.NotificationIntervalTimeSpan.Value == TimeSpanIntervals.NotApplicable)
            {
                deviceError.NextNotification = null;
                _adminLogger.Trace("[DeviceErrorHandler__NotifyAsync] - Interval Time Span is null or set to not applicable.");
            }
            else if (deviceErrorCode.NotificationIntervalQuantity.HasValue && deviceErrorCode.NotificationIntervalTimeSpan.Value != TimeSpanIntervals.NotApplicable)
            {
                deviceError.NextNotification = deviceErrorCode.NotificationIntervalTimeSpan.Value.AddTimeSpan(deviceErrorCode.NotificationIntervalQuantity.Value);
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Scheduled Next Notification for {deviceError.NextNotification}.");
            }
            else
            {
                deviceError.NextNotification = null;
                _adminLogger.AddCustomEvent(Core.PlatformSupport.LogLevel.Error, "[DeviceErrorHandler__NotifyAsync]", "Invalid quantity on NotificationIntervalQuantity", device.DeviceId.ToKVP("deviceId"), exception.ErrorCode.ToKVP("errorCode"));
                _adminLogger.Trace($"[DeviceErrorHandler__NotifyAsync] - Invalid data, will not schedule next notification.");
            }
            
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

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Repo: {repo.Name} - {repo.OwnerOrganization.Text}");

            var device = await _deviceManager.GetDeviceByIdAsync(repo, exception.DeviceUniqueId, org, user);
            if (device == null)
            {
                return InvokeResult.FromError($"FSLite - Handle Device Exception - Could not find device for: {exception.DeviceUniqueId}");
            }

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Device: {device.Result.Name} - {device.Result.OwnerOrganization.Text}");

            var deviceConfig = await _deviceConfigManager.GetDeviceConfigurationAsync(device.Result.DeviceConfiguration.Id, org, user);

            if (deviceConfig == null)
            {
                return InvokeResult.FromError($"DeviceErrorHandler - Handle Device Exception - Could not find device configuration: {device.Result.DeviceConfiguration.Text}");
            }

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Handle Device Exception, Device Configuration: {deviceConfig.Name} - {deviceConfig.OwnerOrganization.Text}");

            var deviceErrorCode = deviceConfig.ErrorCodes.FirstOrDefault(err => err.Key == exception.ErrorCode);
            if (deviceErrorCode == null)
            {
                deviceErrorCode = await _errorCodeManager.GetErrorCodeByKeyAsync(exception.ErrorCode, org, user);
                if (deviceErrorCode == null)
                {
                    return InvokeResult.FromError($"DeviceErrorHandler - Could not find error code [{exception.ErrorCode}] on device configuration [{deviceConfig.Name}], nor in organization for device [{device.Result.Name}]");
                }
            }

            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Device Error Code: {deviceErrorCode.Name}");

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

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Does not have existing error on device: {deviceError.DeviceErrorCode}, creating device error.");
            }
            else
            {
                deviceError.Count++;
                deviceError.Timestamp = timeStamp;
                deviceError.LastSeen = timeStamp;

                if (!deviceError.Active)
                    deviceError.FirstSeen = timeStamp;

                if (deviceErrorCode.AutoexpireTimespanQuantity.HasValue)
                {
                    deviceError.Expires = deviceErrorCode.AutoexpireTimespan.Value.AddTimeSpan(deviceErrorCode.AutoexpireTimespanQuantity.Value);
                }

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Has exisiting device error on device {deviceError.DeviceErrorCode}, incrementing count - {deviceError.Count}.");
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
                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync]- No Service Ticket Template - will not genreate ticket .");
            }

            await NotifyAsync(deviceErrorCode, deviceError, device.Result, exception, org, user);

            var errorCollection = device.Result.Errors;
            var sw = Stopwatch.StartNew();
            device = await _deviceManager.GetDeviceByIdAsync(repo, exception.DeviceUniqueId, org, user);
            device.Result.Errors = errorCollection;
            await _deviceManager.UpdateDeviceAsync(repo, device.Result, org, user);
            
            _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Completed Error Code Processing - Error Count {device.Result.Errors.Count}, full update in {sw.Elapsed.TotalMilliseconds}ms");

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
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Removed Device Exception {exception.ErrorCode}, error count: {device.Errors.Count}");
            }
            else
            {
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Could not find existing error to remove from device [{device.Result.Name}] code: [{exception.ErrorCode}]");
            }
            
            await _exceptionRepo.AddDeviceExceptionClearedAsync(repo, exception);

            await _ticketCreator.ClearDeviceExceptionAsync(exception, org, user);

            if (deviceErrorCode.NotifyOnClear)
            {
                _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Will notify on clear {exception.ErrorCode}");

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
                        await _emailSender.SendAsync(appUser.Email, subject, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent Email to AppUser {appUser.Email}");
                    }

                    if (deviceErrorCode.SendSMS)
                    {
                        var body = $"{device.Result.Name} reesolved {deviceErrorCode.Name} {deviceErrorCode.Description} {exception.Details}";
                        await _smsSender.SendAsync(appUser.PhoneNumber, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent SMS to AppUser {appUser.PhoneNumber}");
                    }
                }

                foreach (var externalContact in externalContacts)
                {
                    if (externalContact.SendEmail)
                    {
                        var body = $"[{deviceErrorCode.Name}] resolved error {device.Result.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
                        await _emailSender.SendAsync(externalContact.Email, subject, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent Email to External Contact {externalContact.Email}");
                    }

                    if (externalContact.SendSMS)
                    {
                        var body = $"{device.Result.Name} resolved error {deviceErrorCode.Name} {deviceErrorCode.Description} {exception.Details}";
                        await _smsSender.SendAsync(externalContact.Phone, body);
                        _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Sent SMS to External Contact {externalContact.Phone}");
                    }
                }
            }

            _adminLogger.Trace($"[DeviceErrorHandler__ClearDeviceExceptionAsync] - Completed Processing");

            return InvokeResult.Success;
        }
    }
}
