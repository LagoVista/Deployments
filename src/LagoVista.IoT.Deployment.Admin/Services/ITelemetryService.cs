using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public interface ITelemetryService
    {
        Task<IEnumerable<TelemetryReportData>> GetForHostAsync(String hostId);

        Task<IEnumerable<TelemetryReportData>> GetForInstanceAsync(String instanceId);
    }
}
