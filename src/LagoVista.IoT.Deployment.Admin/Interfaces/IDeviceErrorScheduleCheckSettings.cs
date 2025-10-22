// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b51a11b4e3441abcf054dad0143c60cea6e55ff5a6407929a5f7c519ed816af0
// IndexVersion: 0
// --- END CODE INDEX META ---
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
