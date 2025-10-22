// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 58a83a806c4371090b70bffce3704835bc2827b0319e16805fa4851cf9853209
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Interfaces;
using System;
using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class TelemetryConnector : ConnectorServiceBase, ITelemetryConnector
    {
        public TelemetryConnector(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger) : base(deploymentHostRepo, logger)
        {

        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetItemDetailsAsync(string itemId, string recordType, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }
    }
}
