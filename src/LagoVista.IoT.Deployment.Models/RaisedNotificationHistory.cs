using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotificationHistory_Title, DeploymentAdminResources.Names.DeviceNotificationHistory_Description, DeploymentAdminResources.Names.DeviceNotificationHistory_Description,
       EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1")]
    public class RaisedNotificationHistory : TableStorageEntity
    {
        public RaisedNotificationHistory(string deviceRepoId)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
            PartitionKey = deviceRepoId;
        }

        public string TimeStamp { get; set; }

        public string Notification { get; set; }
        public string NotificationId { get; set; }

        public string DeviceUniqueId { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceId { get; set; }
        public string OrgId { get; set; }    
        public bool TestMode { get; set; }
    }
}
