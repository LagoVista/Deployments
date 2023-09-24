using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
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
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help, DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfiguration : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity, IFormDescriptor
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

        public String DatabaseName { get; set; }

        public String EntityType { get; set; }

        public double ConfigurationVersion { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType_Help, WaterMark: DeploymentAdminResources.Names.DeviceConfiguration_CustomStatusType_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<StateSet> CustomStatusType { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceIdLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceIdLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceIdLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceNameLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceNameLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceNameLabel { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceTypeLabel, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceTypeLabel_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String DeviceTypeLabel { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogEnabled_Default, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogEnabled_Default_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public bool WatchdogEnabledDefault { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogTimeout, HelpResource: DeploymentAdminResources.Names.DeviceConfiguration_WatchDogTimeout_Help, FieldType: FieldTypes.Integer, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public int? WatchdogSeconds { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_DeviceErrorCodes, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<DeviceErrorCode> ErrorCodes { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_MessageWatchDogs, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<MessageWatchDog> MessageWatchDogs { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_SensorDefintions, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<SensorDefinition> SensorDefinitions { get; set; }

        public string Icon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_IsPublic, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name,
            };
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Routes, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
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
                Description = Description
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
                {
                    nameof(DeviceConfiguration.Name),
                    nameof(DeviceConfiguration.Key),
                    nameof(DeviceConfiguration.CustomStatusType),
                    nameof(DeviceConfiguration.Description),
					nameof(DeviceConfiguration.DeviceLabel),
					nameof(DeviceConfiguration.DeviceIdLabel),
					nameof(DeviceConfiguration.DeviceNameLabel),
					nameof(DeviceConfiguration.DeviceTypeLabel),
					nameof(DeviceConfiguration.WatchdogEnabledDefault),
					nameof(DeviceConfiguration.WatchdogSeconds),
                    nameof(DeviceConfiguration.Routes),
					nameof(DeviceConfiguration.Properties),
					nameof(DeviceConfiguration.ErrorCodes),
					nameof(DeviceConfiguration.MessageWatchDogs),
					nameof(DeviceConfiguration.SensorDefinitions),
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
            if(WatchdogEnabledDefault && !WatchdogSeconds.HasValue)
            {
                result.AddUserError("Watchdog Internal is required.");
            }
            else if(!WatchdogEnabledDefault)
            {
                WatchdogSeconds = null;
            }

            if(ErrorCodes.Count != ErrorCodes.GroupBy(err=>err.Key).Count())
            {
                result.AddUserError("Error codes for a device configuration must be unique..");
            }
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help, DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfigurationSummary : LagoVista.Core.Models.SummaryData
    {
        public string Icon { get; set; }
    }
}