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
        private const string MAX_DATE = "9999-12-31T23:59:59.999Z";

        ITelemetryManager _telemetryManager;
        public TelemetryController(ITelemetryManager telemetryManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _telemetryManager = telemetryManager;
        }

        /// <summary>
        /// Telemetry Get For Host
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/host/{id}/{take}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForHostAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForHostAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry Get For Instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/instance/{id}/{take}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForInstanceAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForInstanceAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry Get For Pipeline Module
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/pipeline/{id}/{take}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForPipelineModuleAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Pipline Queue
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/pipelinequeue/{id}/{take}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineQueueAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForPipelineQueueAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Device 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/deviceid/{id}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceIdAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForDeviceAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


        /// <summary>
        /// Telemetry - Get For Device Type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/devicetype/{id}/{before?}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceTypeIdAsync(String id, int take, string before = MAX_DATE)
        {
            return ListResponse<TelemetryReportData>.Create(await _telemetryManager.GetForDeviceTypeAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }


    }
}
