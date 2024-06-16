using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeviceStatusRepo
    {
        Task<ListResponse<DeviceStatus>> GetDeviceStatusForInstanceAsync(ListRequest listRequest, DeploymentInstance instance);
        Task<ListResponse<DeviceStatus>> GetTimedOutDeviceStatusForInstanceAsync(ListRequest listRequest, DeploymentInstance instance);
        Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryForDevceAsync(ListRequest listRequest, DeviceRepository repo,  string deviceUniqueId);
    }
}
