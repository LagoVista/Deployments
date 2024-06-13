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
