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
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(String pipelineModuleId, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(String pipelineModuleId, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(String deviceId, ListRequest request);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(String deviceTypeId, ListRequest request);
    }
}
