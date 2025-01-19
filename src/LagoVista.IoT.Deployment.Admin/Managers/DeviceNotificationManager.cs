using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceNotificationManager : ManagerBase, IDeviceNotificationManager
    {
        private readonly IDeviceNotificationRepo _deviceNotificationRepo;
        private readonly IDeviceNotificationTracking _notificationTracking;
        private readonly ISecureStorage _secureStorage;
        private readonly IStaticPageStorage _staticPageStorage;
        private readonly IRaisedNotificationHistoryRepo _raisedNotificationHistoryRepo;

        public DeviceNotificationManager(IDeviceNotificationRepo deviceNotificationRepo, IDeviceNotificationTracking notificationTracking,
                ILogger logger,  ISecureStorage secureStorage, IStaticPageStorage staticPageStorage, IRaisedNotificationHistoryRepo raisedNotificationHistoryRepo, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
            base(logger, appConfig, dependencyManager, security)
        {
            _deviceNotificationRepo = deviceNotificationRepo ?? throw new ArgumentNullException(nameof(deviceNotificationRepo));
            _notificationTracking = notificationTracking ?? throw new ArgumentNullException(nameof(notificationTracking));
            _staticPageStorage = staticPageStorage ?? throw new ArgumentNullException(nameof(staticPageStorage));
            _raisedNotificationHistoryRepo = raisedNotificationHistoryRepo ?? throw new ArgumentNullException(nameof(raisedNotificationHistoryRepo));
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
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceNotificationSummary));
            return await _deviceNotificationRepo.GetNotificationForOrgAsync(orgId, listRequest);
        }

        public async Task<ListResponse<DeviceNotificationSummary>> GetNotificationsForCustomerAsync(string orgId, string customerId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceNotificationSummary));
            return await _deviceNotificationRepo.GetNotificationForCustomerAsync(orgId, customerId, listRequest);
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

        public Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _notificationTracking.GetHistoryForRepoAsync(repoId, listRequest);
        }

        public Task<ListResponse<RaisedNotificationHistory>> GetRasiedNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _raisedNotificationHistoryRepo.GetHistoryAsync(deviceid, listRequest);
        }

        public Task<ListResponse<RaisedNotificationHistory>> GetRaisedNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _raisedNotificationHistoryRepo.GetHistoryForRepoAsync(repoId, listRequest);
        }

    }
}