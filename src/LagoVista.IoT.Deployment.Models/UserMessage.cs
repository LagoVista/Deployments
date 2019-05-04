﻿using System;
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
        public MessageTypes MessageType { get; set; }

        public string Body { get; set; }
        public string Subject { get; set; }
    }
}