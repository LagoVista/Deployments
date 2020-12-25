namespace LagoVista.IoT.Deployment.Models
{
    public class WatchdogMessageStatus
    {
        public string Id { get; set; }
        public string LastReceived { get; set; }
        public int TimeoutSeconds { get; set; }
        public int OverdueSeconds { get; set; }
        public string Expired { get; set; }
        public string MessageName { get; set; }
        public string MessageId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DevicConfigurationId { get; set; }
        public string DeviceConfiguration { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceType { get; set; }
        public string LastNotified { get; set; }
    }
}
