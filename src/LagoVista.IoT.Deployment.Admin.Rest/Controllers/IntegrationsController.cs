using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    /// <summary>
    /// Manage Deployment Instances 
    /// </summary>
    [ConfirmedUser]
    [AppBuilder]
    [Authorize]

    public class IntegrationsController : LagoVistaBaseController
    {
        IIntegrationManager _integrationManager;
        public IntegrationsController(IIntegrationManager instanceManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _integrationManager = instanceManager;
        }


        /// <summary>
        /// Integration - Add
        /// </summary>
        /// <param name="integration"></param>
        /// <returns></returns>
        [HttpPost("/api/integration")]
        public Task<InvokeResult> AddInstanceAsync([FromBody] Integration integration)
        {
            return _integrationManager.AddIntegrationAsync(integration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Integration - Update
        /// </summary>
        /// <param name="integration"></param>
        /// <returns></returns>
        [HttpPut("/api/integration")]
        public Task<InvokeResult> UpdateInstanceAsync([FromBody] Integration integration)
        {
            SetUpdatedProperties(integration);
            return _integrationManager.UpdateIntegrationAsync(integration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Integration - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/integration/{id}")]
        public Task DeleteIntegrationAsync(string id)
        {
            return _integrationManager.DeleteIntegrationAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Integration - Get all for org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/integrations")]
        public async Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrg()
        {
            var integrationSummaries = await _integrationManager.GetIntegrationsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
            return ListResponse<IntegrationSummary>.Create(integrationSummaries);
        }

        /// <summary>
        /// Integration - Get Integration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/integration/{id}")]
        public Task<Integration> GetIntegrationAsync(string id)
        {
            return _integrationManager.GetIntegrationAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Integration - Key  In User
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/integration/{key}/keyinuse")]
        public Task<bool> InstanceKeyInUse(String key)
        {
            return _integrationManager.QueryKeyInUse(key, OrgEntityHeader);
        }

    }
}
