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
        Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateErrorCodeAsync(DeviceErrorCode errorCode, EntityHeader org, EntityHeader user); 
    }
}
