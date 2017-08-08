using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForHostAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry Get For Instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/instance/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForInstanceAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForInstanceAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry Get For Pipeline Module
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/pipeline/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForPipelineModuleAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Pipline Queue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/pipelinequeue/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineQueueAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForPipelineQueueAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Device 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/deviceid/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceIdAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForDeviceAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Device Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/devicetype/{id}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceTypeIdAsync(String id)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForDeviceTypeAsync(id, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader));
        }


    }
}
