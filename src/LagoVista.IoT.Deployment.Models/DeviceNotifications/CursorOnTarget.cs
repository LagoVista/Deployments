using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using RingCentral;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models.DeviceNotifications
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.CotNotification_Title, DeploymentAdminResources.Names.CotNotification_Help,
           DeploymentAdminResources.Names.MqttNotifications_Help, EntityDescriptionAttribute.EntityTypes.ChildObject, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
           FactoryUrl: "/api/notification/cot/factory")]
    public class CursorOnTarget : IIDEntity, IIconEntity, IFormDescriptor, IFormConditionalFields
    {
        public CursorOnTarget()
        {
            StaleSeconds = 5 * 60;
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string Name { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, ResourceType: typeof(DeploymentAdminResources))]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-notification-1";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotifiication_Type, HelpResource: DeploymentAdminResources.Names.CotNotification_Type_Help, IsRequired:true, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string NotificationType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_StaleSeconds, HelpResource:DeploymentAdminResources.Names.CotNotification_StaleSeconds_Help, FieldType: FieldTypes.Integer, ResourceType: typeof(DeploymentAdminResources))]
        public int StaleSeconds { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_DataPackageFile, HelpResource: DeploymentAdminResources.Names.CotNotification_DataPackageFile_Help, FieldType: FieldTypes.FileUpload, PrivateFileUpload: true, IsRequired:true, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader DataPackageFile { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_IgnoreCertificateErrors, HelpResource: DeploymentAdminResources.Names.CotNotification_IgnoreCertificateErrors_Help,
            FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IgnoreCertificateErrors { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_UseCustomRoot, HelpResource: DeploymentAdminResources.Names.CotNotification_UseCustomRoot_Help,
            FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool UseCustomCertificate { get; set; }

        
        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_CustomCertExpires, FieldType: FieldTypes.Date, ResourceType: typeof(DeploymentAdminResources))]
        public string CustomCertExpires { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_PublicRootCert, HelpResource: DeploymentAdminResources.Names.CotNotification_PublicRootCert_Help, 
            FieldType: FieldTypes.FileUpload, PrivateFileUpload: true, IsRequired: true, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader CustomRootCert { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_IncludeDeviceLocation, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IncludeDeviceLocation { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_IncludeLocationPolygon, HelpResource:DeploymentAdminResources.Names.CotNotification_IncludeLocationPolygon_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IncludeLocationPolygon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_FillColor, FieldType: FieldTypes.Color, ResourceType: typeof(DeploymentAdminResources))]
        public string FillColor { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_Remarks, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Remarks { get; set; }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                  ConditionalFields = new List<string>() { nameof(FillColor), nameof(CustomRootCert), nameof(CustomCertExpires)},
                  Conditionals = new List<FormConditional>()
                  {
                       new FormConditional()
                       {
                            Field = nameof(IncludeLocationPolygon),
                            Value = "true",
                            VisibleFields = new List<string>() { nameof(FillColor)}
                       },
                       new FormConditional()
                       {
                           Field = nameof(UseCustomCertificate),
                           Value = "true",
                           VisibleFields = new List<string>() {nameof(CustomRootCert), nameof(CustomCertExpires) },
                           RequiredFields = new List<string>() {nameof(CustomRootCert), nameof(CustomCertExpires) }
                       }
                  },
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
                nameof(NotificationType),
                nameof(StaleSeconds),
                nameof(DataPackageFile),
                nameof(IgnoreCertificateErrors),
                nameof(UseCustomCertificate),
                nameof(CustomRootCert),
                nameof(CustomCertExpires),
                nameof(Remarks),
                nameof(IncludeDeviceLocation),
                nameof(IncludeLocationPolygon),
                nameof(FillColor),
            };
        }
    }
}
