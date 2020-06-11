using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IServiceTicketCreator
    {
        Task<InvokeResult<string>> CreateServiceTicketAsync(string templateId, string repoId, string deviceId, string details = "");
        Task<InvokeResult> HandleDeviceExceptionAsync(DeviceException exception, EntityHeader org,EntityHeader user);
    }
}
