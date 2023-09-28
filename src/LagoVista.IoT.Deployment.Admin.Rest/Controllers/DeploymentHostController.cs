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
using LagoVista.IoT.ProductStore;


namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    /// <summary>
    /// Manage Deployment Hosts
    /// </summary>     
    [ConfirmedUser]
    [OrgAdmin]
    [Authorize]
    public class DeploymentHostController : LagoVistaBaseController
    {
        private readonly IDeploymentHostManager _hostManager;
        private readonly IProductStore _productStore;

        public DeploymentHostController(IDeploymentHostManager hostManager, IProductStore productStore, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _hostManager = hostManager;
            _productStore = productStore;
        }

        /// <summary>
        /// Deployment Host - Add
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/host")]
        public Task<InvokeResult> AddHostAsync([FromBody] DeploymentHost host)
        {
            return _hostManager.AddDeploymentHostAsync(host, OrgEntityHeader, UserEntityHeader);
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
        /// Deployment Host - Update Status
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/host/status")]
        public Task<InvokeResult> UpdateHostStatusAsync([FromBody] HostStatusUpdate host)
        {
            return _hostManager.UpdateDeploymentHostStatusAsync(host.Id, host.NewStatus, host.Version, OrgEntityHeader, UserEntityHeader, host.Details);
        }


        /// <summary>
        /// Deployment Host - Get for Current Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/hosts")]
        public async Task<ListResponse<DeploymentHostSummary>> GetHostsForOrgAsync()
        {
            var hostSummaries = await _hostManager.GetDeploymentHostsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
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
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/generate/{key}")]
        public Task<InvokeResult<string>> RegenerateAccessKeysAsync(String id, string key)
        {
            return _hostManager.RegenerateKeyAsync(id, key, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Deployment Host - Get Status History
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/statushistory")]
        public Task<ListResponse<DeploymentHostStatus>> GetDeploymentHostStatusHistoryAsync(string id)
        {
            return _hostManager.GetDeploymentHostStatusHistoryAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Deployment Host - Deploy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/deployhost")]
        public Task<InvokeResult> DeployAsync(String id)
        {
            return _hostManager.DeployHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Host - Deploy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/deploycontainer")]
        public Task<InvokeResult> DeployContainerAsync(String id)
        {
            return _hostManager.DeployContainerAsync(id, OrgEntityHeader, UserEntityHeader);
        }       

        /// <summary>
        /// Deployment Host - Restart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/restarthost")]
        public Task<InvokeResult> RestartHostAsync(String id)
        {
            return _hostManager.RestartHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        //TODO: This should probably be nuked, it's either named horably or not inuse.
        /// <summary>
        /// Deployment Host - Update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/update")]
        public Task<InvokeResult> UpdateAsync(String id)
        {
            return _hostManager.RestartHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }       

        /// <summary>
        /// Deployment Host - Destory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/destroyhost")]
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
        /// Deployment Host - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/secure")]
        public async Task<DetailResponse<DeploymentHost>> GetHostWithKeysAsync(String id)
        {
            var deploymentHost = await _hostManager.GetSecureDeploymentHostAsync(id, OrgEntityHeader, UserEntityHeader);
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

        /// <summary>
        /// Deployment Host - Get Types of VMS to be installed.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/store/vms")]
        public async Task<ListResponse<ProductOffering>> GetAvailableHostTypes()
        {
            var vms = await _productStore.GetProductsAsync("vms");
            return ListResponse<ProductOffering>.Create(vms);
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/hosts/active")]
        public Task<ListResponse<DeploymentHostSummary>> GetAllActiveHostsAsync()
        {
            return _hostManager.SysAdminGetActiveHostsAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/hosts/failed")]
        public Task<ListResponse<DeploymentHostSummary>> GetFailedHostsAsync()
        {
            return _hostManager.SysAdminFailedHostsAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/hosts")]
        public Task<ListResponse<DeploymentHostSummary>> GetAlldHostsAsync()
        {
            return _hostManager.SysAdminAllHostsAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }
    }
}
