using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRuntimeDeviceMediaRepo
    {
        Task<InvokeResult<string>> StoreAsync(DeviceRepository repo, string organizationId, string pemId, byte[] data, string contentType, long length, double? latitude, double? longitude);
        Task<InvokeResult<string>> AttachAsync(string organizationId, string pemId, string title, string uniqueDeviceId, string deviceId);
        Task<InvokeResult<RuntimeDeviceMediaContent>> GetAsync(string organizationId, string uniqueDeviceId, string mediaItemId);
    }
}
