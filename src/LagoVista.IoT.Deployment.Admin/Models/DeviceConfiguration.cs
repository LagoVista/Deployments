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

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, Resources.DeploymentAdminResources.Names.DeviceConfiguration_Help,  Resources.DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfiguration :  LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity
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



        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
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
            var route = Routes.Where(rte => rte.MessageDefinitions.Where(msg => msg.Value.MessageId == messageId).Any()).FirstOrDefault();
            if(route == null)
            {
                return Routes.Where(rte => rte.IsDefault).FirstOrDefault();
            }
            else
            {
                return route;
            }
        }

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
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceConfiguration_Title, Resources.DeploymentAdminResources.Names.DeviceConfiguration_Help, Resources.DeploymentAdminResources.Names.DeviceConfiguration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceConfigurationSummary : LagoVista.Core.Models.SummaryData
    {

    }
}