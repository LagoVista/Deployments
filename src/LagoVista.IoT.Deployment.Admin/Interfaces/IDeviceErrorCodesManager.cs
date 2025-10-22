// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f215a5207f65bce13d2cad049ae05214d8c26e1aba81368a83e074e65337660c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeviceErrorCodesManager
    {
        Task<InvokeResult> AddErrorCodeAsync(DeviceErrorCode errorCode, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteErrorCodeAsync(string id, EntityHeader org, EntityHeader user);
        Task<DeviceErrorCode> GetErrorCodeAsync(string id, EntityHeader org, EntityHeader user);
        Task<DeviceErrorCode> GetErrorCodeByKeyAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateErrorCodeAsync(DeviceErrorCode errorCode, EntityHeader org, EntityHeader user); 
    }
}
