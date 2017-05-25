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
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using LagoVista.IoT.DeviceMessaging.Admin.Repos;
using LagoVista.IoT.DeviceMessaging.Admin.Managers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceConfigurationManager : ManagerBase, IDeviceConfigurationManager
    {
        IDeviceMessageDefinitionManager _deviceMessageDefinitionManager;
        IDeviceConfigurationRepo _deviceConfigRepo;
        IPipelineModuleManager _pipelineModuleManager;
        IDeviceAdminManager _deviceAdminManager;
        

        public DeviceConfigurationManager(IDeviceConfigurationRepo deviceConfigRepo, IDeviceMessageDefinitionManager deviceMessageDefinitionManager, IPipelineModuleManager pipelineModuleManager, IDeviceAdminManager deviceAdminManager, ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
            base(logger, appConfig, depmanager, security)
        {
            _pipelineModuleManager = pipelineModuleManager;
            _deviceConfigRepo = deviceConfigRepo;
            _deviceAdminManager = deviceAdminManager;
            _deviceMessageDefinitionManager = deviceMessageDefinitionManager;
    }

        public async Task<InvokeResult> AddDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user)
        {
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
                    msgDefinition.Value = await _deviceMessageDefinitionManager.LoadFullDeviceMessageDefinitionAsync(msgDefinition.Id);
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
            await AuthorizeOrgAccess(user, orgId, typeof(DeviceConfiguration));        
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
    }
}
