using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IRuntimeTokenManager
    {
        Task<InvokeResult<ConnectionSettings>> GetDeviceStorageConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetDeviceWatchdogConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetUsageStorageConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetWSNotifyConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<RPCSettings>> GetRPCConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<LoggingSettings>> GetLoggingSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetPEMStorageSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetNuvIoTCacheSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceDataStorageSettings>> GetDataStorageSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetEHCheckPointStorageSttings(String instanceId, EntityHeader org, EntityHeader user);
    }
}
