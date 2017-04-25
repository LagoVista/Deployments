using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Instance_Title, Resources.DeploymentAdminResources.Names.Instance_Help, Resources.DeploymentAdminResources.Names.Instance_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentInstance : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity
    {
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Host, HelpResource:Resources.DeploymentAdminResources.Names.Instance_Host_Help, WaterMark: DeploymentAdminResources.Names.Instance_Host_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader Host { get; set; }

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

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Solution, WaterMark:DeploymentAdminResources.Names.Instance_Solution_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:true)]
        public EntityHeader<Solution> Solution { get; set; }
        public DeploymentInstanceSummary CreateSummary()
        {
            return new DeploymentInstanceSummary()
            {
                Description = Description,
                Name = Name,
                Key = Key,
                Id = Id,
                IsPublic = IsPublic
            };
        }

    }

    public class DeploymentInstanceSummary : SummaryData
    {

    }
}
