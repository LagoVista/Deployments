using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Models.Settings
{
    public class DeviceDataStorageSettings
    {
        public ConnectionSettings DeviceArchiveTableStorage { get; set; }
        public ConnectionSettings DeviceExceptionTableStorage { get; set; }
        public ConnectionSettings DeviceStatusTableStorage { get; set; }
        public ConnectionSettings DeviceMediaTableStorage { get; set; }
        public ConnectionSettings DeviceMediaBlobStorage { get; set; }

    }
}
