using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
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

    /// <summary>
    /// Manage Deployment Instances 
    /// </summary>
    [Authorize]
    public class DeploymentInstanceController : LagoVistaBaseController
    {
        IDeploymentInstanceManager _instanceManager;
        public DeploymentInstanceController(IDeploymentInstanceManager instanceManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
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
        /// <returns></returns>
        [HttpGet("/api/deployment/instances")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync()
        {
            var instanceSummaries = await _instanceManager.GetInstanceForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
            return ListResponse<DeploymentInstanceSummary>.Create(instanceSummaries);
        }

        /// <summary>
        /// Deployment Instance - Get Status History
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/statushistory")]
        public Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string id)
        {
            return _instanceManager.GetDeploymentInstanceStatusHistoryAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }


        /// <summary>
        /// Deployment Instance - Check in Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _instanceManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}")]
        public async Task<DetailResponse<DeploymentInstance>> GetInstanceAsync(String id)
        {
            var deviceInstance = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<DeploymentInstance>.Create(deviceInstance);

            return response;
        }

        /// <summary>
        /// Deployment Instance - Get Full
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/full")]
        public async Task<DetailResponse<DeploymentInstance>> GetFullInstanceAsync(String id)
        {
            var deviceInstance = await _instanceManager.LoadFullInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
            if (deviceInstance.Successful)
            {
                return DetailResponse<DeploymentInstance>.Create(deviceInstance.Result);
            }
            else
            {
                var resp = DetailResponse<DeploymentInstance>.Create(null);
                resp.Errors.AddRange(deviceInstance.Errors);
                return resp;
            }
        }

        /// <summary>
        /// Deployment Instance - Get Full with version
        /// </summary>
        /// <param name="id"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/{versionid}/full")]
        public async Task<DetailResponse<DeploymentInstance>> GetFullInstanceAsync(string id, string versionid)
        {
            var deviceInstance = await _instanceManager.LoadFullInstanceWithVersionAsync(id, versionid, OrgEntityHeader, UserEntityHeader);
            if (deviceInstance.Successful)
            {
                return DetailResponse<DeploymentInstance>.Create(deviceInstance.Result);
            }
            else
            {
                var resp = DetailResponse<DeploymentInstance>.Create(null);
                resp.Errors.AddRange(deviceInstance.Errors);
                return resp;
            }
        }

        /// <summary>
        /// Deployment Instance - Update Status
        /// </summary>
        /// <returns></returns>
        [HttpPut("/api/deployment/instance/status")]
        public  Task<InvokeResult> UpdateInstanceStatus([FromBody] InstanceStatusUpdate statusUpdate)
        {
            return _instanceManager.UpdateInstanceStatusAsync(statusUpdate.Id, statusUpdate.NewStatus, statusUpdate.Deployed, statusUpdate.Version, 
                OrgEntityHeader, UserEntityHeader, statusUpdate.Details);
        }

        /// <summary>
        /// Deployment Instance - Get Runtime Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/runtime")]
        public Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceRunTimeAsync(String id)
        {
            return _instanceManager.GetInstanceDetailsAsync(id, OrgEntityHeader, UserEntityHeader);
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
        [HttpGet("/api/deployment/instance/{id}/deployhost")]
        public Task<InvokeResult> DeployHostAsync(String id)
        {
            return _instanceManager.DeployHostAsync(id, OrgEntityHeader, UserEntityHeader);
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
        /// Deployment Instance - Start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/pause")]
        public Task<InvokeResult> PauseAsync(String id)
        {
            return _instanceManager.PauseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Reload Solution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/reloadsolution")]
        public Task<InvokeResult> RealodSolutionAsync(String id)
        {
            return _instanceManager.ReloadSolutionAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Update Runtime
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/updateruntime")]
        public Task<InvokeResult> UpdateRuntimeAsync(String id)
        {
            return _instanceManager.UpdateRuntimeAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Restart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/restarthost")]
        public Task<InvokeResult> RestartHostAsync(String id)
        {
            return _instanceManager.RestartHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Reset Container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/resetartcontainer")]
        public Task<InvokeResult> ResetContainerAsync(String id)
        {
            return _instanceManager.RestartContainerAsync(id, OrgEntityHeader, UserEntityHeader);
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
        [HttpGet("/api/deployment/instance/{id}/destroyhost")]
        public Task<InvokeResult> RemoveAsync(String id)
        {
            return _instanceManager.DestroyHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Web Socket URI - Get a URI to Receive Web Socket Notifcations
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="id"></param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [HttpGet("/api/wsuri/{channel}/{id}/{verbosity}")]
        public Task<InvokeResult<string>> GetMonitorUriAsync(string channel, string id, string verbosity)
        {
            return _instanceManager.GetRemoteMonitoringURIAsync(channel, id, verbosity, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Request a key from a run time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/wsuri/{channel}/{id}/{verbosity}")]
        public Task<InvokeResult<string>> RequestKeyAsync([FromBody] KeyRequest request)
        {
            return _instanceManager.GetKeyAsync(request, OrgEntityHeader, UserEntityHeader);
        }
    }
}