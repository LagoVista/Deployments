﻿using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{

    public enum IntegrationTypes
    {
        [EnumLabel(Integration.IntegrationType_Twillio, DeploymentAdminResources.Names.IntegrationType_Twillio, typeof(DeploymentAdminResources))]
        Twillio,
        [EnumLabel(Integration.IntegrationType_PagerDuty, DeploymentAdminResources.Names.IntegrationType_PagerDuty, typeof(DeploymentAdminResources))]
        PagerDuty,
        [EnumLabel(Integration.IntegrationType_SendGrid, DeploymentAdminResources.Names.IntegrationType_SendGrid, typeof(DeploymentAdminResources))]
        SendGrid
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Integration_Title, DeploymentAdminResources.Names.Integration_Help, DeploymentAdminResources.Names.Integration_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class Integration : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, INoSQLEntity, IFormDescriptor
    {
        public const string IntegrationType_Twillio = "twilio";
        public const string IntegrationType_PagerDuty = "pagerduty";
        public const string IntegrationType_SendGrid = "sendgrid";


        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        public List<string> GetFormFields()
        {
            throw new NotImplementedException();
        }


        [FormField(LabelResource: DeploymentAdminResources.Names.IntegrationType, FieldType: FieldTypes.Picker, EnumType: typeof(IntegrationTypes), ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false, WaterMark: DeploymentAdminResources.Names.IntegrationType_Select_Watermark)]
        public string IntegrationType
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integeration_APIKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public string ApiKey
        {
            get; set;
        }


        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_Uri, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public string URI
        {
            get; set;
        }


        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_FromAddress, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public string FromAddress
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_SMS, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public string SMS
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_SMTP, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: false)]
        public string SMTP
        {
            get; set;
        }

        public string ApiKeySecretId
        {
            get; set;
        }

        public IntegrationSummary CreateSummary()
        {
            return new IntegrationSummary()
            {
                Description = Description,
                Id = Id,
                IsPublic = IsPublic,
                Key = Key,
                Name = Name
            };
        }
    }

    public class IntegrationSummary : SummaryData
    {

    }
}
