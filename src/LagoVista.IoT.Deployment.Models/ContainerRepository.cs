using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.ContainerRepository_Title, DeploymentAdminResources.Names.ContainerRepository_Help, 
        DeploymentAdminResources.Names.ContainerRepository_Description, 
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        GetListUrl: "/api/container/repos", GetUrl: "/api/container/repo/{id}", FactoryUrl: "/api/container/repo/factory", SaveUrl: "/api/container/repo")]

    public class ContainerRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, INoSQLEntity, IFormDescriptor, IKeyedEntity
    {
        public ContainerRepository()
        {
            Tags = new List<TaggedContainer>();
            Registry = new EntityHeader() { Id = "CE5197F74A7D4A03A18FAC45FF046187", Text = "Docker Hub" };
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_IsPublic, HelpResource: DeploymentAdminResources.Names.Common_IsPublic_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsPublic { get; set; }

        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_Registry, HelpResource:DeploymentAdminResources.Names.ContainerRepository_Registry_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader Registry { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_UserName, HelpResource: DeploymentAdminResources.Names.ContainerRepository_UserName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string UserName { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_IsDefautForRuntime, HelpResource: DeploymentAdminResources.Names.ContainerRepository_IsDefautForRuntime_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsDefaultForRuntime { get; set; }


        //NOTE: Password is required when creating, but not when updating, we set the FormField to false to make sure we can get past validation, validation on client will ensure this is put in place, also this is only temporary until we do a better job of managing containers.
        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_Password, HelpResource: DeploymentAdminResources.Names.ContainerRepository_Password_Help, FieldType: FieldTypes.Password,
           SecureIdFieldName:nameof(SecurePasswordId), ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string Password { get; set; }

        public String SecurePasswordId { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_Namespace, HelpResource: DeploymentAdminResources.Names.ContainerRepository_Namespace_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Namespace { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_RepositoryName, HelpResource: DeploymentAdminResources.Names.ContainerRepository_RepositoryName_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string RepositoryName { get;  set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_Preferred, HelpResource: DeploymentAdminResources.Names.ContainerRepository_Preferred_Help, WaterMark:DeploymentAdminResources.Names.ContainerRepository_Preferred_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:false, IsUserEditable: true)]
        public EntityHeader PreferredTag { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ContainerRepository_Tags,  FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources))]
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

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(ContainerRepository.Name),
                nameof(ContainerRepository.Key),
                nameof(ContainerRepository.Registry),
                nameof(ContainerRepository.Namespace),
                nameof(ContainerRepository.IsDefaultForRuntime),
                nameof(ContainerRepository.RepositoryName),
                nameof(ContainerRepository.PreferredTag),
                nameof(ContainerRepository.UserName),
                nameof(ContainerRepository.Password),
                nameof(ContainerRepository.Tags),
            };
        }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Key = Key,
                Text = Name,
            };
        }
    }

    public class ContainerRepositorySummary : SummaryData
    {
        public EntityHeader PreferredTag { get; set; }
    }
}
