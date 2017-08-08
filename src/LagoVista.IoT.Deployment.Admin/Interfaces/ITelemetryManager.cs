using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ITelemetryManager
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(String pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(String pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(String deviceId, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(String deviceTypeId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
