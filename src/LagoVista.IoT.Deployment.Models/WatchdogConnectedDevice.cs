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
        
        /// <summary>
        /// This is the actual devices id such as [dev0001]
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// This is the unique/guid of the device.
        /// </summary>
        public string DeviceUniqueId { get; set; }

        public string DevicConfigurationId { get; set; }
        public string DeviceConfiguration { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceType { get; set; }
        public string LastNotified { get; set; }
        public bool WatchdogDisabled { get; set; }
    }
}
