using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using RingCentral;
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
        public bool DryRun { get; set; }

        public string CustomerId { get; set; }
        public string Customer { get; set; }

        public string SmsText { get; set; }

        public string EmailText { get; set; }

        public bool Acknowledged { get; set; }
        public string AcknowledgedByUserId { get; set; }
        public string AcknowledgedByUser { get; set; }
        public string AcknowledgedTimeStamp { get; set; }

        public bool Cleared { get; set; }
        public string ClearedByUserId { get; set; }
        public string ClearedByUser { get; set; }
        public string ClearedTimeStamp { get; set; }


        public bool Escalated { get; set; }
        public bool EscalatedNotificationId { get; set; }
        public bool EscalatedBySystem { get; set; }
        public string EscalatedByUserId { get; set; }
        public string EscalatedByUser { get; set; }
        public string EscalationTimeStamp { get; set; }

    }

    public class RaiseNotificationSummary
    {
        public string TimeStamp { get; set; }

        public EntityHeader Device { get; set; }
        public EntityHeader Notification { get; set; }
        public EntityHeader Customer { get; set; }

        public EntityHeader ClearedByUser { get; set; }
        public string ClearedTimeStamp { get; set; }

        public EntityHeader AcknowledgedByUser { get; set; }
        public string AcknowledgedTimeStamp { get; set; }
        public bool TestMode { get; set; }
        public bool DryRun { get; set; }
        public string EmailContent { get; set; }
        public string SmsContent { get; set; }

        public static RaiseNotificationSummary Create(RaisedNotificationHistory notification)
        {
            var summary = new RaiseNotificationSummary()
            {
                TimeStamp = notification.TimeStamp,
                Notification = EntityHeader.Create(notification.NotificationId, notification.Notification),
                SmsContent = notification.SmsText,
                EmailContent = notification.EmailText,                 
            };

            if(!String.IsNullOrEmpty(notification.Customer) && !String.IsNullOrEmpty(notification.CustomerId))
            {
                summary.Customer = EntityHeader.Create(notification.CustomerId, notification.Customer);
            }

            if(notification.Acknowledged)
            {
                summary.AcknowledgedByUser = EntityHeader.Create(notification.AcknowledgedByUserId, notification.AcknowledgedByUser);
                summary.AcknowledgedTimeStamp = notification.AcknowledgedTimeStamp;
            }

            if(notification.Cleared)
            {
                summary.ClearedByUser = EntityHeader.Create(notification.ClearedByUserId, notification.ClearedByUser);
                summary.ClearedTimeStamp = notification.ClearedTimeStamp;
            }

            return summary;
        }
    }

    public class SentNotification
    {

        public string Email { get; set; }
        public string Phone { get; set; }
        public bool SentEmail { get; set; }
        public bool SentSms { get; set; }
        public string Errors { get; set; }
        public bool Viewed { get; set; }
        public string ViewedTimeStamp { get; set; }
        public EntityHeader ForwardDevice { get; set; }

        public static SentNotification Create(DeviceNotificationHistory history)
        {
            return new SentNotification()
            {
                Email = history.Email,
                Phone = history.Phone,
                SentEmail = history.SentEmail,
                SentSms = history.SentSMS,
                Errors = history.Errors,
                Viewed = history.Viewed,
                ViewedTimeStamp = history.ViewedTimeStamp,
            };
        }
    }
}
