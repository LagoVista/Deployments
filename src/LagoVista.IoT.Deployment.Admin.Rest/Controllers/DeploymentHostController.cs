using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Web.Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using LagoVista.Core;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.IoT.Web.Common.Attributes;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    /// <summary>
    /// Manage Deployment Hosts
    /// </summary>     
    [ConfirmedUser]
    [Authorize]
    public class DeploymentHostController : LagoVistaBaseController
    {
        IDeploymentHostManager _hostManager;
        public DeploymentHostController(IDeploymentHostManager hostManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _hostManager = hostManager;
        }

        /// <summary>
        /// Deployment Host - Add
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/host")]
        public Task<InvokeResult> AddHostAsync([FromBody] DeploymentHost host)
        {
            return _hostManager.AddDeploymentHostAsync(host, UserEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Update
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/host")]
        public Task<InvokeResult> UpdateHostAsync([FromBody] DeploymentHost host)
        {
            SetUpdatedProperties(host);
            return _hostManager.UpdateDeploymentHostAsync(host, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Get for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/deployment/hosts")]
        public async Task<ListResponse<DeploymentHostSummary>> GetHostsForOrgAsync(String orgId)
        {
            var hostSummaries = await _hostManager.GetDeploymentHostsForOrgAsync(orgId, UserEntityHeader);
            var response = ListResponse<DeploymentHostSummary>.Create(hostSummaries);

            return response;
        }

        /// <summary>
        /// Deployment Host - Can Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _hostManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Regenerate Access Keys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/regenerateaccesskeys")]
        public Task<InvokeResult> RegenerateAccessKeysAsync(String id)
        {
            return _hostManager.RegenerateAccessKeys(id, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Deployment Host - Deploy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/deploy")]
        public Task<InvokeResult> DeplooyAsync(String id)
        {
            return _hostManager.DeployHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/start")]
        public Task<InvokeResult> StartAsync(String id)
        {
            return _hostManager.StartHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Restart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/reset")]
        public Task<InvokeResult> ResetAsync(String id)
        {
            return _hostManager.ResetHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Stop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/stop")]
        public Task<InvokeResult> StopHostAsync(String id)
        {
            return _hostManager.StopHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Destory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/destroy")]
        public Task<InvokeResult> DestroyHostAsync(String id)
        {
            return _hostManager.DestroyHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}")]
        public async Task<DetailResponse<DeploymentHost>> GetHostAsync(String id)
        {
            var deploymentHost = await _hostManager.GetDeploymentHostAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeploymentHost>.Create(deploymentHost);
        }

        /// <summary>
        /// Deployment Host - Get Deployed Instances
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/deployedinstances")]
        public Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(String id)
        {
            return _hostManager.GetDeployedInstancesAsync(id, OrgEntityHeader, UserEntityHeader);
        }

  
        /// <summary>
        /// Device Host - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{key}/keyinuse")]
        public Task<bool> HostKeyInUse(String key)
        {
            return _hostManager.QueryDeploymentHostKeyInUseAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/deployment/host/{id}")]
        public Task<InvokeResult> DeleteHostAsync(string id)
        {
            return _hostManager.DeleteDeploymentHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Deploymnent Host - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/factory")]
        public DetailResponse<DeploymentHost> CreateDeploymentHost()
        {
            var response = DetailResponse<DeploymentHost>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }
    }
}
