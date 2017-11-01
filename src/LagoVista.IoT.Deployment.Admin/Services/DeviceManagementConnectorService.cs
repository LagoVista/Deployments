using System;
using System.Collections.Generic;
using LagoVista.Core;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceManagementConnectorService : ConnectorServiceBase, IDeviceManagementConnector
    {

        public DeviceManagementConnectorService(IDeploymentHostManager deploymentHostManager, IAdminLogger logger) : base(deploymentHostManager, logger)
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

        public Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string instanceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices";
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string instanceId, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByDeviceIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/deviceid/{deviceId}";

            throw new NotImplementedException();
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

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {

            var uri = $"/api/devices/{status}";
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(string instanceId, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }
    }
}
