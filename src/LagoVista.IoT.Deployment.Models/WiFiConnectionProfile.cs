using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.WiFiConnectionProfile_Title, DeploymentAdminResources.Names.WiFiConnectionProfile_Help, DeploymentAdminResources.Names.WiFiConnectionProfile_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class WiFiConnectionProfile : IIDEntity, IKeyedEntity, INamedEntity, IDescriptionEntity
    {
        public string Id { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Name, FieldType: FieldTypes.Text, IsRequired: true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, IsRequired: true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_SSID, FieldType: FieldTypes.Text, IsRequired:true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Ssid { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Password, FieldType: FieldTypes.Password, IsRequired:true, SecureIdFieldName:nameof(PasswordSecretId), ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Password { get; set; }

        public string PasswordSecretId { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Description { get; set; }
    }
}
