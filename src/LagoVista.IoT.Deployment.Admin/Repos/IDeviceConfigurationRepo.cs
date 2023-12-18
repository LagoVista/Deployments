using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeviceConfigurationRepo
    {
        Task AddDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration);
        Task<DeviceConfiguration> GetDeviceConfigurationAsync(string id);
        Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration);
        Task DeleteDeviceConfigurationAsync(string id);
        Task<bool> QueryKeyInUseAsync(string key, string orgId);
    }
}