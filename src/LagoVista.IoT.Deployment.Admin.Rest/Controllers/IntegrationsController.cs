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
using LagoVista.Core;
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
        public Task<InvokeResult> DeleteIntegrationAsync(string id)
        {
            return _integrationManager.DeleteIntegrationAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Integration - Get all for org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/integrations")]
        public Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrg()
        {
            return _integrationManager.GetIntegrationsForOrgAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
        }

        /// <summary>
        /// Integration - Get Integration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/integration/{id}")]
        public async Task<DetailResponse<Integration>> GetIntegrationAsync(string id)
        {
            var integration = await _integrationManager.GetIntegrationAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<Integration>.Create(integration);
        }

        /// <summary>
        /// Integration - Create new
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/integration/factory")]
        public DetailResponse<Integration> CreateNewIntegration(string id)
        {
            var integration = DetailResponse<Integration>.Create();
            integration.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(integration.Model);
            SetOwnedProperties(integration.Model);
            return integration;

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
