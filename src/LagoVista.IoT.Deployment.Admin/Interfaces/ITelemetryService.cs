// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6bacaf63156397cba82062c1391fc766c327362fdd2b53e6623563dc3b3474ec
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ITelemetryService
    {
        Task<ListResponse<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForDeploymentActviityAsync(string activityId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForPemAsync(string pemId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForDeviceAsync(string deviceId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, string recordType, ListRequest request);
        Task<ListResponse<TelemetryReportData>> GetAllErrorsasync(ListRequest request);
    }
}
