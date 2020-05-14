using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IConnectedDevicesService
    {
        Task<ListResponse<WatchdogConnectedDevice>> GetWatchdogConnectedDevicesAsync(ListRequest request);

        Task<ListResponse<WatchdogConnectedDevice>> GetTimedOutDevicesAsync(ListRequest listRequest);
    }
}
