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
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId, EntityHeader org, EntityHeader user);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId, EntityHeader org, EntityHeader user);
    }
}
