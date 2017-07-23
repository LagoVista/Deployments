using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.ContainerRepository_Title, Resources.DeploymentAdminResources.Names.ContainerRepository_Help, Resources.DeploymentAdminResources.Names.ContainerRepository_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]

    public class ContainerRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, INoSQLEntity
    {
        public ContainerRepository()
        {
            Tags = new List<TaggedContainer>();
            Registry = new EntityHeader() { Id = "CE5197F74A7D4A03A18FAC45FF046187", Text = "Docker Hub" };
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Registry, HelpResource:Resources.DeploymentAdminResources.Names.ContainerRepository_Registry_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader Registry { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_UserName, HelpResource: Resources.DeploymentAdminResources.Names.ContainerRepository_UserName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string UserName { get; set; }

        //NOTE: Password is required when creating, but not when updating, we set the FormField to false to make sure we can get past validation, validation on client will ensure this is put in place, also this is only temporary until we do a better job of managing containers.
        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Password, HelpResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Password_Help, FieldType: FieldTypes.Password, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string Password { get; set; }

        public String SecurePasswordId { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Namespace, HelpResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Namespace_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Namespace { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_RepositoryName, HelpResource: Resources.DeploymentAdminResources.Names.ContainerRepository_RepositoryName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string RepositoryName { get;  set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Preferred, HelpResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Preferred_Help, WaterMark:Resources.DeploymentAdminResources.Names.ContainerRepository_Preferred_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:false, IsUserEditable: true)]
        public EntityHeader PreferredTag { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.ContainerRepository_Tags,  FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources))]
        public List<TaggedContainer> Tags{ get; set; }

        public ContainerRepositorySummary CreateSummary()
        {
            return new ContainerRepositorySummary()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsPublic = IsPublic,
                Key = Key,
                PreferredTag = PreferredTag
            };
        }
    }

    public class ContainerRepositorySummary : SummaryData
    {
        public EntityHeader PreferredTag { get; set; }
    }
}
