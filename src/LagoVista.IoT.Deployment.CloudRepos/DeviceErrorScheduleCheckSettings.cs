using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Interfaces;

namespace LagoVista.IoT.Deployment.CloudRepos
{
    public class DeviceErrorScheduleCheckSettings : IDeviceErrorScheduleCheckSettings
    {
        public IConnectionSettings DeviceErrorScheduleQueueSettings { get; }
    }
}
