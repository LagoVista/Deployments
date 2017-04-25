using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceMessageDefinition_Title, Resources.DeploymentAdminResources.Names.DeviceMessageDefinition_Help, Resources.DeploymentAdminResources.Names.DeviceMessageDefinition_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceMessageDefinition : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IKeyedEntity,  IOwnedEntity, INoSQLEntity
    {
        public DeviceMessageDefinition()
        {
            Properties = new List<CustomField>();
        }

        public String DatabaseName { get; set; }

        public String EntityType { get; set; }


        [FormField(LabelResource:DeploymentAdminResources.Names.DeviceMessageDefinition_MessageId, HelpResource: DeploymentAdminResources.Names.DeviceMessageDefinition_MessageId_Help, ResourceType:typeof(DeploymentAdminResources))]
        public string MessageId { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]

        public String Key { get; set; }


        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public List<CustomField> Properties { get; set; }

        public DeviceMessageDefinitionSummary CreateSummary()
        {
            return new DeviceMessageDefinitionSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                IsPublic = IsPublic,
                Description = Description
            };
        }
    }

    public class DeviceMessageDefinitionSummary : LagoVista.Core.Models.SummaryData
    {

    }

}
