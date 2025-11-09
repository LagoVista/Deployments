// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e2a02e0069763ddb1611ae2f0dcc4107e6b5ed3decfefe61f00f3ac2a93087ee
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
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
    /// Manage Remote Deployments
    /// </summary>
    [ConfirmedUser]
    [Authorize]
    public class RemoteDeploymentController : LagoVistaBaseController
    {
        IRemoteDeploymentManager _manager;

        public RemoteDeploymentController(IRemoteDeploymentManager hostManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _manager = hostManager;
        }

        /// <summary>
        /// Remote Deployment - Add
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/remotedeployment")]
        public Task<InvokeResult> AddHostAsync([FromBody] RemoteDeployment deployment)
        {
            return _manager.AddRemoteDeploymentAsync(deployment, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Update
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/remotedeployment")]
        public Task<InvokeResult> UpdateHostAsync([FromBody] RemoteDeployment deployment)
        {
            SetUpdatedProperties(deployment);
            return _manager.UpdateRemoteDeploymentAsync(deployment, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Get for Current Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/remotedeployments")]
        public Task<ListResponse<RemoteDeploymentSummary>> GetRemoteDeploymentForOrgAsync()
        {
            return _manager.GetRemoteDeploymentsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Can Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/remotedeployment/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _manager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Regenerate Access Keys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/remotedeployment/{id}/regenerateaccesskeys")]
        public Task<InvokeResult> RegenerateAccessKeysAsync(String id)
        {
            return _manager.RegenerateAccessKeys(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/deployment/remotedeployment/{id}")]
        public async Task<InvokeResult> DeleteRemoteDeployment(String id)
        {
            return await _manager.DeleteRemoteDeploymentHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Remote Deployment - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/remotedeployment/{id}")]
        public async Task<DetailResponse<RemoteDeployment>> GetHostAsync(String id)
        {
            var deploymentHost = await _manager.GetRemoteDeploymentAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<RemoteDeployment>.Create(deploymentHost);
        }

        /// <summary>
        /// Remote Deployment - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/remotedeployment/factory")]
        public  DetailResponse<RemoteDeployment> CreateNew()
        {
            var newRemoteDeployment = DetailResponse<RemoteDeployment>.Create();
            SetOwnedProperties(newRemoteDeployment.Model);
            SetAuditProperties(newRemoteDeployment.Model);
            return newRemoteDeployment;
        }
    }
}
