using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
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
        IDeviceRepositoryManager _repoManager;

        public TelemetryController(ITelemetryManager telemetryManager, IDeviceRepositoryManager repoManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _telemetryManager = telemetryManager;
            _repoManager = repoManager;
        }

        /// <summary>
        /// Telemetry - Get For Host
        /// </summary>
        /// <param name="hostid"></param>
        /// <param name="recordtype"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/host/{hostid}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForHostAsync(string recordtype, string hostid)
        {
            return await _telemetryManager.GetForHostAsync(hostid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Instance
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="recordtype"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/instance/{instanceid}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForInstanceAsync(string recordtype, string instanceid)
        {
            return await _telemetryManager.GetForInstanceAsync(instanceid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Pipeline Module
        /// </summary>
        /// <param name="recordtype"></param> 
        /// <param name="instanceid"></param> 
        /// <param name="pipelinemoduleid"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/{instanceid}/pipeline/{pipelinemoduleid}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineAsync(string recordtype, string instanceid, string pipelinemoduleid)
        {
            return await _telemetryManager.GetForPipelineModuleAsync(instanceid, pipelinemoduleid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Pipline Queue
        /// </summary>
        /// <param name="recordtype"></param>
        /// <param name="instanceid"></param> 
        /// <param name="pipelinemoduleid"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/{instanceid}pipelinequeue/{pipelinemoduleid}")]
        public async Task<ListResponse<TelemetryReportData>> GetTelemetryForPipelineQueueAsync(string recordtype, string instanceid, string pipelinemoduleid)
        {
            return await _telemetryManager.GetForPipelineQueueAsync(instanceid, pipelinemoduleid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Device 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="devicerepoid"></param>
        /// <param name="recordtype"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/devices/{devicerepoid}/{deviceid}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceIdAsync(string recordtype, string devicerepoid, string deviceid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _telemetryManager.GetForDeviceAsync(repo, deviceid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Device Type
        /// </summary>
        /// <param name="devicetypeid"></param>
        /// <param name="devicerepoid"></param>
        /// <param name="recordtype"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/devicetype/{devicerepoid}/{devicetypeid}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceTypeIdAsync(string recordtype, string devicerepoid, string devicetypeid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, OrgEntityHeader, UserEntityHeader);
            return await _telemetryManager.GetForDeviceTypeAsync(repo, devicetypeid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get Details for a Deployment Activity
        /// </summary>
        /// <param name="deploymentactivityid"></param>
        /// <param name="recordtype"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/deploymentactivity/{deploymentactivityid}")]
        public async Task<ListResponse<TelemetryReportData>> GetForDeviceTypeIdAsync(string recordtype, string deploymentactivityid)
        {
            return await _telemetryManager.GetForDeploymentActivityAsync(deploymentactivityid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get For Pipeline Execution Message
        /// </summary>
        /// <param name="recordtype"></param>
        /// <param name="instanceid"></param>
        /// <param name="pemid"></param>
        /// <returns></returns>
        [HttpGet("/api/telemetry/{recordtype}/{instanceid}/pem/{pemid}")]
        public async Task<ListResponse<TelemetryReportData>> GetForPemAsync(string recordtype, string instanceid, string pemid)
        {
            return await _telemetryManager.GetForPemAsync(instanceid, pemid, recordtype, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Telemetry - Get Latest Errors
        /// </summary>
        /// <returns></returns>
        [SystemAdmin()]
        [HttpGet("/api/telemetry/errors")]
        public async Task<ListResponse<TelemetryReportData>> GetAllErrorsAsync()
        {
            return await _telemetryManager.GetAllErrorsAsync(GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }
    }
}
