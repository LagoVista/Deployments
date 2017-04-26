using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.Core.Validation;
using LagoVista.IoT.Pipeline.Admin.Managers;
using LagoVista.IoT.DeviceAdmin.Interfaces.Managers;
using LagoVista.Core.Managers;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.Core.Authentication.Exceptions;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceConfigurationManager : ManagerBase, IDeviceConfigurationManager
    {
        IDeviceConfigurationRepo _deviceConfigRepo;
        IPipelineModuleManager _pipelineModuleManager;
        IDeviceAdminManager _deviceAdminManager;
        IDeviceMessageDefinitionRepo _deviceMessageDefinitionRepo;

        public DeviceConfigurationManager(IDeviceConfigurationRepo deviceConfigRepo, IDeviceMessageDefinitionRepo deviceMessageDefinitionRepo, 
            IPipelineModuleManager pipelineModuleManager, IDeviceAdminManager deviceAdminManager, ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _pipelineModuleManager = pipelineModuleManager;
            _deviceConfigRepo = deviceConfigRepo;
            _deviceAdminManager = deviceAdminManager;
            _deviceMessageDefinitionRepo = deviceMessageDefinitionRepo;
        }

        public async Task<InvokeResult> AddDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(deviceConfiguration, Actions.Create);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Create, user, org);
            ValidationCheck(deviceConfiguration, Actions.Create);
            await _deviceConfigRepo.AddDeviceConfigurationAsync(deviceConfiguration);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckDeviceConfigInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceConfig = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);
            await AuthorizeAsync(deviceConfig, AuthorizeActions.Read, user, org);
            return await base.CheckForDepenenciesAsync(deviceConfig);
        }


        public async Task<InvokeResult> DeleteDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Delete, user,org);            
            await ConfirmNoDepenenciesAsync(deviceConfiguration);
            await _deviceMessageDefinitionRepo.DeleteDeviceMessageDefinitionAsync(id);
            return InvokeResult.Success;
        }

        public async Task<DeviceConfiguration> GetDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Read, org, user);
            return deviceConfiguration;
        }

        public async Task<DeviceConfiguration> LoadFullDeviceConfigurationAsync(string id)
        {
            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);

            foreach (var route in deviceConfiguration.Routes)
            {
                await PopulateRoutes(route);
            }

            return deviceConfiguration;
        }

        public async Task PopulateRoutes(Route route)
        {
            if(route.MessageDefinitions != null)
            {
                foreach(var msgDefinition in route.MessageDefinitions)
                {
                    msgDefinition.Value = await LoadFullDeviceMessageDefinitionAsync(msgDefinition.Id);
                }
            }

            if (route.InputTranslator != null && route.InputTranslator.HasValue)
            {
                route.InputTranslator.Value = await _pipelineModuleManager.LoadFullInputTranslatorConfigurationAsync(route.InputTranslator.Id);
            }

            if (route.Sentinel != null && route.Sentinel.HasValue)
            {
                route.Sentinel.Value = await _pipelineModuleManager.LoadFullSentinelConfigurationAsync(route.Sentinel.Id);
            }

            if (route.DeviceWorkflow != null && route.DeviceWorkflow.HasValue)
            {
                route.DeviceWorkflow.Value = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(route.DeviceWorkflow.Id);
            }

            if (route.OutputTranslator != null && route.OutputTranslator.HasValue)
            {
                route.OutputTranslator.Value = await _pipelineModuleManager.LoadFullOutputTranslatorConfigurationAsync(route.OutputTranslator.Id);
            }

            if (route.Transmitter != null && route.Transmitter.HasValue)
            {
                route.Transmitter.Value = await _pipelineModuleManager.LoadFullTransmitterConfigurationAsync(route.Transmitter.Id);
            }
        }

        public async Task<IEnumerable<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgsAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(Solution));        
            return await _deviceConfigRepo.GetDeviceConfigurationsForOrgAsync(orgId);
        }

        public async Task<InvokeResult> UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(deviceConfiguration, Actions.Update);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Update, org, user);
            await _deviceConfigRepo.UpdateDeviceConfigurationAsync(deviceConfiguration);
            return InvokeResult.Success;
        }

        public Task<bool> QueryDeviceConfigurationKeyInUseAsync(string key, string orgId)
        {
            return _deviceConfigRepo.QueryKeyInUseAsync(key, orgId);
        }

        public async Task<InvokeResult> AddDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageConfiguration, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceMessageConfiguration, AuthorizeActions.Create, user, org);
            ValidationCheck(deviceMessageConfiguration, Actions.Create);
            await _deviceMessageDefinitionRepo.AddDeviceMessageDefinitionAsync(deviceMessageConfiguration);
            return InvokeResult.Success;
        }

        public async Task<DeviceMessageDefinition> GetDeviceMessageDefinitionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceMessageDefinition = await _deviceMessageDefinitionRepo.GetDeviceMessageDefinitionAsync(id);
            await AuthorizeAsync(deviceMessageDefinition, AuthorizeActions.Read, org, user);
            return deviceMessageDefinition;
        }

        public Task<DeviceMessageDefinition> LoadFullDeviceMessageDefinitionAsync(string id)
        {
            return  _deviceMessageDefinitionRepo.GetDeviceMessageDefinitionAsync(id);            
        }

        public async Task<IEnumerable<DeviceMessageDefinitionSummary>> GetDeviceMessageDefinitionsForOrgsAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(Solution));
            return await _deviceMessageDefinitionRepo.GetDeviceMessageDefinitionsForOrgAsync(orgId);
        }

        public async Task<InvokeResult> UpdateDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageConfiguration, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceMessageConfiguration, AuthorizeActions.Update, org, user);
            ValidationCheck(deviceMessageConfiguration, Actions.Update);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteDeviceMessageDefinitionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var messageDefinition = await _deviceMessageDefinitionRepo.GetDeviceMessageDefinitionAsync(id);
            await AuthorizeAsync(messageDefinition, AuthorizeActions.Delete, org, user);
            await ConfirmNoDepenenciesAsync(messageDefinition);
            return InvokeResult.Success;
        }

        public  Task<bool> QueryDeviceMessageDefinitionKeyInUseAsync(string key, string orgId)
        {
            return _deviceMessageDefinitionRepo.QueryKeyInUseAsync(key, orgId);
        }

        public async Task<DependentObjectCheckResult> CheckDeviceMessageInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceMessageConfiguration = await _deviceMessageDefinitionRepo.GetDeviceMessageDefinitionAsync(id);
            await AuthorizeAsync(deviceMessageConfiguration, AuthorizeActions.Read, user, org);            
            return await base.CheckForDepenenciesAsync(deviceMessageConfiguration);
        }
    }
}
