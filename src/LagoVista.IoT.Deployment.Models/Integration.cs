using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Resources;
using Newtonsoft.Json;
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

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Integration_Title, DeploymentAdminResources.Names.Integration_Help, DeploymentAdminResources.Names.Integration_Description, 
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        GetListUrl: "/api/integrations", GetUrl: "/api/integration/{id}", SaveUrl: "/api/integration", FactoryUrl: "/api/integration/factory", DeleteUrl: "/api/integration/{id}")]
    public class Integration : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IFormConditionalFields, ISummaryFactory
    {
        public const string IntegrationType_Twillio = "twillio";
        public const string IntegrationType_PagerDuty = "pagerduty";
        public const string IntegrationType_SendGrid = "sendgrid";



        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Integration.Name),
                nameof(Integration.Key),
                nameof(Integration.IntegrationType),
                nameof(Integration.AccountId),
                nameof(Integration.FromAddress),
                nameof(Integration.Uri),
                nameof(Integration.Sms),
                nameof(Integration.SmtpAddress),
                nameof(Integration.ApiKey),
                nameof(Integration.Description),
            };
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.IntegrationType, FieldType: FieldTypes.Picker, EnumType: typeof(IntegrationTypes), ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true, WaterMark: DeploymentAdminResources.Names.IntegrationType_Select_Watermark)]
        public EntityHeader IntegrationType
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integeration_APIKey, FieldType: FieldTypes.Text, SecureIdFieldName:nameof(ApiKeySecretId), ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string ApiKey
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_Uri, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string Uri
        {
            get; set;
        }


        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_AccountId, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string AccountId
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_FromAddress, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string FromAddress
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_SMS, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string Sms
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_RoutingKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string RoutingKey
        {
            get; set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Integration_SMTP, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string SmtpAddress
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
                Name = Name,
                IntegrationType = IntegrationType.Text
            };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(Sms), nameof(ApiKey), nameof(FromAddress), nameof(AccountId), nameof(SmtpAddress), nameof(RoutingKey) },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                         Field = nameof(IntegrationType),
                         Value = IntegrationType_Twillio,
                         VisibleFields = new List<string>() { nameof(Sms), nameof(ApiKey)},
                         RequiredFields = new List<string>() { nameof(Sms), nameof(ApiKey)},
                    },
                    new FormConditional()
                    {
                         Field = nameof(IntegrationType),
                         Value = IntegrationType_SendGrid,
                         VisibleFields = new List<string>() { nameof(FromAddress), nameof(ApiKey)},
                         RequiredFields = new List<string>() { nameof(FromAddress), nameof(ApiKey)},
                    },
                    new FormConditional()
                    {
                         Field = nameof(IntegrationType),
                         Value = IntegrationType_PagerDuty,
                         VisibleFields = new List<string>() { nameof(RoutingKey), nameof(ApiKey)},
                         RequiredFields = new List<string>() { nameof(RoutingKey), nameof(ApiKey)},
                    },
                }
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Integration_Title, DeploymentAdminResources.Names.Integration_Help, DeploymentAdminResources.Names.Integration_Description,
        EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources),
        GetListUrl: "/api/integrations", GetUrl: "/api/integration/{id}", SaveUrl: "/api/integration", FactoryUrl: "/api/integration/factory", DeleteUrl: "/api/integration/{id}")]
    public class IntegrationSummary : SummaryData
    {
        public string IntegrationType { get; set; }
    }
}
