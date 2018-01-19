using System;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceManagementConnectorService : ConnectorServiceBase, IDeviceManagementConnector
    {

        public DeviceManagementConnectorService(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger) : base(deploymentHostRepo, logger)
        {
        }

        public Task<InvokeResult> AddDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user)
        {
            var uri = "/api/device";
            return PostAsync<Device>(uri, device, instanceId, org, user);
        }

        public Task DeleteDeviceAsync(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/{id}";
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user)
        {
            var uri = "/api/device";
            return PutAsync<Device>(uri, device, instanceId, org, user);
        }

        public Task DeleteDeviceByIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/deviceid/{deviceId}";
            return DeleteAsync(uri,instanceId, org, user);
        }

        public  Task<ListResponse<DeviceSummary>> GetDevicesForOrgIdAsync(string instanceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices";
            return GetListResponseAsync<DeviceSummary>(uri, instanceId, org, user, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(string instanceId, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/deviceid/{deviceId}";
            var result = await GetAsync<Device>(uri, instanceId, org, user);
            return result.Result;
        }

        public Task<bool> CheckIfDeviceIdInUse(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/{id}";

            throw new NotImplementedException();
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices/{status}";
            return GetListResponseAsync<DeviceSummary>(uri, instanceId, org, user, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices/config/{configurationId}";
            return GetListResponseAsync<DeviceSummary>(uri, instanceId, org, user, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(string instanceId, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices/{deviceTypeId}";
            return GetListResponseAsync<DeviceSummary>(uri, instanceId, org, user, listRequest);
        }

        public Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices/config/{configurationId}/full";
            return GetListResponseAsync<Device>(uri, instanceId, org, user, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices/customstatus/{status}";
            return GetListResponseAsync<DeviceSummary>(uri, instanceId, org, user, listRequest);
        }
    }
}
