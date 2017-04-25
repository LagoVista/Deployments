using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Web.Common.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.UserAdmin.Models.Account;
using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    [Route("api")]
    public class DeviceMessageDefinitionController : LagoVistaBaseController
    {
        IDeviceConfigurationManager _deviceConfigManager;
        public DeviceMessageDefinitionController(IDeviceConfigurationManager deviceConfigManager, UserManager<AppUser> userManager, ILogger logger) : base(userManager, logger)
        {
            _deviceConfigManager = deviceConfigManager;
        }

     
        /// <summary>
        /// Device Message Config - Add New
        /// </summary>
        /// <param name="deviceMessageConfiguration"></param>
        /// <returns></returns>
        [HttpPost("/api/devicemessageconfig")]
        public Task<InvokeResult> AddDeviceMessageConfigurationAsync([FromBody] DeviceMessageDefinition deviceMessageConfiguration)
        {
            return _deviceConfigManager.AddDeviceMessageDefinitionAsync(deviceMessageConfiguration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Message Config - Update Config
        /// </summary>
        /// <param name="deviceMessageConfiguration"></param>
        /// <returns></returns>
        [HttpPut("/api/devicemessageconfig")]
        public Task<InvokeResult> UpdateDeviceMessageConfigurationAsync([FromBody] DeviceMessageDefinition deviceMessageConfiguration)
        {
            SetUpdatedProperties(deviceMessageConfiguration);
            return _deviceConfigManager.UpdateDeviceMessageDefinitionAsync(deviceMessageConfiguration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Message Config - Get Configs for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/devicemessageconfigs")]
        public async Task<ListResponse<DeviceMessageDefinitionSummary>> GetDeviceMessageConfigurationsForOrgAsync(String orgId)
        {
            var deviceMessageConfiguration = await _deviceConfigManager.GetDeviceMessageDefinitionsForOrgsAsync(orgId, UserEntityHeader);
            return ListResponse<DeviceMessageDefinitionSummary>.Create(deviceMessageConfiguration);
        }

        /// <summary>
        /// Device Message Config - Get A Configuration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicemessageconfig/{id}")]
        public async Task<DetailResponse<DeviceMessageDefinition>> GetDeviceMessageConfigurationAsync(String id)
        {
            var deviceMessageConfiguration = await _deviceConfigManager.GetDeviceMessageDefinitionAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeviceMessageDefinition>.Create(deviceMessageConfiguration);
        }

        /// <summary>
        /// Device Message Config - Check in Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/devicemessageconfig/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _deviceConfigManager.CheckDeviceMessageInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Message Config - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/devicemessageconfig/{key}/keyinuse")]
        public Task<bool> DeviceConfigKeyInUse(String key)
        {
            return _deviceConfigManager.QueryDeviceConfigurationKeyInUseAsync(key, CurrentOrgId);
        }

        /// <summary>
        /// Device Message Config - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/devicemessageconfig/{id}")]
        public Task<InvokeResult> DeleteDeviceMessageConfigurationAsync(string id)
        {
            return _deviceConfigManager.DeleteDeviceConfigurationAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Message Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("devicemessageconfig/factory")]
        public DetailResponse<DeviceMessageDefinition> CreateDeviceMessageConfigurartion()
        {
            var response = DetailResponse<DeviceMessageDefinition>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }
    }
}
