using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Pipeline.Admin.Managers;
using LagoVista.IoT.DeviceAdmin.Interfaces.Managers;
using LagoVista.Core.Managers;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceMessaging.Admin.Managers;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceConfigurationManager : ManagerBase, IDeviceConfigurationManager, IDeviceConfigHelper
    {
        IDeviceMessageDefinitionManager _deviceMessageDefinitionManager;
        IDeviceConfigurationRepo _deviceConfigRepo;
        IDeploymentInstanceManagerCore _deploymentInstanceManager;
        IPipelineModuleManager _pipelineModuleManager;
        IDeviceAdminManager _deviceAdminManager;
        IDataStreamManager _dataStreamManager;

        public DeviceConfigurationManager(IDeviceConfigurationRepo deviceConfigRepo, IDataStreamManager dataStreamManager, IDeviceMessageDefinitionManager deviceMessageDefinitionManager,
                            IPipelineModuleManager pipelineModuleManager, IDeviceAdminManager deviceAdminManager, IAdminLogger logger, IDeploymentInstanceManagerCore deploymentInstnaceManager,
                            IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _pipelineModuleManager = pipelineModuleManager;
            _deviceConfigRepo = deviceConfigRepo;
            _deviceAdminManager = deviceAdminManager;
            _deviceMessageDefinitionManager = deviceMessageDefinitionManager;
            _deploymentInstanceManager = deploymentInstnaceManager;
            _dataStreamManager = dataStreamManager;
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

            foreach (var prop in deviceConfiguration.Properties.OrderBy(prop => prop.Order))
            {
                if (prop.FieldType.Value == DeviceAdmin.Models.ParameterTypes.State)
                {
                    prop.StateSet.Value = await _deviceAdminManager.GetStateSetAsync(prop.StateSet.Id, org, user);
                }
                else if (prop.FieldType.Value == DeviceAdmin.Models.ParameterTypes.ValueWithUnit)
                {
                    prop.UnitSet.Value = await _deviceAdminManager.GetAttributeUnitSetAsync(prop.UnitSet.Id, org, user);
                }
            }

            if(!EntityHeader.IsNullOrEmpty(deviceConfiguration.CustomStatusType))
            {
                deviceConfiguration.CustomStatusType.Value = await _deviceAdminManager.GetStateSetAsync(deviceConfiguration.CustomStatusType.Id, org, user);
            }

            return deviceConfiguration;
        }


        public async Task<EntityHeader<StateSet>> GetCustomDeviceStatesAsync(string deviceConfigId, EntityHeader org, EntityHeader user)
        {
            var deviceConfig = await GetDeviceConfigurationAsync(deviceConfigId, org, user);
            return deviceConfig.CustomStatusType;
        }

        public async Task<InvokeResult<DeviceConfiguration>> LoadFullDeviceConfigurationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var result = new InvokeResult<DeviceConfiguration>();

            var deviceConfiguration = await _deviceConfigRepo.GetDeviceConfigurationAsync(id);

            foreach (var route in deviceConfiguration.Routes)
            {
                await PopulateRoutes(route, org, user);
            }

            if (result.Successful)
            {
                return InvokeResult<DeviceConfiguration>.Create(deviceConfiguration);
            }

            return result;
        }

        public async Task<InvokeResult> PopulateRoutes(Route route, EntityHeader org, EntityHeader user)
        {
            var fullLoadResult = new InvokeResult();

            var msgLoadResult = await _deviceMessageDefinitionManager.LoadFullDeviceMessageDefinitionAsync(route.MessageDefinition.Id, org, user);
            if (msgLoadResult.Successful)
            {
                route.MessageDefinition.Value = msgLoadResult.Result;
            }
            else
            {
                fullLoadResult.Concat(fullLoadResult);
            }

            foreach (var module in route.PipelineModules)
            {
                switch (module.ModuleType.Value)
                {
                    case Pipeline.Admin.Models.PipelineModuleType.InputTranslator:
                        {
                            var result = await _pipelineModuleManager.LoadFullInputTranslatorConfigurationAsync(module.Module.Id);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.DataStream:
                        {
                            var result = await _dataStreamManager.LoadFullDataStreamConfigurationAsync(module.Module.Id, org, user);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.Sentinel:
                        {
                            var result = await _pipelineModuleManager.LoadFullSentinelConfigurationAsync(module.Module.Id);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.Workflow:
                        {
                            var result = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(module.Module.Id, org, user);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                                var destModuleConfig = route.PipelineModules.Where(mod => mod.Id == module.PrimaryOutput.Id).FirstOrDefault();

                                if (destModuleConfig.ModuleType.Value == Pipeline.Admin.Models.PipelineModuleType.OutputTranslator)
                                {
                                    if (module.PrimaryOutput != null && module.PrimaryOutput.Mappings != null)
                                    {
                                        for (var idx = 0; idx < module.PrimaryOutput.Mappings.Count; ++idx)
                                        {
                                            var mapping = module.PrimaryOutput.Mappings[idx];
                                            if (mapping.Value != null)
                                            {
                                                var mappingValue = JsonConvert.DeserializeObject<OutputCommandMapping>(mapping.Value.ToString());
                                                if (mappingValue != null && !EntityHeader.IsNullOrEmpty(mappingValue.OutgoingDeviceMessage))
                                                {
                                                    var outgoingMsgLoadResult = await _deviceMessageDefinitionManager.LoadFullDeviceMessageDefinitionAsync(mappingValue.OutgoingDeviceMessage.Id, org, user);
                                                    mappingValue.OutgoingDeviceMessage.Value = outgoingMsgLoadResult.Result;
                                                    module.PrimaryOutput.Mappings[idx] = new KeyValuePair<string, object>(mapping.Key, mappingValue);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.OutputTranslator:
                        {
                            var result = await _pipelineModuleManager.LoadFullOutputTranslatorConfigurationAsync(module.Module.Id);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.Transmitter:
                        {
                            var result = await _pipelineModuleManager.LoadFullTransmitterConfigurationAsync(module.Module.Id);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                    case Pipeline.Admin.Models.PipelineModuleType.Custom:
                        {
                            var result = await _pipelineModuleManager.LoadFullCustomPipelineModuleConfigurationAsync(module.Module.Id);
                            if (result.Successful)
                            {
                                module.Module.Value = result.Result;
                            }
                            else
                            {
                                fullLoadResult.Concat(result);
                            }
                        }
                        break;
                }
            }

            return fullLoadResult;
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

        public async Task<InvokeResult> PopulateDeviceConfigToDeviceAsync(Device device, EntityHeader instanceEH, EntityHeader org, EntityHeader user)
        {
            Console.WriteLine("Hhhhh...eeerrre");

            var result = new InvokeResult();

            if (EntityHeader.IsNullOrEmpty(instanceEH))
            {
                result.AddSystemError($"Device does not have a valid device configuration Device Id={device.Id}");
                return result;
            }

            Console.WriteLine("H1");

            var deviceConfig = await GetDeviceConfigurationAsync(device.DeviceConfiguration.Id, org, user);
            if (deviceConfig == null)
            {
                result.AddSystemError($"Could Not Load Device Configuration with Device Configuration {device.DeviceConfiguration.Text}, Id={device.DeviceConfiguration.Id}.");
                return result;
            }

            var instance = await _deploymentInstanceManager.GetInstanceAsync(instanceEH.Id, org, user);

            Console.WriteLine("H2");

            if (instance != null && instance.Status.Value == DeploymentInstanceStates.Running)
            {
                if (instance.InputCommandSSL)
                {
                    device.DeviceURI = $"https://{instance.DnsHostName}:{instance.InputCommandPort}/devices/{device.Id}";
                }
                else
                {
                    device.DeviceURI = $"http://{instance.DnsHostName}:{instance.InputCommandPort}/devices/{device.Id}";
                }

                var endpoints = new List<InputCommandEndPoint>();
                foreach (var route in deviceConfig.Routes)
                {
                    Console.WriteLine("H3");

                    foreach (var module in route.PipelineModules)
                    {
                        Console.WriteLine("H4");

                        if (module.ModuleType.Value == Pipeline.Admin.Models.PipelineModuleType.Workflow)
                        {
                            Console.WriteLine("H4.1");
                            var wfLoadResult = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(module.Module.Id, org, user);
                            Console.WriteLine("H4.2");
                            if (wfLoadResult.Successful)
                            {
                                Console.WriteLine("H4.3");
                                if (wfLoadResult.Result.Attributes != null)
                                {
                                    foreach (var attribute in wfLoadResult.Result.Attributes)
                                    {
                                        if (device.AttributeMetaData == null) device.AttributeMetaData = new List<DeviceAdmin.Models.Attribute>();
                                        if (!device.AttributeMetaData.Where(attr => attr.Key == attribute.Key).Any())
                                        {
                                            device.AttributeMetaData.Add(attribute);
                                        }
                                    }
                                }

                                Console.WriteLine("H4.4");

                                if (wfLoadResult.Result.StateMachines != null)
                                {
                                    if (device.StateMachineMetaData == null) device.StateMachineMetaData = new List<StateMachine>();
                                    foreach (var stateMachine in wfLoadResult.Result.StateMachines)
                                    {
                                        if (!device.StateMachineMetaData.Where(attr => attr.Key == stateMachine.Key).Any())
                                        {
                                            device.StateMachineMetaData.Add(stateMachine);
                                        }
                                    }
                                }

                                Console.WriteLine("H4.5");

                                if (wfLoadResult.Result.InputCommands != null)
                                {
                                    foreach (var inputCommand in wfLoadResult.Result.InputCommands)
                                    {
                                        var protocol = instance.InputCommandSSL ? "https://" : "http://";
                                        var endPoint = new InputCommandEndPoint
                                        {
                                            EndPoint = $"{protocol}{instance.DnsHostName}:{instance.InputCommandPort}/{deviceConfig.Key}/{route.Key}/{wfLoadResult.Result.Key}/{inputCommand.Key}/{device.DeviceId}",
                                            InputCommand = inputCommand
                                        };

                                        if (!endpoints.Where(end => end.EndPoint == endPoint.EndPoint).Any())
                                        {
                                            endpoints.Add(endPoint);
                                        }
                                    }
                                }

                                Console.WriteLine("H43");
                            }
                            else
                            {
                                result.Concat(result);
                            }
                        }
                    }
                }
                device.InputCommandEndPoints = endpoints;
            }
            else
            {
                Console.WriteLine("H21");
                device.InputCommandEndPoints = new List<InputCommandEndPoint>();
                device.AttributeMetaData = new List<DeviceAdmin.Models.Attribute>();
                device.StateMachineMetaData = new List<StateMachine>();
            }

            Console.WriteLine("H5");

            if (deviceConfig.Properties != null)
            {
                device.PropertiesMetaData = new List<DeviceAdmin.Models.CustomField>();
                foreach (var prop in deviceConfig.Properties.OrderBy(prop => prop.Order))
                {
                    device.PropertiesMetaData.Add(prop);
                    if (prop.FieldType.Value == DeviceAdmin.Models.ParameterTypes.State)
                    {
                        prop.StateSet.Value = await _deviceAdminManager.GetStateSetAsync(prop.StateSet.Id, org, user);
                    }
                    else if (prop.FieldType.Value == DeviceAdmin.Models.ParameterTypes.ValueWithUnit)
                    {
                        prop.UnitSet.Value = await _deviceAdminManager.GetAttributeUnitSetAsync(prop.UnitSet.Id, org, user);
                    }
                }
            }

            Console.WriteLine("H6");

            return result;
        }
    }
}
