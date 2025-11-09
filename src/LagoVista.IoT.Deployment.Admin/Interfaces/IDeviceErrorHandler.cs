// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d9dd9a8dfbfeb5e353031c45d4def138dc4cad4c2eac6c3bcd7c5f54657df7a2
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceErrorHandler
    {
        Task<InvokeResult> HandleDeviceExceptionAsync(DeviceException deviceException, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ClearDeviceExceptionAsync(DeviceException deviceException, EntityHeader org, EntityHeader user);
    }
}
