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

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_IncludeLocationPolygon, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IncludeLocationPoloygon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_FillColor, FieldType: FieldTypes.Color, ResourceType: typeof(DeploymentAdminResources))]
        public string FillColor { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.CotNotification_Remarks, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Remarks { get; set; }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                 ConditionalFields = new List<string>() { nameof(FillColor)},
                  Conditionals = new List<FormConditional>()
                  {
                       new FormConditional()
                       {
                            Field = nameof(IncludeLocationPoloygon),
                            Value = "true",
                            VisibleFields = new List<string>() { nameof(FillColor)}
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
                nameof(Remarks),
                nameof(IncludeLocationPoloygon),
                nameof(FillColor),
            };
        }


    }
}
