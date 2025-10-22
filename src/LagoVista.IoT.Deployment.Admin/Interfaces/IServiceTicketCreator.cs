// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fb9591cd91834f85a1ad9471ed1d4e76ad49f430f1a6d7a08645095d941b7013
// IndexVersion: 0
// --- END CODE INDEX META ---
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
   
        Task<InvokeResult> ClearDeviceExceptionAsync(DeviceException exception, EntityHeader org, EntityHeader user);
    }
}
