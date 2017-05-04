using LagoVista.IoT.Web.Common.Controllers;

using LagoVista.Core.PlatformSupport;
using LagoVista.UserAdmin.Models.Account;
using Microsoft.AspNetCore.Identity;
using LagoVista.IoT.Deployment.Admin.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LagoVista.Core.Interfaces;
using LagoVista.Core;
using System;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{

    /// <summary>
    /// Manage Deployment Instances 
    /// </summary>
    [Authorize]
    public class DeploymentInstanceController : LagoVistaBaseController
    {
        IDeploymentInstanceManager _instanceManager;
        public DeploymentInstanceController(IDeploymentInstanceManager instanceManager, UserManager<AppUser> userManager, ILogger logger) : base(userManager, logger)
        {
            _instanceManager = instanceManager;
        }        

        /// <summary>
        /// Deployment Instance - Add
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/instance")]
        public Task<InvokeResult> AddInstanceAsync([FromBody] DeploymentInstance instance)
        {
            return _instanceManager.AddInstanceAsync(instance, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Update
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/instance")]
        public Task<InvokeResult> UpdateInstanceAsync([FromBody] DeploymentInstance instance)
        {
            SetUpdatedProperties(instance);
            return _instanceManager.UpdateInstanceAsync(instance, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/deployment/instances")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(String orgId)
        {
            var instanceSummaries = await _instanceManager.GetInstanceForOrgAsyncAsync(orgId, UserEntityHeader);
            return ListResponse<DeploymentInstanceSummary>.Create(instanceSummaries);
        }

        /// <summary>
        /// Deployment Instance - Check in Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _instanceManager.CheckInUserAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}")]
        public async Task<DetailResponse<DeploymentInstance>> GetInstanceAsync(String id)
        {
            var deviceMessageConfiguration = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<DeploymentInstance>.Create(deviceMessageConfiguration);

            return response;
        }

        /// <summary>
        /// Device Message Config - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{key}/keyinuse")]
        public Task<bool> InstanceKeyInUse(String key)
        {
            return _instanceManager.QueryInstanceKeyInUseAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/deployment/instance/{id}")]
        public Task<InvokeResult> DeleteInstanceAsync(string id)
        {
            return _instanceManager.DeleteInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Deploymnent Instance - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/factory")]
        public DetailResponse<DeploymentInstance> CreateInstance()
        {
            var response = DetailResponse<DeploymentInstance>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /* Methods to manage the instance */

        /// <summary>
        /// Deployment Instance - Deploy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/deploy")]
        public Task<InvokeResult> DeployAsync(String id)
        {
            return _instanceManager.DeployAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/start")]
        public Task<InvokeResult> StartAsync(String id)
        {
            return _instanceManager.StartAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Pause
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/pause")]
        public Task<InvokeResult> PauseAsync(String id)
        {
            return _instanceManager.PauseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Stop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/stop")]
        public Task<InvokeResult> StopAsync(String id)
        {
            return _instanceManager.StopAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Remove
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/remove")]
        public Task<InvokeResult> RemoveAsync(String id)
        {
            return _instanceManager.RemoveAsync(id, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="channel"></param>
        /// <param name="id"></param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/{channel}/{id}/{verbosity}")]
        public Task<InvokeResult<string>> GetMonitorUriAsync(String instanceid, string channel, string id, string verbosity)
        {
            return _instanceManager.GetRemoteMonitoringURIAsync(instanceid, channel, id, verbosity, OrgEntityHeader, UserEntityHeader);
        }
    }
}