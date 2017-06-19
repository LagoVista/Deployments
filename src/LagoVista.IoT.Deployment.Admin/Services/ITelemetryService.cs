using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public interface ITelemetryService
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, int take, string afterTimeStamp);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, int take, string afterTimeStamp);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(String pipelineModuleId, int take, string afterTimeStamp);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(String pipelineModuleId, int take, string afterTimeStamp);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(String deviceId, int take, string afterTimeStamp);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(String deviceTypeId, int take, string afterTimeStamp);
    }
}
