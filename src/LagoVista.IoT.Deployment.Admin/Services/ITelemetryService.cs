using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public interface ITelemetryService
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, string recordType, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, string recordType, ListRequest request);
        Task<String> GetItemDetailsAsync(string itemId, string recordType);
    }
}
