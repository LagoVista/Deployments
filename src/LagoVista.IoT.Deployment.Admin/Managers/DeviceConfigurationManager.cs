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
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceConfigurationManager : ManagerBase, IDeviceConfigurationManager
    {
        IDeviceMessageDefinitionManager _deviceMessageDefinitionManager;
        IDeviceConfigurationRepo _deviceConfigRepo;
        IPipelineModuleManager _pipelineModuleManager;
        IDeviceAdminManager _deviceAdminManager;


        public DeviceConfigurationManager(IDeviceConfigurationRepo deviceConfigRepo, IDeviceMessageDefinitionManager deviceMessageDefinitionManager, IPipelineModuleManager pipelineModuleManager, IDeviceAdminManager deviceAdminManager, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
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
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Delete, user, org);
            await ConfirmNoDepenenciesAsync(deviceConfiguration);
            return InvokeResult.Success;
        }

        public async Task<DeviceConfiguration> GetDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Read, user, org);
            return deviceConfiguration;
        }

        public async Task<DeviceConfiguration> LoadFullDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);

            foreach (var route in deviceConfiguration.Routes)
            {
                await PopulateRoutes(route, org, user);
            }

            return deviceConfiguration;
        }

        public async Task PopulateRoutes(Route route, EntityHeader org, EntityHeader user)
        {            
            route.MessageDefinition.Value = await _deviceMessageDefinitionManager.LoadFullDeviceMessageDefinitionAsync(route.MessageDefinition.Id);

            foreach (var module in route.PipelineModules)
            {
                switch (module.ModuleType.Value)
                {
                    case Pipeline.Admin.Models.PipelineModuleType.InputTranslator: module.Value = await _pipelineModuleManager.LoadFullInputTranslatorConfigurationAsync(module.Id); break;
                    case Pipeline.Admin.Models.PipelineModuleType.Sentinel: module.Value = await _pipelineModuleManager.LoadFullSentinelConfigurationAsync(module.Id); break;
                    case Pipeline.Admin.Models.PipelineModuleType.Workflow: module.Value = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(module.Id, org, user); break;
                    case Pipeline.Admin.Models.PipelineModuleType.OutputTranslator: module.Value = await _pipelineModuleManager.LoadFullOutputTranslatorConfigurationAsync(module.Id); break;
                    case Pipeline.Admin.Models.PipelineModuleType.Transmitter: module.Value = await _pipelineModuleManager.LoadFullTransmitterConfigurationAsync(module.Id); break;
                    case Pipeline.Admin.Models.PipelineModuleType.Custom: module.Value = await _pipelineModuleManager.LoadFullCustomPipelineModuleConfigurationAsync(module.Id); break;
                }
            }
        }

        public async Task<IEnumerable<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgsAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceConfiguration));
            return await _deviceConfigRepo.GetDeviceConfigurationsForOrgAsync(orgId);
        }

        public async Task<InvokeResult> UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfiguration, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(deviceConfiguration, Actions.Update);
            await AuthorizeAsync(deviceConfiguration, AuthorizeActions.Update, user, org);
            await _deviceConfigRepo.UpdateDeviceConfigurationAsync(deviceConfiguration);
            return InvokeResult.Success;
        }

        public Task<bool> QueryDeviceConfigurationKeyInUseAsync(string key, string orgId)
        {
            return _deviceConfigRepo.QueryKeyInUseAsync(key, orgId);
        }
    }
}
