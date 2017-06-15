using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class TelemetryController : LagoVistaBaseController
    {
        ITelemetryManager _telemetryManager;
        public TelemetryController(ITelemetryManager telemetryManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _telemetryManager = telemetryManager;
        }

        /// <summary>
        /// Telemetry Get For Host
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/host/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForHostAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForHostAsync(id, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry Get For Instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/instance/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForInstanceAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForInstanceAsync(id, OrgEntityHeader, UserEntityHeader));
        }

    }
}
