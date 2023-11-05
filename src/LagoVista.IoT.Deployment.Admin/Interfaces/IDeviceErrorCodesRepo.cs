using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeviceErrorCodesRepo
    {
        Task AddErrorCodeAsync(DeviceErrorCode errorCode);
        Task DeleteErrorCodeAsync(string id);
        Task<DeviceErrorCode> GetErrorCodeAsync(string id);
        Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateErrorCodeAsync(DeviceErrorCode errorCode);
    }
}
