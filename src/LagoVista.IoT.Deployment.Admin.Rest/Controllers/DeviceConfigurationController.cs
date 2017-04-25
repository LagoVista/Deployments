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
    public class DeviceConfigurationController : LagoVistaBaseController
    {
        IDeviceConfigurationManager _deviceConfigManager;
        public DeviceConfigurationController(IDeviceConfigurationManager deviceConfigManager, UserManager<AppUser> userManager, ILogger logger) : base(userManager, logger)
        {
            _deviceConfigManager = deviceConfigManager;
        }

        /// <summary>
        /// Device Config - Add New
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <returns></returns>
        [HttpPost("/api/deviceconfig")]
        public Task<InvokeResult> AddDeviceConfigurationAsync([FromBody] DeviceConfiguration deviceConfiguration)
        {
            return _deviceConfigManager.AddDeviceConfigurationAsync(deviceConfiguration,OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Config - Update Config
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <returns></returns>
        [HttpPut("/api/deviceconfig")]
        public Task<InvokeResult> UpdateDeviceConfigurationAsync([FromBody] DeviceConfiguration deviceConfiguration)
        {
            SetUpdatedProperties(deviceConfiguration);
            return _deviceConfigManager.UpdateDeviceConfigurationAsync(deviceConfiguration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Config - Get Configs for Org
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        [HttpGet("/api/org/{orgid}/deviceconfigs")]
        public async Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgAsync(String orgId)
        {
            var deviceConfigurations = await _deviceConfigManager.GetDeviceConfigurationsForOrgsAsync(orgId, UserEntityHeader);
            var response = ListResponse<DeviceConfigurationSummary>.Create(deviceConfigurations);

            return response;
        }

        /// <summary>
        /// Device Config - Check in Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _deviceConfigManager.CheckDeviceConfigInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Config - Get A Configuration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/{id}")]
        public async Task<DetailResponse<DeviceConfiguration>> GetDeviceConfigurationAsync(String id)
        {
            var deviceConfiguration = await _deviceConfigManager.GetDeviceConfigurationAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<DeviceConfiguration>.Create(deviceConfiguration);

            return response;
        }

        /// <summary>
        /// Device Config - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/{key}/keyinuse")]
        public Task<bool> DeviceConfigKeyInUse(String key)
        {
            return _deviceConfigManager.QueryDeviceConfigurationKeyInUseAsync(key, CurrentOrgId);
        }

        /// <summary>
        /// Device Config - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/deviceconfig/{id}")]
        public Task<InvokeResult> DeleteDeviceConfigurationAsync(string id)
        {
            return _deviceConfigManager.DeleteDeviceConfigurationAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Device Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/factory")]
        public DetailResponse<DeviceConfiguration> CreateDeviceConfigurartion()
        {
            var response = DetailResponse<DeviceConfiguration>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        ///  Route - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/route/factory")]
        public DetailResponse<Route> CreateRoute()
        {
            var response = DetailResponse<Route>.Create();
            response.Model.Id = Guid.NewGuid().ToId();

            SetAuditProperties(response.Model);

            return response;
        }

    }
}
