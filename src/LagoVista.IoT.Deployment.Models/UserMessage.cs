// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: de23d87e534b8a722c46683829a02f89ead15f96e2779ed7b44ffe25cb161f7d
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum MessageTypes
    {
        Email,
        SMS,
        SMSAndEmail
    }

    public class UserMessage
    {
        public string UserId { get; set; }
        public string DistributionGroupId { get; set; }
        public MessageTypes MessageType { get; set; }

        public string DeviceId { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Body { get; set; }
        public string Link { get; set; }
        public string Subject { get; set; }

    }
}
