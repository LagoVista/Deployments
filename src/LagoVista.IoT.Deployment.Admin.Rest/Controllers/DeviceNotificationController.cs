using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class DeviceNotificationController : LagoVistaBaseController
    {
        IDeviceNotificationManager _notificationManager;

        public DeviceNotificationController(IDeviceNotificationManager notificationManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _notificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));   
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPost("/api/notification")]
        public Task<InvokeResult> AddnotificationAsync([FromBody] DeviceNotification notification)
        {
            return _notificationManager.AddNotificationAsync(notification, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [HttpPut("/api/notification")]
        public Task<InvokeResult> UpdatenotificationAsync([FromBody] DeviceNotification notification)
        {
            SetUpdatedProperties(notification);
            return _notificationManager.UpdateNotificationAsync(notification, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Get Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/notification/{id}")]
        public async Task<DetailResponse<DeviceNotification>> GetnotificationAsync(string id)
        {
            var container = await _notificationManager.GetNotificationAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeviceNotification>.Create(container);
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/notification/factory")]
        public DetailResponse<DeviceNotification> CreateNewnotification()
        {
            var notification = DetailResponse<DeviceNotification>.Create();
            SetAuditProperties(notification.Model);
            SetOwnedProperties(notification.Model);
            return notification;
        }

        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/notifications")]
        public Task<ListResponse<DeviceNotificationSummary>> GetnotificationsForOrg()
        {
            return _notificationManager.GetNotificationsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Delete Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/notification/{id}")]
        public async Task<InvokeResult> DeletenotificationAsync(string id)
        {
            return await _notificationManager.DeleteNotificationAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}
