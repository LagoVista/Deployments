using System;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.Core.Models;
using LagoVista.Core;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum TagContainer_Status
    {
        [EnumLabel(TaggedContainer.TaggedContainer_Status_Prerelease, DeploymentAdminResources.Names.TaggedContainer_Status_Prerelease, typeof(DeploymentAdminResources))]
        Prerelease,
        [EnumLabel(TaggedContainer.TaggedContainer_Status_Alpha, DeploymentAdminResources.Names.TaggedContainer_Status_Alpha, typeof(DeploymentAdminResources))]
        Alpha,
        [EnumLabel(TaggedContainer.TaggedContainer_Status_Beta, DeploymentAdminResources.Names.TaggedContainer_Status_Beta, typeof(DeploymentAdminResources))]
        Beta,
        [EnumLabel(TaggedContainer.TaggedContainer_Status_Production, DeploymentAdminResources.Names.TaggedContainer_Status_Production, typeof(DeploymentAdminResources))]
        Production,
        [EnumLabel(TaggedContainer.TaggedContainer_Status_Deprecated, DeploymentAdminResources.Names.TaggedContainer_Status_Deprecated, typeof(DeploymentAdminResources))]
        Deprecated,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.TaggedContainer_Title, Resources.DeploymentAdminResources.Names.TaggedContainer_Help, Resources.DeploymentAdminResources.Names.TaggedContainer_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class TaggedContainer : IKeyedEntity
    {
        public const string TaggedContainer_Status_Prerelease = "prerelease";
        public const string TaggedContainer_Status_Alpha = "alpha";
        public const string TaggedContainer_Status_Beta = "beta";
        public const string TaggedContainer_Status_Production = "production";
        public const string TaggedContainer_Status_Deprecated = "deprecated";

        public TaggedContainer()
        {
            Status = EntityHeader<TagContainer_Status>.Create(TagContainer_Status.Prerelease);
            CreationDate = DateTime.UtcNow.ToJSONString();
        }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.TaggedContainer_Status, EnumType: (typeof(TagContainer_Status)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.TaggedContainer_Status_Select, IsRequired: true)]
        public EntityHeader<TagContainer_Status> Status { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.TaggedContainer_ReleaseNotes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string ReleaseNotes { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.TaggedContainer_CreationDate, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public String CreationDate { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.TaggedContainer_Tag, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Tag { get; set; }
    }
}
