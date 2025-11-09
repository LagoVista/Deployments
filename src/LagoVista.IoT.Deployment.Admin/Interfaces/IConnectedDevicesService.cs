// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c8e17e994624e99fef3d2693bfe80b3490737b798a73fba8d7d2ade3fb4af679
// IndexVersion: 2
// --- END CODE INDEX META ---
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
