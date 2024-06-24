using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [ConfirmedUser]
    public class DeviceNotificationController : LagoVistaBaseController
    {
        IDeviceNotificationManager _notificationManager;
        ILocationDiagramRepo _locationDiagramRepo;

        public DeviceNotificationController(IDeviceNotificationManager notificationManager, ILocationDiagramRepo locationDiagramRepo, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _notificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
            _locationDiagramRepo = locationDiagramRepo ?? throw new ArgumentNullException(nameof(locationDiagramRepo));
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

        [HttpGet("/api/notifications/{repoid}/{deviceuniqueid}/{notificationkey}")]
        public Task<InvokeResult> TestSendAsync(string repoid, string deviceuniqueid, string notificationkey, string testing = "true")
        {
            return _notificationManager.RaiseNotificationAsync(new RaisedDeviceNotification()
            {
                TestMode = testing == "true",
                DeviceUniqueId = deviceuniqueid,
                DeviceRepositoryId = repoid,
                NotificationKey = notificationkey
            }, OrgEntityHeader, UserEntityHeader);
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


        [HttpGet("/api/device/notification/{deviceid}/history")]
        public async Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistory(string deviceid)
        {
            return await _notificationManager.GetNotificationHistoryAsync(deviceid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        private string GetMessage(string payload)
        {
            var html = @"<html>
<head>
  <link rel=""icon"" type=""image/png"" href=""https://nuviot.blob.core.windows.net/cdn/sa/favicon.png"">
  <link href=""https://nuviot.blob.core.windows.net/cdn/sa/style.css"" rel=""stylesheet"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
</head>
<body>
<div class=""header""></div>" +
payload +
@"</body>
</html>";

            return html;
        }

        [AllowAnonymous]
        [HttpGet("/device/notifications/{notifid}/{orgid}/{recipientid}/acknowledge")]
        public async Task<ActionResult> AcknowledgAsync(string notifid, string orgid, string recipientid, string pageid)
        {
            var result = await _notificationManager.AcknowledgeNotificationAsync(notifid, recipientid);
            if (result.Successful)
            {
                var content = Content(GetMessage("Thank you for acknowledging the notification."));
                content.ContentType = "text/html";
                return content;
            }
            else
                return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("/device/notifications/{notifid}/{orgid}/{recipientid}/{pageid}")]
        public  async Task<ActionResult> GetNotificationPage(string notifid, string orgid, string recipientid, string pageid)
        {
            var result = await _notificationManager.HandleNotificationAsync(notifid, orgid, recipientid, pageid);
            if (result.Successful)
            {
                var content = Content(GetMessage(result.Result));
                content.ContentType = "text/html";
                return content;
            } else
                return NotFound();
        }


        [AllowAnonymous]
        [HttpGet("/device/notifications/diagram/{diagramid}")]
        public async Task<LocationDiagram> GetLocationDiagram(string diagramid)
        {
            return await _locationDiagramRepo.GetLocationDiagramAsync(diagramid);
        }
    }
}
