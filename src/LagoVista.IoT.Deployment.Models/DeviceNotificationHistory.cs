﻿using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotificationHistory_Title, DeploymentAdminResources.Names.DeviceNotificationHistory_Description, DeploymentAdminResources.Names.DeviceNotificationHistory_Description,
       EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1")]
    public class DeviceNotificationHistory : TableStorageEntity
    {
        public DeviceNotificationHistory(string deviceId, string rowKey)
        {
            RowKey = rowKey;
            PartitionKey = deviceId;
        }

        public string StaticPageId { get; set; }
        public string DeviceId {  get => PartitionKey;  }
        public string OrgId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SentTimeStamp { get; set; }
        public bool Viewed { get; set; }
        public string ViewedTimeStamp { get; set; }
    
        public bool SentEmail { get; set; }
        public bool SentSMS { get; set; }
    
        public bool TestMode { get; set; }
    }
}
