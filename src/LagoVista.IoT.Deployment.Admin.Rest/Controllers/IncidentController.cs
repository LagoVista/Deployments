// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c6c92006d52512c531086844fe84d0d7193b1583141f51ae847f09bd4cb1d7fc
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class IncidentController : LagoVistaBaseController
    {
        IIncidentManager _incidentManager;

        public IncidentController(IIncidentManager incidentManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _incidentManager = incidentManager ?? throw new ArgumentNullException(nameof(incidentManager));
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="incident"></param>
        /// <returns></returns>
        [HttpPost("/api/incident")]
        public Task<InvokeResult> AddIncidentAsync([FromBody] Incident incident)
        {
            return _incidentManager.AddIncidentAsync(incident, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="incident"></param>
        /// <returns></returns>
        [HttpPut("/api/incident")]
        public Task<InvokeResult> UpdateIncidentAsync([FromBody] Incident incident)
        {
            SetUpdatedProperties(incident);
            return _incidentManager.UpdateIncidentAsync(incident, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Get Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/incident/{id}")]
        public async Task<DetailResponse<Incident>> GetIncidentAsync(string id)
        {
            var incident = await _incidentManager.GetIncidentAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Incident>.Create(incident);
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/incident/factory")]
        public DetailResponse<Incident> CreateNewIncident()
        {
            var incident = DetailResponse<Incident>.Create();
            SetAuditProperties(incident.Model);
            SetOwnedProperties(incident.Model);
            return incident;
        }

        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/incidents")]
        public Task<ListResponse<IncidentSummary>> GetIncidentsForOrg()
        {
            return _incidentManager.GetIncidentsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/incidents/open")]
        public Task<ListResponse<IncidentSummary>> GetOpenIncidentsForOrg()
        {
            return _incidentManager.GetIncidentsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Delete Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/incident/{id}")]
        public async Task<InvokeResult> DeleteIncidentAsync(string id)
        {
            return await _incidentManager.DeleteIncidentAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}