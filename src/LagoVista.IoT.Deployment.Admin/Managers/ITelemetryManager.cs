using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface ITelemetryManager
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(String pipelineModuleId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(String pipelineModuleId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(String deviceId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(String deviceTypeId, int take, string afterDateStamp, EntityHeader org, EntityHeader user);
    }
}
