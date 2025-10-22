// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5ff23fc85259c6d52d42e42afca7ba207e3fddcd632277d25c81af5e079c17ba
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Interfaces.Managers;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceMessaging.Admin.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Pipeline.Admin;
using LagoVista.IoT.Pipeline.Admin.Managers;
using LagoVista.IoT.Pipeline.Admin.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

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
        IDeviceTypeRepo _deviceTypeRepo;
        IRouteSupportRepo _routeSupportRepo;
        IStateSetRepo _stateSetRepo;
        IUnitSetRepo _unitSetRepo;
        ICacheProvider _cacheProvider;

        public DeviceConfigurationManager(IDeviceConfigurationRepo deviceConfigRepo, IDataStreamManager dataStreamManager, IDeviceMessageDefinitionManager deviceMessageDefinitionManager,
                            IPipelineModuleManager pipelineModuleManager, IDeviceAdminManager deviceAdminManager, IAdminLogger logger, IDeploymentInstanceManagerCore deploymentInstnaceManager,
                            IStateSetRepo stateSetRepo, IUnitSetRepo unitSetRepo,
                            IDeviceTypeRepo deviceTypeRepo, IRouteSupportRepo routeSupportRepo, IAppConfig appConfig, ICacheProvider cacheProvider, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _pipelineModuleManager = pipelineModuleManager;
            _deviceConfigRepo = deviceConfigRepo;
            _deviceAdminManager = deviceAdminManager;
            _deviceMessageDefinitionManager = deviceMessageDefinitionManager;
            _deploymentInstanceManager = deploymentInstnaceManager;
            _dataStreamManager = dataStreamManager;
            _deviceTypeRepo = deviceTypeRepo;
            _routeSupportRepo = routeSupportRepo;
            _stateSetRepo = stateSetRepo;
            _unitSetRepo = unitSetRepo;
            _cacheProvider = cacheProvider;
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

            await this._deviceConfigRepo.DeleteDeviceConfigurationAsync(id);

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

            if (!EntityHeader.IsNullOrEmpty(deviceConfiguration.CustomStatusType))
            {
                deviceConfiguration.CustomStatusType.Value = await _deviceAdminManager.GetStateSetAsync(deviceConfiguration.CustomStatusType.Id, org, user);
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceTypeLabel))
            {
                deviceConfiguration.DeviceTypeLabel = DeploymentAdminResources.DeviceConfiguration_DeviceTypeLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceIdLabel))
            {
                deviceConfiguration.DeviceIdLabel = DeploymentAdminResources.DeviceConfiguration_DeviceIdLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceNameLabel))
            {
                deviceConfiguration.DeviceNameLabel = DeploymentAdminResources.DeviceConfiguration_DeviceNameLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceLabel))
            {
                deviceConfiguration.DeviceLabel = DeploymentAdminResources.DeviceConfiguration_DeviceLabel_Default;
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

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceTypeLabel))
            {
                deviceConfiguration.DeviceTypeLabel = DeploymentAdminResources.DeviceConfiguration_DeviceTypeLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceIdLabel))
            {
                deviceConfiguration.DeviceIdLabel = DeploymentAdminResources.DeviceConfiguration_DeviceIdLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceNameLabel))
            {
                deviceConfiguration.DeviceNameLabel = DeploymentAdminResources.DeviceConfiguration_DeviceNameLabel_Default;
            }

            if (String.IsNullOrEmpty(deviceConfiguration.DeviceLabel))
            {
                deviceConfiguration.DeviceLabel = DeploymentAdminResources.DeviceConfiguration_DeviceLabel_Default;
            }

            if(!EntityHeader.IsNullOrEmpty(deviceConfiguration.CustomStatusType))
            {
                deviceConfiguration.CustomStatusType.Value = await _stateSetRepo.GetStateSetAsync(deviceConfiguration.CustomStatusType.Id);
            }

            foreach (var route in deviceConfiguration.Routes)
            {
                await PopulateRoutes(route, org, user);
            }

            foreach (var prop in deviceConfiguration.Properties)
            {
                switch (prop.FieldType.Value)
                {
                    case ParameterTypes.State: prop.StateSet.Value = await _stateSetRepo.GetStateSetAsync(prop.StateSet.Id); break;
                    case ParameterTypes.ValueWithUnit: prop.UnitSet.Value = await _unitSetRepo.GetUnitSetAsync(prop.UnitSet.Id); break;
                }
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

        public async Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgsAsync(string orgId, ListRequest listRequest, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceConfiguration));
            return await _deviceConfigRepo.GetDeviceConfigurationsForOrgAsync(orgId, listRequest);
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

        public static string GetDeviceConfigMetaDataKey(String id)
        {
            return $"DeviceConfig-InstanceMetaData-{id}";
        }

        public async Task<InvokeResult> PopulateDeviceConfigToDeviceAsync(Device device, EntityHeader instanceEH, EntityHeader org, EntityHeader user)
        {
            Logger.Trace("[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] Start;");

            var result = new InvokeResult();
            if (EntityHeader.IsNullOrEmpty(instanceEH))
            {
                Logger.Trace("[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - Does not have Intance EH, can not populate end points.");
                instanceEH = EntityHeader.Create(Guid.Empty.ToId(), "none");
            }

            var fullSw = Stopwatch.StartNew();
            var deviceConfigMetaJSON = await _cacheProvider.GetFromCollection(GetDeviceConfigMetaDataKey(instanceEH.Id), device.DeviceConfiguration.Id);
            if (!String.IsNullOrEmpty(deviceConfigMetaJSON))
            {
                Logger.Trace("[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] Found in Cache;");

                var cachedMetaData = JsonConvert.DeserializeObject<MetaDataCache>(deviceConfigMetaJSON);
                device.DeviceType.Value = cachedMetaData.DeviceType;
                device.DeviceLabel = cachedMetaData.DeviceConfiguration.DeviceLabel;
                device.DeviceIdLabel = cachedMetaData.DeviceConfiguration.DeviceIdLabel;
                device.DeviceNameLabel = cachedMetaData.DeviceConfiguration.DeviceNameLabel;
                device.DeviceTypeLabel = cachedMetaData.DeviceConfiguration.DeviceTypeLabel;
                device.PropertiesMetaData = cachedMetaData.PropertiesMetaData;
                device.AttributeMetaData = cachedMetaData.AttributeMetaData;
                device.StateMachineMetaData = cachedMetaData.StateMachineMetaData;
                device.InputCommandEndPoints = cachedMetaData.InputCommandEndPoints;

                foreach (var ep in device.InputCommandEndPoints)
                {
                    ep.EndPoint = ep.EndPoint.Replace("[DEVICEID]", device.DeviceId);
                }


                var protocol = cachedMetaData.EndpointSSL ? "https" : "http";
                device.DeviceURI = $"{protocol}://{cachedMetaData.DnsHostName}:{cachedMetaData.EndpointPort}/devices/{device.Id}";

                result.Timings.Add(new ResultTiming() { Key = $"Load device config {cachedMetaData.DeviceConfiguration.Name} meta data from cache", Ms = fullSw.Elapsed.TotalMilliseconds });
                return result;
            }

            Logger.Trace("[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - Did not find in cache, populte manually;");

            var sw = Stopwatch.StartNew();
            device.DeviceType.Value = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);
            result.Timings.Add(new ResultTiming() { Key = "GetDeviceType", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();


            var deviceConfig = await _deviceConfigRepo.GetDeviceConfigurationAsync(device.DeviceConfiguration.Id);
            if (deviceConfig == null)
            {
                Logger.Trace("[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - No Device Configuration, can not populate (System Error, should be a required field.");
                result.AddSystemError($"Could Not Load Device Configuration with Device Configuration {device.DeviceConfiguration.Text}, Id={device.DeviceConfiguration.Id}.");
                return result;
            }

            result.Timings.Add(new ResultTiming() { Key = "GetDeviceConfiguration", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            device.DeviceLabel = deviceConfig.DeviceLabel;
            device.DeviceIdLabel = deviceConfig.DeviceIdLabel;
            device.DeviceNameLabel = deviceConfig.DeviceNameLabel;
            device.DeviceTypeLabel = deviceConfig.DeviceTypeLabel;

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

            sw.Restart();
            DeploymentInstance instance = null;

            var workflowKeys = new List<string>();
            foreach (var route in deviceConfig.Routes)
            {
                foreach (var module in route.PipelineModules)
                {
                    if (module.ModuleType.Value == Pipeline.Admin.Models.PipelineModuleType.Workflow)
                    {
                        sw.Restart();

                        var wfLoadResult = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(module.Module.Id, org, user);
                        result.Timings.AddRange(wfLoadResult.Timings);
                        result.Timings.Add(new ResultTiming() { Key = $"LoadFullDeviceWorkflowAsync - {wfLoadResult.Result.Name}", Ms = sw.Elapsed.TotalMilliseconds });
                        if (wfLoadResult.Successful && !workflowKeys.Contains(wfLoadResult.Result.Key))
                        {
                            workflowKeys.Add(wfLoadResult.Result.Key);
                            if (wfLoadResult.Result.Attributes != null)
                            {
                                foreach (var attribute in wfLoadResult.Result.Attributes)
                                {
                                    if (device.AttributeMetaData == null)
                                    {
                                        device.AttributeMetaData = new List<DeviceAdmin.Models.Attribute>();
                                    }

                                    if (!device.AttributeMetaData.Where(attr => attr.Key == attribute.Key).Any())
                                    {
                                        device.AttributeMetaData.Add(attribute);
                                    }
                                }
                            }

                            if (wfLoadResult.Result.StateMachines != null)
                            {
                                if (device.StateMachineMetaData == null)
                                {
                                    device.StateMachineMetaData = new List<StateMachine>();
                                }

                                foreach (var stateMachine in wfLoadResult.Result.StateMachines)
                                {
                                    if (!device.StateMachineMetaData.Where(attr => attr.Key == stateMachine.Key).Any())
                                    {
                                        device.StateMachineMetaData.Add(stateMachine);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Logger.Trace($"[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - Loaded attributes and state machines to device configurations.");

            if (instanceEH.Id != Guid.Empty.ToId())
            {             
                var instanceResult = await _deploymentInstanceManager.GetInstanceAsync(instanceEH.Id, org, user);
                if (!instanceResult.Successful)
                    throw new Exception(instanceResult.ErrorMessage);
                
                result.Timings.AddRange(instanceResult.Timings);
                instance = instanceResult.Result;

                result.Timings.Add(new ResultTiming() { Key = "GetInstanceAsync", Ms = sw.Elapsed.TotalMilliseconds });

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
                        foreach (var module in route.PipelineModules)
                        {
                            if (module.ModuleType.Value == Pipeline.Admin.Models.PipelineModuleType.Workflow)
                            {
                                sw.Restart();
                              
                                var wfLoadResult = await _deviceAdminManager.LoadFullDeviceWorkflowAsync(module.Module.Id, org, user);
                                result.Timings.AddRange(wfLoadResult.Timings);
                                result.Timings.Add(new ResultTiming() { Key = $"LoadFullDeviceWorkflowAsync - {wfLoadResult.Result.Name}", Ms = sw.Elapsed.TotalMilliseconds });
                                if (wfLoadResult.Successful && !workflowKeys.Contains(wfLoadResult.Result.Key))
                                {
                                    workflowKeys.Add(wfLoadResult.Result.Key);
                                    if (wfLoadResult.Result.InputCommands != null)
                                    {
                                        foreach (var inputCommand in wfLoadResult.Result.InputCommands)
                                        {
                                            var protocol = instance.InputCommandSSL ? "https://" : "http://";
                                            var endPoint = new InputCommandEndPoint
                                            {
                                                EndPoint = $"{protocol}{instance.DnsHostName}:{instance.InputCommandPort}/{deviceConfig.Key}/{route.Key}/{wfLoadResult.Result.Key}/{inputCommand.Key}/[DEVICEID]",
                                                InputCommand = inputCommand
                                            };

                                            foreach (var param in inputCommand.Parameters)
                                            {
                                                if (param.ParameterType.Value == ParameterTypes.State)
                                                {
                                                    sw.Restart();
                                                    param.StateSet.Value = await _deviceAdminManager.GetStateSetAsync(param.StateSet.Id, org, user);
                                                    result.Timings.Add(new ResultTiming() { Key = $"GetStateSetAsync - {param.StateSet.Text}", Ms = sw.Elapsed.TotalMilliseconds });
                                                }
                                                else if (param.ParameterType.Value == ParameterTypes.ValueWithUnit)
                                                {
                                                    sw.Restart();
                                                    param.UnitSet.Value = await _deviceAdminManager.GetAttributeUnitSetAsync(param.UnitSet.Id, org, user);
                                                    result.Timings.Add(new ResultTiming() { Key = $"GetAttributeUnitSetAsync - {param.UnitSet.Text}", Ms = sw.Elapsed.TotalMilliseconds });
                                                }
                                            }

                                            if (!endpoints.Where(end => end.EndPoint == endPoint.EndPoint).Any())
                                            {
                                                endpoints.Add(endPoint);
                                            }
                                        }
                                    }
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
                    device.InputCommandEndPoints = new List<InputCommandEndPoint>();
                }

                foreach (var ep in device.InputCommandEndPoints)
                {
                    ep.EndPoint = ep.EndPoint.Replace("[DEVICEID]", device.DeviceId);
                }
            }
            else
            {
                Logger.Trace($"[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - Instance not available, can't populate end points.");
                device.InputCommandEndPoints = new List<InputCommandEndPoint>();
            }

            var metaDataCache = new MetaDataCache()
            {
                DeviceType = device.DeviceType.Value,
                DeviceConfiguration = deviceConfig,
                InputCommandEndPoints = device.InputCommandEndPoints,
                AttributeMetaData = device.AttributeMetaData,
                StateMachineMetaData = device.StateMachineMetaData,
                PropertiesMetaData = device.PropertiesMetaData,
            };

            if (instance != null)
            {
                metaDataCache.EndpointPort = instance.InputCommandPort;
                metaDataCache.EndpointSSL = instance.InputCommandSSL;
                metaDataCache.DnsHostName = instance.DnsHostName;
            }

            Logger.Trace($"[DeviceConfigurationManager__PopulateDeviceConfigToDeviceAsync] - Added meta data to cache with cache key: ${GetDeviceConfigMetaDataKey(instanceEH.Id)}.");

            await _cacheProvider.AddToCollectionAsync(GetDeviceConfigMetaDataKey(instanceEH.Id), deviceConfig.Id, JsonConvert.SerializeObject(metaDataCache));
           
            result.Timings.Add(new ResultTiming() { Key = "LoadFullDeviceConfig", Ms = fullSw.Elapsed.TotalMilliseconds });

            return result;
        }

        public async Task<Route> CreateRouteWithDefaultsAsync(EntityHeader org)
        {
            var route = Route.Create();
            await _routeSupportRepo.SetDefaultPipelineModulesAsync(org, route);
            return route;
        }

        public Task<string> GetCustomPageForDeviceConfigAsync(string id, EntityHeader org, EntityHeader user)
        {
            return _deviceConfigRepo.GetCustomPageForDeviceConfigAsync(id);
        }

        public Task<string> GetQuickLinkCustomPageForDeviceConfigAsync(string id, EntityHeader org, EntityHeader user)
        {
            return _deviceConfigRepo.GetQuickLinkCustomPageForDeviceConfigAsync(id);
        }

        public async Task<HomePages> GetHomePagesAsync(string deviceConfigId, EntityHeader org, EntityHeader user)
        {
            var config = await _deviceConfigRepo.GetDeviceConfigurationAsync(deviceConfigId);
            return new HomePages()
            {
                 CustomMobilePage = config.CustomMobilePage,
                 CustomPage = config.CustomPage,
                 CustomPageQuickLink = config.CustomPageQuickLink
            };
        }
    }

    public class MetaDataCache
    {
        public DeviceConfiguration DeviceConfiguration { get; set; }
        public DeviceType DeviceType { get; set; }

        public bool EndpointSSL { get; set; }
        public int EndpointPort { get; set; }
        public string DnsHostName { get; set; }

       public List<InputCommandEndPoint> InputCommandEndPoints { get; set; }
       public List<DeviceAdmin.Models.Attribute> AttributeMetaData { get; set; }
       public List<StateMachine> StateMachineMetaData { get; set; }
        public List<DeviceAdmin.Models.CustomField> PropertiesMetaData { get; set; }
    }
}
