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

        public Task<IEnumerable<TelemetryReportData>> GetForHostAsync(string hostId, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccess(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForHostAsync(hostId);
        }

        public Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            base.AuthorizeOrgAccess(user, org, typeof(TelemetryReportData));
            return _telemetryService.GetForInstanceAsync(instanceId);
        }
    }
}
