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

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class TelemetryManager : ManagerBase, ITelemetryManager
    {
        ITelemetryService _telemetryService;

        public TelemetryManager(IAdminLogger adminLogger, IAppConfig appConfig, ITelemetryService telemetryService, IDependencyManager dependencyManager, ISecurity security) : base(adminLogger, appConfig, dependencyManager, security)
        {
            _telemetryService = telemetryService;
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceAsync(string deviceId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForDeviceAsync(deviceId, take, afterDateStamp);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForDeviceTypeAsync(string deviceTypeId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForDeviceTypeAsync(deviceTypeId, take, afterDateStamp);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForHostAsync(hostId, take, afterDateStamp);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForInstanceAsync(instanceId,  take,  afterDateStamp);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineModuleAsync(string pipelineModuleId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForPipelineModuleAsync(pipelineModuleId, take, afterDateStamp);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForPipelineQueueAsync(string pipelineModuleId, int take, string afterDateStamp, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForPipelineQueueAsync(pipelineModuleId, take, afterDateStamp);
        }
    }
}
