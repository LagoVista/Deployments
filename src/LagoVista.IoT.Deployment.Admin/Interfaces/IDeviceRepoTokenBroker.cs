using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeviceRepoTokenBroker
    {
        Task<InvokeResult<DeviceRepoToken>> GetDeviceRepoTokenAsync(EntityHeader instance, EntityHeader org);
    }
}
