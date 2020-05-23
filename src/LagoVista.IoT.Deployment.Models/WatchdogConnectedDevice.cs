using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class WatchdogConnectedDevice
    {
        public string Id { get; set; }
        public string LastContact { get; set; }
        public int TimeoutSeconds { get; set; }
        public int OverdueSeconds { get; set; }
        public string Expired { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceConfiguration { get; set; }
        public string DeviceType { get; set; }
        public string LastNotified { get; set; }
    }
}
