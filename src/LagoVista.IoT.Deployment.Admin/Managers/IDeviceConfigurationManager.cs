using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IDeviceConfigurationManager
    {
        Task<InvokeResult> AddDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user);
        Task<DeviceConfiguration> GetDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);

        Task<DeviceConfiguration> LoadFullDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckDeviceConfigInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgsAsync(string orgId, EntityHeader user);
        Task<InvokeResult> UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryDeviceConfigurationKeyInUseAsync(string key, string orgId);   
    }
}