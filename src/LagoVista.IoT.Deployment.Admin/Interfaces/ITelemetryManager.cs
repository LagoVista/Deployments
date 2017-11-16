using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ITelemetryManager
    {
        Task<ListResponse<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForDeviceAsync(DeviceRepository deviceRepo, string deviceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetAllErrorsAsync(ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForDeploymentActivityAsync(string activityid, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

        Task<ListResponse<TelemetryReportData>> GetForPemAsync(string pemId, string recordType, ListRequest request, EntityHeader org, EntityHeader user);

    }
}
