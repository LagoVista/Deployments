using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models.DeviceNotifications
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.MqttNotification_Title, DeploymentAdminResources.Names.MqttNotifications_Help,
            DeploymentAdminResources.Names.MqttNotifications_Help, EntityDescriptionAttribute.EntityTypes.ChildObject, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
            FactoryUrl: "/api/notification/mqtt/factory")]
    public class Mqtt : IIDEntity, IIconEntity, IFormDescriptor, IFormConditionalFields
    {
        public Mqtt()
        {
            Id = Guid.NewGuid().ToId();
            Port = 1883;
        }

        public string Id { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, IsRequired: true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, IsRequired:true,  FieldType: FieldTypes.Key, ResourceType: typeof(DeploymentAdminResources))]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-notification-1";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select,
            ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Address, IsRequired:true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string Address { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Port, IsRequired:true, FieldType: FieldTypes.Integer, ResourceType: typeof(DeploymentAdminResources))]
        public int Port { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Topic, FieldType: FieldTypes.Text, HelpResource:DeploymentAdminResources.Names.MqttNotification_Topic_Help, ResourceType: typeof(DeploymentAdminResources))]
        public string Topic { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Payload, FieldType: FieldTypes.MultiLineText, HelpResource:DeploymentAdminResources.Names.MqttNotification_Payload_Help, ResourceType: typeof(DeploymentAdminResources))]
        public string Payload { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Anonymous, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool Anonymous { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_UserName, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string UserName { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Password, SecureIdFieldName: nameof(PasswordSecretId), FieldType: FieldTypes.Password, ResourceType: typeof(DeploymentAdminResources))]
        public string Password { get; set; }
        public string PasswordSecretId { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_ClientId, HelpResource: DeploymentAdminResources.Names.MqttNotification_ClientId_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string ClientId { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_SecureConnection, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SecureConnection { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_Certificate, SecureIdFieldName: nameof(CertificateSecureId), FieldType: FieldTypes.SecureCertificate, ResourceType: typeof(DeploymentAdminResources))]
        public string Certificate { get; set; }

        public string CertificateSecureId { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.MqttNotification_CertificatePassword, SecureIdFieldName: nameof(CertificatePasswordSecureId), FieldType: FieldTypes.Password, ResourceType: typeof(DeploymentAdminResources))]
        public string CertificatePassword { get; set; }

        public string CertificatePasswordSecureId { get; set; }


        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(UserName), nameof(Password), nameof(ClientId), nameof(Certificate), nameof(CertificatePassword) },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                        Field = nameof(SecureConnection),
                        Value = "true",
                        VisibleFields = new List<string>() {nameof(Certificate), nameof(CertificatePassword)},
                    },
                    new FormConditional()
                    {
                         Field = nameof(Anonymous),
                         Value = "false",
                         VisibleFields = new List<string>() {nameof(UserName), nameof(Password)},
                         RequiredFields = new List<string>() {nameof(UserName), nameof(Password)}
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
                nameof(Address),
                nameof(Port),
                nameof(ClientId),
                nameof(Anonymous),
                nameof(UserName),
                nameof(Password),
                nameof(SecureConnection),
                nameof(Certificate),
                nameof(CertificatePassword),
                nameof(Topic),
                nameof(Payload),
            };
        }
    }
}
