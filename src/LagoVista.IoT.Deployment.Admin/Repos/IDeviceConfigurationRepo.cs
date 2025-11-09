// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 959d858db7e66114444498f51f2c87ba010ed190d6b8f6fa6e5015d2a49c8e63
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        Task<string> GetCustomPageForDeviceConfigAsync(string id);

        Task<string> GetQuickLinkCustomPageForDeviceConfigAsync(string id);
    }
}