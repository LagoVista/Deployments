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
        Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryForDeviceAsync(ListRequest listRequest, DeploymentInstance instance, string deviceUniqueId);
        Task AddDeviceStatusAsync(DeploymentInstance instance, DeviceStatus status);
        Task UpdateDeviceStatusAsync(DeploymentInstance instance, DeviceStatus status);
        Task<DeviceStatus> GetDeviceStatusAsync(DeploymentInstance instance, string deviceUniqueId);
    }
}
