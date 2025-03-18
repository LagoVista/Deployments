using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help,
        DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Cloneable:true,
        SaveUrl: "/api/deviceconfig", FactoryUrl: "/api/deviceconfig/factory", GetUrl: "/api/deviceconfig/{id}", GetListUrl: "/api/deviceconfigs", DeleteUrl: "/api/deviceconfig/{id}",
        ListUIUrl: "/iotstudio/device/deviceconfigurations", EditUIUrl: "/iotstudio/device/deviceconfiguration/{id}", CreateUIUrl: "/iotstudio/device/deviceconfiguration/add",
        Icon: "icon-ae-device-config")]
    public class DeviceConfiguration : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IFormDescriptorAdvanced, IFormConditionalFields,
        IFormDescriptorAdvancedCol2, IIconEntity, ISummaryFactory, ICategorized
    {
        public DeviceConfiguration()
        {
            Id = Guid.NewGuid().ToId();
            Routes = new List<Route>();
            Properties = new List<CustomField>();
            ConfigurationVersion = 0.1;
            DeviceTypeLabel = DeploymentAdminResources.DeviceConfiguration_DeviceTypeLabel_Default;
            DeviceIdLabel = DeploymentAdminResources.DeviceConfiguration_DeviceIdLabel_Default;
            DeviceNameLabel = DeploymentAdminResources.DeviceConfiguration_DeviceNameLabel_Default;
            DeviceLabel = DeploymentAdminResources.DeviceConfiguration_DeviceLabel_Default;
            SensorDefinitions = new List<SensorDefinition>();
            ErrorCodes = new List<DeviceErrorCode>();
            MessageWatchDogs = new List<MessageWatchDog>();
            Icon = "icon-ae-device-config";
        }

        public double ConfigurationVersion { get; set; }


        [FKeyProperty(nameof(StateSet), typeof(StateSet), "CustomStatusType.Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType_Help, WaterMark: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<StateSet> CustomStatusType { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceIdLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceIdLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceIdLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceNameLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceNameLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceNameLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceTypeLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceTypeLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceTypeLabel { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogEnabled_Default, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogEnabled_Default_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public bool WatchdogEnabledDefault { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogTimeout, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogTimeout_Help, FieldType: FieldTypes.Integer, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public int? WatchdogSeconds { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceErrorCodes, FactoryUrl: "/api/errorcode/factory", FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<DeviceErrorCode> ErrorCodes { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_MessageWatchDogs, FactoryUrl: "/api/deviceconfig/messagewatchdog/factory", FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<MessageWatchDog> MessageWatchDogs { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_SensorDefintions, FactoryUrl: "/api/device/sensor/factory", FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<SensorDefinition> SensorDefinitions { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfig_Commands, HelpResource: DeploymentAdminResources.Names.DeviceConfig_Commands_Help, FactoryUrl: "/api/deviceconfig/command/factory", FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<DeviceCommand> Commands { get; set; } = new List<DeviceCommand>();



        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomPage, HelpResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomPage_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string CustomPage { get; set; }
        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomMobilePage, HelpResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomMobilePage_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string CustomMobilePage { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomPage_QuickLink, HelpResource: DeploymentAdminResources.Names.DeviceConfiugration_CustomPage_QuickLink_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string CustomPageQuickLink { get; set; }
        

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Routes, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/deviceconfig/route/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<Route> Routes { get; set; }

        public Route FindRoute(string messageId)
        {
            var route = Routes.Where(rte => rte.MessageDefinition.Value.MessageId == messageId).FirstOrDefault();
            if (route == null)
            {
                return Routes.Where(rte => rte.IsDefault).FirstOrDefault();
            }
            else
            {
                return route;
            }
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Properties, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_Properties_Help, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<CustomField> Properties { get; set; }

        public static DeviceConfiguration Create(Organization org, AppUser appUser)
        {
            return new DeviceConfiguration()
            {
                Id = Guid.NewGuid().ToId(),
                CreatedBy = appUser.ToEntityHeader(),
                CreationDate = DateTime.Now.ToJSONString(),
                LastUpdatedBy = appUser.ToEntityHeader(),
                LastUpdatedDate = DateTime.Now.ToJSONString(),
                OwnerOrganization = org.ToEntityHeader(),
            };
        }

        public DeviceConfigurationSummary CreateSummary()
        {
            return new DeviceConfigurationSummary()
            {
                Id = Id,
                IsPublic = IsPublic,
                Key = Key,
                Name = Name,
                Icon = Icon,
                Description = Description,
                Category = Category?.Text,
                CategoryId = Category?.Id,
                CategoryKey = Category?.Key,
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
                {
                    nameof(DeviceConfiguration.Name),
                    nameof(DeviceConfiguration.Key),
                    nameof(DeviceConfiguration.Icon),
                    nameof(DeviceConfiguration.Category),
                    nameof(DeviceConfiguration.Description),
                    nameof(DeviceConfiguration.Routes),
                };
        }

        /// <summary>
        /// Perform a deep validation,normal validation only ensures that the correct properties are set but doesn't load
        /// and objects that are loaded via relationships.  This assumes that all the modules and their dependencies have been loaded.
        /// </summary>
        /// <param name="result"></param>
        public void DeepValidation(ValidationResult result)
        {
            foreach (var route in Routes)
            {
                route.DeepValidation(result);
            }
        }


        [CustomValidator]
        public void Validate(ValidationResult result)
        {
            if (WatchdogEnabledDefault && !WatchdogSeconds.HasValue)
            {
                result.AddUserError("Watchdog Internal is required.");
            }
            else if (!WatchdogEnabledDefault)
            {
                WatchdogSeconds = null;
            }

            if (ErrorCodes.Count != ErrorCodes.GroupBy(err => err.Key).Count())
            {
                result.AddUserError("Error codes for a device configuration must be unique..");
            }
        }

        public List<string> GetAdvancedFields()
        {
            return new List<string>()
                {
                    nameof(DeviceConfiguration.Name),
                    nameof(DeviceConfiguration.Key),
                    nameof(DeviceConfiguration.Icon),
                    nameof(DeviceConfiguration.Category),
                    nameof(DeviceConfiguration.CustomStatusType),
                    nameof(DeviceConfiguration.Description),
                    nameof(DeviceConfiguration.Routes),
                    nameof(DeviceConfiguration.SensorDefinitions),
                };
        }

        Core.Interfaces.ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }

        public List<string> GetAdvancedFieldsCol2()
        {
            return new List<string>()
                {
                    nameof(DeviceConfiguration.DeviceLabel),
                    nameof(DeviceConfiguration.DeviceIdLabel),
                    nameof(DeviceConfiguration.DeviceNameLabel),
                    nameof(DeviceConfiguration.DeviceTypeLabel),
                    nameof(DeviceConfiguration.CustomPage),
                    nameof(DeviceConfiguration.CustomMobilePage),
                    nameof(DeviceConfiguration.CustomPageQuickLink),
                    nameof(DeviceConfiguration.Properties),
                    nameof(DeviceConfiguration.Commands),
                    nameof(DeviceConfiguration.WatchdogEnabledDefault),
                    nameof(DeviceConfiguration.WatchdogSeconds),                    
                    nameof(DeviceConfiguration.MessageWatchDogs),
                };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(WatchdogSeconds) },
                Conditionals = new List<FormConditional>()
                 {
                      new FormConditional()
                      {
                          Field = nameof(WatchdogEnabledDefault),
                          Value = "true",
                          VisibleFields = new List<string>() { nameof(WatchdogSeconds) },
                          RequiredFields = new List<string> { nameof(WatchdogSeconds) }
                      }
                 }
            };
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfigurations_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help,
      DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-device-config",
      SaveUrl: "/api/deviceconfig", FactoryUrl: "/api/deviceconfig/factory", GetUrl: "/api/deviceconfig/{id}", GetListUrl: " /api/deviceconfigs", DeleteUrl: "/api/deviceconfig/{id}")]
    public class DeviceConfigurationSummary : SummaryData
    {
    }
}