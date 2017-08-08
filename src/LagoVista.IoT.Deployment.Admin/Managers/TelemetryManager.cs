using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.Core.Managers;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class TelemetryManager : ManagerBase, ITelemetryManager
    {
        ITelemetryService _telemetryService;

        public TelemetryManager(IAdminLogger adminLogger, IAppConfig appConfig, ITelemetryService telemetryService, IDependencyManager dependencyManager, ISecurity security) : base(adminLogger, appConfig, dependencyManager, security)
        {
            _telemetryService = telemetryService;
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForDeviceAsync(deviceId, request);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForDeviceTypeAsync(deviceTypeId, request);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForHostAsync(hostId, request);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForInstanceAsync(instanceId, request);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForPipelineModuleAsync(pipelineModuleId, request);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForPipelineQueueAsync(pipelineModuleId, request);
        }
    }
}
