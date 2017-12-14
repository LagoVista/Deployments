using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Linq;
using System.Collections.Generic;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help,  DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfiguration :  LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity, IFormDescriptor
    {
        public DeviceConfiguration()
        {
            Routes = new List<Route>();
            Properties = new List<CustomField>();
            ConfigurationVersion = 0.1;
        }

        public String DatabaseName { get; set; }

        public String EntityType { get; set; }

        public double ConfigurationVersion { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }
        
      
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


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Routes, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
        public List<Route> Routes { get; set; }

        public Route FindRoute(string messageId)
        {
            var route = Routes.Where(rte => rte.MessageDefinition.Value.MessageId == messageId).FirstOrDefault();
            if(route == null)
            {
                return Routes.Where(rte => rte.IsDefault).FirstOrDefault();
            }
            else
            {
                return route;
            }
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Properties, HelpResource:DeploymentAdminResources.Names.DeviceConfiguration_Properties_Help, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
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
                Description = Description
            };
        }

        public List<string> GetFormFields()
        {
                return new List<string>()
                {
                    nameof(DeviceConfiguration.Name),
                    nameof(DeviceConfiguration.Key),
                    nameof(DeviceConfiguration.Description),
                    nameof(DeviceConfiguration.Properties),
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
            foreach(var route in Routes)
            {
                route.DeepValidation(result);
            }
        }

    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, DeploymentAdminResources.Names.DeviceConfiguration_Help, DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfigurationSummary : LagoVista.Core.Models.SummaryData
    {

    }
}