using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ITelemetryManager
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<string> GetItemDetailAsync(string itemId, string recordType, EntityHeader org, EntityHeader user);
    }
}
