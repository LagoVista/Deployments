using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LagoVista.IoT.Deployment.CloudRepos
{
    public class DeviceErrorScheduleCheckSettings : IDeviceErrorScheduleCheckSettings
    {
        public IConnectionSettings DeviceErrorScheduleQueueSettings { get; }

        public DeviceErrorScheduleCheckSettings(IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection("ErrorHandlerQueue");
            DeviceErrorScheduleQueueSettings = new ConnectionSettings()
            {
                AccountId = section.Require("AccountId"),
                UserName = section.Require("AccessKeyName"),
                AccessKey = section.Require("AccessKey"),
                ResourceName = section.Require("QueueName"),
            };
        }
    }
}
