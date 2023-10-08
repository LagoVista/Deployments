using LagoVista.Core.Attributes;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeploymentInstanceCredentials_Title, DeploymentAdminResources.Names.DeploymentInstanceCredentials_Help,
        DeploymentAdminResources.Names.DeploymentInstanceCredentials_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentInstanceCredentials
    {
        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstanceCredentials__Expires,
            HelpResource: DeploymentAdminResources.Names.DeploymentInstanceCredentials__Expires_Help,
            FieldType: FieldTypes.Date, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: false)]

        public string Expires { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstanceCredentials__UserId, FieldType: FieldTypes.Text,
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: true)]
        public string UserId { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeploymentInstanceCredentials__Password, FieldType: FieldTypes.Password,
            SecureIdFieldName:nameof(PasswordSecretId),
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: true)]
        public string Password { get; set; }

        public string PasswordSecretId { get; set; }

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            if(action == Actions.Create) 
            {
                if (String.IsNullOrEmpty(Password))
                    result.AddUserError("Password is a required field.");
            }
            else if(action == Actions.Update)
            {
                if (String.IsNullOrEmpty(Password) && String.IsNullOrEmpty(PasswordSecretId))
                    result.AddUserError("When updating a deployment instance credential either password secret id or password must be present.");
            }
        }
    }
}
