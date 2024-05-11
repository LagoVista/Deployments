using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using RingCentral;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceNotificationManager : ManagerBase, IDeviceNotificationManager
    {
        private readonly IDeviceNotificationRepo _deviceNotificationRepo;
        private readonly IDistributionListRepo _distroListRepo;
        private readonly IOrgLocationRepo _orgLocationRepo;
        private readonly IDeviceManager _deviceManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IStaticPageStorage _staticPageStorage;


        public DeviceNotificationManager(IDeviceNotificationRepo deviceNotificationRepo,
                IDistributionListRepo distroListRepo,IOrgLocationRepo orgLocationRepo, IDeviceManager deviceManager, IEmailSender emailSender, ISmsSender smsSender,
                ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : 
            base(logger, appConfig, dependencyManager, security)
        {
            _deviceNotificationRepo = deviceNotificationRepo ?? throw new ArgumentNullException(nameof(deviceNotificationRepo));
            _distroListRepo = distroListRepo ?? throw new ArgumentNullException(nameof(distroListRepo));
            _orgLocationRepo = orgLocationRepo ?? throw new ArgumentNullException(nameof(orgLocationRepo));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        }

        public async Task<InvokeResult> AddNotificationAsync(DeviceNotification notification, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(notification, Actions.Create);
            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Create, user, org);

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

        public Task<InvokeResult> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader)
        {
            throw new NotImplementedException();
        }

        public async Task<InvokeResult> UpdateNotificationAsync(DeviceNotification notification, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(notification, Actions.Create);
            await AuthorizeAsync(notification, AuthorizeResult.AuthorizeActions.Update, user, org);

            await _deviceNotificationRepo.UpdateNotificationAsync(notification);

            return InvokeResult.Success;
        }
    }
}
