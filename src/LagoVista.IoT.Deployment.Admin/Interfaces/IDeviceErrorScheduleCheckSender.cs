// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 83a7ba35a7b1ea5565a49e17894157c0e17afe52540aae1e5065c5155603fe30
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceErrorScheduleCheckSender
    {
        Task<InvokeResult> ScheduleAsync(DeviceErrorScheduleCheck errorCheck);
    }
}
