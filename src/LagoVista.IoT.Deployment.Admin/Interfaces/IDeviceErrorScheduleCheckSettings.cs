using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceErrorScheduleCheckSettings
    {
        IConnectionSettings DeviceErrorScheduleQueueSettings { get; }
    }
}
