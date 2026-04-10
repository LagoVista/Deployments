using LagoVista.Core;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class DeviceNotificationRecipient 
    {
        public string Id { get; set; }
        public EntityHeader Org { get; set; }
        public EntityHeader User { get; set; }
        public UtcTimestamp CreationDate { get; set; }
        public NormalizedId32 DeviceRepoId { get; set; }
        public NormalizedId32 DeviceUniqueId { get; set; }
        public string NotificationKey { get; set; }
  
        public string NotificationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool SendEmail { get; set;  }
        public bool SendSms { get; set; }
    }
}
