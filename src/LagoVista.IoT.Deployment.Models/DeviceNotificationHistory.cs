using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
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
