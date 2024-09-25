using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Web.Common.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceAdmin.Models;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Models;
using MongoDB.Bson.Serialization.Serializers;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class DeviceConfigurationController : LagoVistaBaseController
    {
        private readonly IDeviceConfigurationManager _deviceConfigManager;
       
        public DeviceConfigurationController(IDeviceConfigurationManager deviceConfigManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deviceConfigManager = deviceConfigManager ?? throw new ArgumentNullException(nameof(deviceConfigManager));
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
        /// Device Config - Get Configs for Current Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfigs")]
        public Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgAsync()
        {
            return _deviceConfigManager.GetDeviceConfigurationsForOrgsAsync(OrgEntityHeader.Id, GetListRequestFromHeader(), UserEntityHeader);
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
        /// Device Config - Get custom page for device configuration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/{id}/custompage")]
        public async Task<InvokeResult<string>> GetCustomPageForDeviceConfigAsync(String id)
        {
            var customPage = await  _deviceConfigManager.GetCustomPageForDeviceConfigAsync(id, OrgEntityHeader, UserEntityHeader);
            return InvokeResult<string>.Create(customPage);
        }

        [HttpGet("/api/deviceconfig/{id}/custompage/quicklink")]
        public async Task<InvokeResult<string>> GetQuickLinkCustomPageForDeviceConfigAsync(String id)
        {
            var customPage = await _deviceConfigManager.GetQuickLinkCustomPageForDeviceConfigAsync(id, OrgEntityHeader, UserEntityHeader);
            return InvokeResult<string>.Create(customPage);
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
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);
            return response;
        }

        /// <summary>
        ///  Device Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/sensordefinition/factory")]
        public DetailResponse<SensorDefinition> CreateSensorDefinition()
        {
            return DetailResponse<SensorDefinition>.Create();
        }

        /// <summary>
        ///  Device Config Command - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/command/factory")]
        public DetailResponse<DeviceCommand> CreateDeviceConfigCommand()
        {
            return DetailResponse<DeviceCommand>.Create();
        }


        /// <summary>
        ///  Device Config Command Parameter- Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/command/parameter/factory")]
        public DetailResponse<Parameter> CreateCommandParameter()
        {
            return DetailResponse<Parameter>.Create();
        }

        /// <summary>
        ///  Device Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/messagewatchdog/factory")]
        public DetailResponse<MessageWatchDog> CreateMessageWatchDog()
        {
            return DetailResponse<MessageWatchDog>.Create();
        }

        /// <summary>
        ///  Device Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/watchdogexclusion/factory")]
        public DetailResponse<WatchdogExclusion> CreateWatchDogExlusion()
        {
            return DetailResponse<WatchdogExclusion>.Create();
        }

        /// <summary>
        ///  Device Config Property - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/property/factory")]
        public DetailResponse<CustomField> CreateDeviceConfigProperty()
        {
            return DetailResponse<CustomField>.Create();
        }

        /// <summary>
        /// Device Config Properpty - Get List for Config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/{id}/properties")]
        public async Task<List<CustomField>> GetDeviceConfigProperties(string id)
        {
            var config = await _deviceConfigManager.GetDeviceConfigurationAsync(id, OrgEntityHeader, UserEntityHeader);
            return config.Properties;
        }

        /// <summary>
        ///  Route - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/route/factory")]
        public async Task<DetailResponse<Route>> CreateRoute()
        {
            var route = await _deviceConfigManager.CreateRouteWithDefaultsAsync(OrgEntityHeader);
            var response = DetailResponse<Route>.Create(route, true);
            response.IsEditing = false;
            return response;
        }

        /// <summary>
        ///  Route - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/route/moduleconfig/factory")]
        public DetailResponse<RouteModuleConfig> CreateRouteModuleConfig()
        {
            return DetailResponse<RouteModuleConfig>.Create();
        }

        /// <summary>
        ///  Error Codes - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deviceconfig/errorcode/factory")]
        public DetailResponse<DeviceErrorCode> CreateErrorCodeConfig()
        {
            return DetailResponse<DeviceErrorCode>.Create();
        }
    }
}
