// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b0f4974d8e19d817dedc25c7830c3e7f5394aec513d94800a0c826c07127312f
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Models.Settings
{
    public class DeviceDataStorageSettings
    {
        public ConnectionSettings DeviceArchiveTableStorage { get; set; }
        public ConnectionSettings DeviceExceptionTableStorage { get; set; }
        public ConnectionSettings DeviceCurrentStatusTableStorage { get; set; }
        public ConnectionSettings DeviceStatusHistoryTableStorage { get; set; }
        public ConnectionSettings DeviceMediaTableStorage { get; set; }
        public ConnectionSettings DeviceMediaBlobStorage { get; set; }
        public ConnectionSettings SensorDataArchiveStorage { get; set; }

    }
}
