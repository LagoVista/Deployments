using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IServiceTicketCreator
    {
        Task<InvokeResult<string>> CreateServiceTicketAsync(string templateId, string repoId, string deviceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> CreateServiceTicketAsync(CreateServiceTicketRequest request, EntityHeader org, EntityHeader user);

        Task<InvokeResult> HandleDeviceExceptionAsync(DeviceException exception, EntityHeader org, EntityHeader user);

        Task<InvokeResult> ClearDeviceExceptionAsync(DeviceException exception, EntityHeader org, EntityHeader user);
    }
}
