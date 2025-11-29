// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0f16c042b340054ff2612aa0da0c63a0bdf2963f033deb2cc590b1c54db5c064
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public enum SettingType
    {
        Host,
        Instance
    }

    public interface IRuntimeTokenManager
    {
        Task<InvokeResult<ConnectionSettings>> GetDeviceStorageConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetDeviceWatchdogConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<ConnectionSettings>> GetDeviceEventConnectionAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetUsageStorageConnectionAsync(SettingType settingType, String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetDeviceTransactionStorageAsync(SettingType settingType, String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetAzureEventHubsWSNotifyConnectionAsync(SettingType settingType, String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetRabbitMQWSNotifyConnectionAsync(SettingType settingType, String id, EntityHeader org, EntityHeader user);
      
        Task<InvokeResult<RPCSettings>> GetRPCConnectionAsync(SettingType settingType, String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult<LoggingSettings>> GetLoggingSettingsAsync(SettingType settingType, String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetPEMStorageSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetSensorDataArchiveSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetNuvIoTCacheSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceDataStorageSettings>> GetDataStorageSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetEHCheckPointStorageSttings(SettingType stetingType, String instanceId, EntityHeader org, EntityHeader user);
    }
}
