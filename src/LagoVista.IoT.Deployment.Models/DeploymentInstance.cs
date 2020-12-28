using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using LagoVista.IoT.Pipeline.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Instance_Title, DeploymentAdminResources.Names.Instance_Help, DeploymentAdminResources.Names.Instance_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentInstance : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity, IFormDescriptor
    {
        public DeploymentInstance()
        {
            Status = EntityHeader<DeploymentInstanceStates>.Create(DeploymentInstanceStates.Offline);
            InputCommandSSL = false;
            InputCommandPort = 80;
            SettingsValues = new List<AttributeValue>();
            CloudProvider = new EntityHeader() { Text = "Digital Ocean", Id = "378463ADF57B4C02B60FEF4DCB30F7E2" };
            DataStreams = new List<EntityHeader<DataStream>>();
            ApplicationCaches = new List<EntityHeader<ApplicationCache>>();
            Integrations = new List<EntityHeader<Integration>>();
            HealthCheckEnabled = true;
            TimeZone = new Deployment.Models.TimeZone()
            {
                Id = "UTC",
                Text = "(UTC) Coordinated Universal Time",
            };
        }

        public const string Status_Offline = "offline";

        public const string Status_DeployingRuntime = "deployingruntime";

        public const string Status_CreatingRuntime = "creatingruntime";
        public const string Status_StartingRuntime = "startingruntime";

        public const string Status_Initializing = "initializing";
        public const string Status_Starting = "starting";

        public const string Status_Running = "running";

        public const string Status_Paused = "paused";
        public const string Status_Pausing = "pausing";
        public const string Status_Stopping = "stopping";
        public const string Status_Stopped = "stopped";

        public const string Status_HostRestarting = "hostrestarting";
        public const string Status_UpdatingSolution = "updatingsolution";

        public const string Status_FatalError = "fatalerror";
        public const string Status_FailedToDeploy = "failedtodeploy";
        public const string Status_FailedToInitialize = "failedtoinitialize";
        public const string Status_FailedToStart = "failedtostart";
        public const string Status_HostFailedHealthCheck = "hostfailedhealthcheck";

        public const string DeploymentType_Cloud = "cloud";
        public const string DeploymentType_Managed = "managed";
        public const string DeploymentType_OnPremise = "onpremise";

        public const string DeploymentConfiguration_UWP = "uwp";
        public const string DeploymentConfiguration_SingleInstance = "singleinstance";
        public const string DeploymentConfiguration_Kubernetes = "kubernetes";
        public const string DeploymentConfiguration_DockerSwarm = "dockerswarm";

        public const string DeploymentQueueType_InMemory = "inmemory";
        public const string DeploymentQueueType_Kafka = "kafka";
        public const string DeploymentQueueType_RabbitMQ = "rabbitmq";
        public const string DeploymentQueueType_ServiceBus = "servicebus";

        public const string Deployment_Logging_Local = "local";
        public const string Deployment_Logging_Cloud = "cloud";

        public const string Deployment_WorkingStorage_Local = "local";
        public const string Deployment_WorkingStorage_Cloud = "cloud";

        public const string Deployment_MessageArchiveStorage_FileSystem = "local";
        public const string Deployment_MessageArchiveStorage_Cloud = "cloud";
        public const string Deployment_MessageArchiveStorage_Elastic = "elastic";

        public const string NuvIoTEdition_App = "app";
        public const string NuvIoTEdition_Container = "container";
        public const string NuvIoTEdition_Cluster = "cluster";

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_IsDeployed, HelpResource: DeploymentAdminResources.Names.Instance_IsDeployed_Help, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public bool IsDeployed { get; set; }

        private EntityHeader<DeploymentInstanceStates> _status;

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Status, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader<DeploymentInstanceStates> Status
        {
            get { return _status; }
            set
            {
                _status = value;
                StatusTimeStamp = DateTime.UtcNow.ToJSONString();
            }
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_TimeZone, IsRequired:true, FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public LagoVista.IoT.Deployment.Models.TimeZone TimeZone { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_StatusTimeStamp, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string StatusTimeStamp { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_StatusDetails, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string StatusDetails{ get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Host, HelpResource: DeploymentAdminResources.Names.Instance_Host_Help, WaterMark: DeploymentAdminResources.Names.Instance_Host_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeploymentHost> Host { get; set; }

        /// <summary>
        /// This is the primary host that will be be used as an access point into the instance, if the instance consists of many machines, this will manage all the other hosts for a clustered version of an instance.
        /// </summary>
        EntityHeader<DeploymentHost> _primaryHost;
        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Host, HelpResource: DeploymentAdminResources.Names.Instance_Host_Help, WaterMark: DeploymentAdminResources.Names.Instance_Host_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeploymentHost> PrimaryHost
        {
            get
            {
                if (EntityHeader.IsNullOrEmpty(_primaryHost))
                {
                    _primaryHost = Host;
                    return Host;
                }
                else
                {
                    return _primaryHost;
                }
            }
            set { _primaryHost = value; }
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_DataStreams, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<DataStream>> DataStreams { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Caches, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<ApplicationCache>> ApplicationCaches { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_IsPublic, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_Integrations, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<Integration>> Integrations { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name,
            };
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_UpSince, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: false)]
        public string UpSince { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_Subscription, WaterMark: DeploymentAdminResources.Names.Host_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_Size, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Host_SelectSize)]
        public EntityHeader Size { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_DeviceRepo, HelpResource: DeploymentAdminResources.Names.Instance_DeviceRepo_Help, WaterMark: DeploymentAdminResources.Names.Instance_DeviceRepo_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public EntityHeader<DeviceRepository> DeviceRepository { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_SettingsValues, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public List<AttributeValue> SettingsValues { get; set; }

        public Dictionary<string, object> PropertyBag { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_Version, HelpResource: DeploymentAdminResources.Names.DeploymentInstance_Version_Help, WaterMark: DeploymentAdminResources.Names.DeploymentInstance_Version_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public EntityHeader Version { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_ContainerRepository, WaterMark: DeploymentAdminResources.Names.Host_ContainerRepository_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader ContainerRepository { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_HealthCheckEnabled, HelpResource: DeploymentAdminResources.Names.DeploymentInstance_HealthCheckEnabled, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool HealthCheckEnabled { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_ContainerTag, WaterMark: DeploymentAdminResources.Names.Host_ContainerTag_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader ContainerTag { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_SharedAccessKey1, HelpResource: DeploymentAdminResources.Names.DeploymentInstance_SharedAccessKey_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string SharedAccessKey1 { get; set; }
        public string SharedAccessKeySecureId1 { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstance_SharedAccessKey2, HelpResource: DeploymentAdminResources.Names.DeploymentInstance_SharedAccessKey_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string SharedAccessKey2 { get; set; }
        public string SharedAccessKeySecureId2 { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_LastPing, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string LastPing { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_DNSName, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string DnsHostName { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_InputCommandSSL, FieldType: FieldTypes.CheckBox, HelpResource: DeploymentAdminResources.Names.Instance_InputCommandSSL_Help, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public bool InputCommandSSL { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_InputCommandPort, FieldType: FieldTypes.Integer, HelpResource: DeploymentAdminResources.Names.Instance_InputCommandPort_Help, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public int InputCommandPort { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Host_CloudProvider, HelpResource: DeploymentAdminResources.Names.Host_CloudProvider_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: true)]
        public EntityHeader CloudProvider { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_PrimaryCacheType, EnumType: (typeof(CacheTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Instance_PrimaryCacheType_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<CacheTypes> PrimaryCacheType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_PrimaryCache, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Instance_PrimaryCache_Select, IsRequired: false, IsUserEditable: true)]
        public EntityHeader<ApplicationCache> PrimaryCache { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_DeploymentType, EnumType: (typeof(DeploymentTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Instance_DeploymentType_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeploymentTypes> DeploymentType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_DeploymentConfiguration, EnumType: (typeof(DeploymentConfigurations)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Instance_DeploymentConfiguration_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeploymentConfigurations> DeploymentConfiguration { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.NuvIoT_Edition, EnumType: (typeof(NuvIoTEditions)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.NuvIoTEdition_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<NuvIoTEditions> NuvIoTEdition { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WorkingStorage, EnumType: (typeof(WorkingStorage)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.WorkingStorage_Select, IsUserEditable: true)]
        public EntityHeader<WorkingStorage> WorkingStorage { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentQueueType_QueueTechnology, EnumType: (typeof(QueueTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources),
            WaterMark: DeploymentAdminResources.Names.DeploymentQueueType_QueueTechnology_Select, HelpResource: DeploymentAdminResources.Names.DeploymentQueueType_QueueTechnology_Help, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<QueueTypes> QueueType { get; set; }


        public EntityHeader<IConnectionSettings> QueueConnection { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Deployment_Logging, EnumType: (typeof(LogStorage)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources),
            WaterMark: DeploymentAdminResources.Names.Deployment_Logging_Select, HelpResource: DeploymentAdminResources.Names.Deployment_Logging_Help, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<LogStorage> LogStorage { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_DebugMode, HelpResource: DeploymentAdminResources.Names.Instance_DebugMode_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public bool DebugMode { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Solution, WaterMark: DeploymentAdminResources.Names.Instance_Solution_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public EntityHeader<Solution> Solution { get; set; }
        public DeploymentInstanceSummary CreateSummary()
        {
            var summary = new DeploymentInstanceSummary()
            {
                Description = Description,
                Name = Name,
                Key = Key,
                Id = Id,
                IsPublic = IsPublic,
                IsDeployed = IsDeployed,
                Status = Status,
                NuvIoTEdition = EntityHeader.IsNullOrEmpty(NuvIoTEdition) ? "???" : NuvIoTEdition.Text,
                DeploymentType = EntityHeader.IsNullOrEmpty(DeploymentType) ? "???" : DeploymentType.Text,
                WorkingStorage = EntityHeader.IsNullOrEmpty(WorkingStorage) ? "???" : WorkingStorage.Text,
                QueueType = EntityHeader.IsNullOrEmpty(QueueType) ? "???" : QueueType.Text,
                OrgId = OwnerOrganization.Id,
                OrgName = OwnerOrganization.Text,
                DeviceRepoId = DeviceRepository?.Id,
                DeviceRepoName = DeviceRepository?.Text
            };

            if (EntityHeader.IsNullOrEmpty(DeviceRepository))
            {
                summary.DeviceRepoId = DeviceRepository.Id;
                summary.DeviceRepoName = DeviceRepository.Text;
            };

            return summary;
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(DeploymentInstance.Name),
                nameof(DeploymentInstance.Key),
                nameof(DeploymentInstance.DeploymentType),
                nameof(DeploymentInstance.DeploymentConfiguration),
                nameof(DeploymentInstance.DnsHostName),
                nameof(DeploymentInstance.Status),
                nameof(DeploymentInstance.IsDeployed),
                nameof(DeploymentInstance.Subscription),
                nameof(DeploymentInstance.Size),
                nameof(DeploymentInstance.CloudProvider),
                nameof(DeploymentInstance.ContainerRepository),
                nameof(DeploymentInstance.ContainerTag),
                nameof(DeploymentInstance.DeviceRepository),
                nameof(DeploymentInstance.Solution),
            };
        }

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            if (action == Actions.Create)
            {
                if (String.IsNullOrEmpty(SharedAccessKey1))
                {
                    result.AddSystemError("Upon creation, Shared Access Key 1 is Required.");
                }

                if (String.IsNullOrEmpty(SharedAccessKey2))
                {
                    result.AddSystemError("Upon creation, Shared Access Key 2 is Required.");
                }
            }

            if (NuvIoTEdition?.Value == NuvIoTEditions.Container)
            {
                if (EntityHeader.IsNullOrEmpty(ContainerRepository))
                {
                    result.AddSystemError("Container Repository Is Required for NuvIoT Container Editions.");
                }

                if (EntityHeader.IsNullOrEmpty(ContainerTag))
                {
                    result.AddSystemError("Container Tag Is Required for NuvIoT Container Editions.");
                }

                if (EntityHeader.IsNullOrEmpty(Size))
                {
                    result.AddSystemError("Image Size is a Required FIeld.");
                }

                if (EntityHeader.IsNullOrEmpty(WorkingStorage))
                {
                    result.AddSystemError("Image Size is a Required FIeld.");
                }
            }
            else if (NuvIoTEdition?.Value == NuvIoTEditions.Cluster)
            {
                if (EntityHeader.IsNullOrEmpty(WorkingStorage))
                {
                    result.AddSystemError("Image Size is a Required FIeld.");
                }
            }

            if (action == Actions.Update)
            {
                if (String.IsNullOrEmpty(SharedAccessKey1) && String.IsNullOrEmpty(SharedAccessKeySecureId1))
                {
                    result.AddSystemError("Upon creation, Shared Access Key 1 or Shared Access Secure Id 1 is Required.");
                }

                if (String.IsNullOrEmpty(SharedAccessKey2) && String.IsNullOrEmpty(SharedAccessKeySecureId2))
                {
                    result.AddSystemError("Upon updates, Shared Access Key 2 or Shared Access Secure Id 2 is Required.");
                }
            }

            if (!EntityHeader.IsNullOrEmpty(PrimaryCacheType) && PrimaryCacheType.Value == CacheTypes.Redis)
            {
                if (EntityHeader.IsNullOrEmpty(PrimaryCache))
                {
                    result.AddSystemError("Must provide primary cache type.");
                }
            }
        }
    }

    public class DeploymentInstanceSummary : SummaryData
    {
        public EntityHeader<DeploymentInstanceStates> Status { get; set; }
        public bool IsDeployed { get; set; }
        public string NuvIoTEdition { get; set; }
        public string DeploymentType { get; set; }
        public string WorkingStorage { get; set; }
        public string QueueType { get; set; }

        public string DeviceRepoId { get; set; }
        public string DeviceRepoName { get; set; }
        public string OrgName { get; set; }
        public string OrgId { get; set; }
    }
}
