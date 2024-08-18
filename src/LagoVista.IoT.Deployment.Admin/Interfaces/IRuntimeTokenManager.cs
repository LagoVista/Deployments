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
        Task<InvokeResult<ConnectionSettings>> GetNuvIoTCacheSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeviceDataStorageSettings>> GetDataStorageSettingsAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ConnectionSettings>> GetEHCheckPointStorageSttings(SettingType stetingType, String instanceId, EntityHeader org, EntityHeader user);
    }
}
