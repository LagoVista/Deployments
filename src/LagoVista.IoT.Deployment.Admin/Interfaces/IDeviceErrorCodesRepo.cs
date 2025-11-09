// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 34ac90c158734a9eb6e787d04145bd5de21041c3d819679f1bbd90564ae1c372
// IndexVersion: 2
// --- END CODE INDEX META ---
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
        Task<DeviceErrorCode> GetErrorCodeByKeyAsync(string key, string orgId);
        Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateErrorCodeAsync(DeviceErrorCode errorCode);
    }
}
