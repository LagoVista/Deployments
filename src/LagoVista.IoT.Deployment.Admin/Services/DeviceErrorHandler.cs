using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using System;
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


        public DeviceErrorHandler(IServiceTicketCreator ticketCreator, IDeviceConfigurationManager deviceConfigManager, IAdminLogger adminLogger, IDeviceManager deviceManager, IDeviceRepositoryManager deviceRepoManager,
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
        }


        private async Task SendEmailAndSMSNotification(DeviceErrorCode deviceErrorCode, Device device, DeviceException exception, EntityHeader org, EntityHeader user)
        {


            var result = await _distroManager.GetListAsync(deviceErrorCode.DistroList.Id, org, user);
            var subject = String.IsNullOrEmpty(deviceErrorCode.EmailSubject) ? deviceErrorCode.Name : deviceErrorCode.EmailSubject.Replace("[DEVICEID]", device.DeviceId).Replace("[DEVICENAME]", device.Name);

            foreach (var notificationUser in result.AppUsers)
            {
                var appUser = await _userManager.FindByIdAsync(notificationUser.Id);
                if (deviceErrorCode.SendEmail)
                {
                    var body = $"The error code [{deviceErrorCode.Key}] was detected on the device {device.Name}<br>{deviceErrorCode.Description}<br>{exception.Details}";
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

                if (deviceErrorCode.SendSMS)
                {
                    var body = $"Device {device.Name} generated error code [${deviceErrorCode.Key}] {deviceErrorCode.Description} {exception.Details}";
                    await _smsSender.SendAsync(appUser.PhoneNumber, body);
                }
            }             
        }

        private async Task SendDeviceNotification(DeviceErrorCode deviceErrorCode, DeviceException exception, EntityHeader org, EntityHeader user)
        {
            if (!EntityHeader.IsNullOrEmpty(deviceErrorCode.DeviceNotification))
            {
                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Has Device Notification {deviceErrorCode.DeviceNotification.Text}");

                await _deviceNotificationManager.RaiseNotificationAsync(new RaisedDeviceNotification()
                {
                    DeviceId = exception.DeviceUniqueId,
                    DeviceRepositoryId = exception.DeviceRepositoryId,
                    NotificationKey = deviceErrorCode.DeviceNotification.Key,
                }, org, user);

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Sent Device Notification {deviceErrorCode.DeviceNotification.Text}");
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

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(exception.DeviceRepositoryId, org, user);
            if (repo == null)
            {
                return InvokeResult.FromError($"FSLite - Handle Device Exception - Could not find repo for: {exception.DeviceUniqueId}");
            }

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
                    FirstSeen = DateTime.UtcNow.ToJSONString(),
                    LastDetails = exception.Details,
                    Timestamp = DateTime.UtcNow.ToJSONString(),
                };

                if (deviceErrorCode.AutoexpireTimespanQuantity.HasValue)
                {
                    deviceError.Expires = deviceErrorCode.AutoexpireTimespan.Value.AddTimeSpan(deviceErrorCode.AutoexpireTimespanQuantity.Value);
                }

                device.Result.Errors.Add(deviceError);

                _adminLogger.Trace($"[DeviceErrorHandler__HandleDeviceExceptionAsync] - Does not have existing error on device: {deviceError.DeviceErrorCode}, creating device error.");
            }
            else
            {
                deviceError.Count++;
                deviceError.Timestamp = DateTime.UtcNow.ToJSONString();

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

        public Task<InvokeResult> ClearDeviceExceptionAsync(DeviceException deviceException, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }
    }
}
