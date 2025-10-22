// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 67a614eb362ad51bdd09636423022d5036a20f59ba3f091ff6f84aecc0b0c1a8
// IndexVersion: 0
// --- END CODE INDEX META ---
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
