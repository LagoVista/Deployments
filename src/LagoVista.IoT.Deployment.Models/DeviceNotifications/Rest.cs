// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fd70c05a2cb1cd0182a643299d495781acf05676f67a95220a268b49ed1ab91b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models.DeviceNotifications
{
    public enum RestMethods
    {
        [EnumLabel(Rest.REST_METHOD_GET, DeploymentAdminResources.Names.RestNotification_Method_GET, typeof(DeploymentAdminResources))]
        GET,
        [EnumLabel(Rest.REST_METHOD_POST, DeploymentAdminResources.Names.RestNotification_Method_POST, typeof(DeploymentAdminResources))]
        POST,
        [EnumLabel(Rest.REST_METHOD_PUT, DeploymentAdminResources.Names.RestNotification_Method_PUT, typeof(DeploymentAdminResources))]
        PUT,
        [EnumLabel(Rest.REST_METHOD_DELETE, DeploymentAdminResources.Names.RestNotification_Method_DELETE, typeof(DeploymentAdminResources))]
        DELETE,
        [EnumLabel(Rest.REST_METHOD_PATCH, DeploymentAdminResources.Names.RestNotification_Method_PATCH, typeof(DeploymentAdminResources))]
        PATCH,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.RestNotification_Title, DeploymentAdminResources.Names.RestNotification_Help,
            DeploymentAdminResources.Names.MqttNotifications_Help, EntityDescriptionAttribute.EntityTypes.ChildObject, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
            FactoryUrl: "/api/notification/rest/factory")]
    public class Rest : IIDEntity, IIconEntity, IFormDescriptor, IFormConditionalFields
    {
        public const string REST_METHOD_GET = "get";
        public const string REST_METHOD_POST = "post";
        public const string REST_METHOD_PUT = "put";
        public const string REST_METHOD_DELETE = "delete";
        public const string REST_METHOD_PATCH = "patch";

        public Rest()
        {
            Id = Guid.NewGuid().ToId();
            Anonymous = true;
        }

        public string Id { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, IsRequired: true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string Name { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, IsRequired: true, FieldType: FieldTypes.Key, ResourceType: typeof(DeploymentAdminResources))]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, IsRequired: true, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-notification-1";


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, 
            ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_Address, FieldType: FieldTypes.Text, IsRequired: true, ResourceType: typeof(DeploymentAdminResources))]
        public string Address { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_Method, EnumType:typeof(RestMethods), IsRequired: true, WaterMark:DeploymentAdminResources.Names.RestNotification_Method_Select, 
            FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<RestMethods> Method { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_Payload, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Payload { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_ContentType, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string ContentType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_UserName, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public bool Anonymous { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_UserName, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string BasicAuthUserName { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_Password, SecureIdFieldName:nameof(BasicAuthPasswordSecretId), FieldType: FieldTypes.Password, ResourceType: typeof(DeploymentAdminResources))]
        public string BasicAuthPassword { get; set; }

        public string BasicAuthPasswordSecretId { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_AdditionalHeaders, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/notification/rest/header/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<RestRequestHeader> Headers { get; set; } = new List<RestRequestHeader>();


        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                 ConditionalFields = new List<string>() {  nameof(BasicAuthPassword), nameof(BasicAuthUserName), nameof(Payload) },
                 Conditionals = new List<FormConditional>()
                 {
                     new FormConditional()
                     {
                         Field = nameof(Anonymous),
                         Value = "false",
                         VisibleFields = new List<string>(){nameof(BasicAuthUserName), nameof(BasicAuthPassword)},
                         RequiredFields = new List<string>() { nameof(BasicAuthUserName), nameof(BasicAuthPassword)}
                     },
                     new FormConditional()
                     {
                         Field = nameof(Method),
                         VisibleFields = new List<string>(){ nameof(Payload) },
                         NotEquals = true,
                         Value = REST_METHOD_GET,
                     }
                 }
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Category),
                nameof(Method),
                nameof(Address),
                nameof(Anonymous),
                nameof(BasicAuthUserName),
                nameof(BasicAuthPassword),
                nameof(Payload),
                nameof(Headers)
            };
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.RestNotificaiton_AdditionalHeader_Title, DeploymentAdminResources.Names.RestNotificaiton_AdditionalHeader_Help,
            DeploymentAdminResources.Names.RestNotificaiton_AdditionalHeader_Help, EntityDescriptionAttribute.EntityTypes.ChildObject, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
            FactoryUrl: "/api/notification/rest/header/factory")]
    public class RestRequestHeader : IFormDescriptor
    {
        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_HeaderName, IsRequired: true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string HeaderName { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RestNotification_HeaderValue, IsRequired: true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string HeaderValue { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(HeaderName),
                nameof(HeaderValue)
            };
        }
    }
}
