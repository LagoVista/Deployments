using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class IncidentProtocolProtocolController : LagoVistaBaseController
    {
        IIncidentProtocolManager _incidentProtocolManager;

        public IncidentProtocolProtocolController(IIncidentProtocolManager incidentProtocolManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _incidentProtocolManager = incidentProtocolManager ?? throw new ArgumentNullException(nameof(incidentProtocolManager));
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="incidentProtocol"></param>
        /// <returns></returns>
        [HttpPost("/api/incident/protocol")]
        public Task<InvokeResult> AddIncidentProtocolAsync([FromBody] IncidentProtocol incidentProtocol)
        {
            return _incidentProtocolManager.AddIncidentProtocolAsync(incidentProtocol, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="incidentProtocol"></param>
        /// <returns></returns>
        [HttpPut("/api/incident/protocol")]
        public Task<InvokeResult> UpdateIncidentProtocolAsync([FromBody] IncidentProtocol incidentProtocol)
        {
            SetUpdatedProperties(incidentProtocol);
            return _incidentProtocolManager.UpdateIncidentProtocolAsync(incidentProtocol, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Get Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/incident/protocol/{id}")]
        public async Task<DetailResponse<IncidentProtocol>> GetIncidentProtocolAsync(string id)
        {
            var incidentProtocol = await _incidentProtocolManager.GetIncidentProtocolAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<IncidentProtocol>.Create(incidentProtocol);
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/incident/protocol/factory")]
        public DetailResponse<IncidentProtocol> CreateNewIncidentProtocol()
        {
            var incidentProtocol = DetailResponse<IncidentProtocol>.Create();
            SetAuditProperties(incidentProtocol.Model);
            SetOwnedProperties(incidentProtocol.Model);
            return incidentProtocol;
        }

        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/incident/protocols")]
        public Task<ListResponse<IncidentProtocolSummary>> GetIncidentProtocolsForOrg()
        {
            return _incidentProtocolManager.GetIncidentProtocolsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Delete Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/incident/protocol/{id}")]
        public async Task<InvokeResult> DeleteIncidentProtocolAsync(string id)
        {
            return await _incidentProtocolManager.DeleteIncidentProtocolAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}