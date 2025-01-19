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
