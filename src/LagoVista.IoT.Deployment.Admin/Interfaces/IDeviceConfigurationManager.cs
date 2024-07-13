using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeviceConfigurationManager
    {
        Task<InvokeResult> AddDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user);
        Task<DeviceConfiguration> GetDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult<DeviceConfiguration>> LoadFullDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckDeviceConfigInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgsAsync(string orgId, ListRequest listRequest, EntityHeader user);
        Task<InvokeResult> UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryDeviceConfigurationKeyInUseAsync(string key, string orgId);

        Task<EntityHeader<StateSet>> GetCustomDeviceStatesAsync(string deviceConfigId, EntityHeader org, EntityHeader user);

        Task<Route> CreateRouteWithDefaultsAsync(EntityHeader org);

        Task<string> GetCustomPageForDeviceConfigAsync(string id, EntityHeader org, EntityHeader user);
    }
}