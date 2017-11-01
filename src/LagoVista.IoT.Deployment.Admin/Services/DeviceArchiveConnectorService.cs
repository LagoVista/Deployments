using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using System;
using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceArchiveConnectorService : ConnectorServiceBase, IDeviceArchiveConnector
    {
        public DeviceArchiveConnectorService(IDeploymentHostManager deploymentHostManager, IAdminLogger logger) : base(deploymentHostManager, logger)
        {

        }

        public Task AddArchiveAsync(string instanceId, DeviceArchive archiveEntry, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException("Should not add archives via connector for remote instances, should call local method.");
        }

        public async Task<ListResponse<List<object>>> GetForDateRangeAsync(string instanceId, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/archives/{deviceId}";
            var result = await GetAsync<ListResponse<List<object>>>(uri, instanceId, org, user, listRequest);
            return result.Result;
        }
    }
}
