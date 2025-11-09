// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c529128f154a72b12cbdd1dd909fb88c82b5674c415577f3bbc374ea6f9261bb
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.WiFiConnectionProfile_Title, DeploymentAdminResources.Names.WiFiConnectionProfile_Help,
        DeploymentAdminResources.Names.WiFiConnectionProfile_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        FactoryUrl: "/api/wificonnectionprofile/factory")]
    public class WiFiConnectionProfile : IIDEntity, IKeyedEntity, IValidateable, INamedEntity, IDescriptionEntity, IFormDescriptor, IFormDescriptorAdvanced
    {
        public WiFiConnectionProfile()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Name, FieldType: FieldTypes.Text, IsRequired: true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, IsRequired: true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_SSID, FieldType: FieldTypes.Text, IsRequired:true, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Ssid { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Password, FieldType: FieldTypes.Secret, SecureIdFieldName:nameof(PasswordSecretId), ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Password { get; set; }

        public string PasswordSecretId { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WiFiConnectionProfile_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string Description { get; set; }

        public List<string> GetAdvancedFields()
        {
            return new List<string>
            {
                nameof(Name),
                nameof(Key),
                nameof(Ssid),
                nameof(Password),
                nameof(Description)
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>
            {
                nameof(Name),
                nameof(Key),
                nameof(Ssid),
                nameof(Password),
            };
        }
    }
}
