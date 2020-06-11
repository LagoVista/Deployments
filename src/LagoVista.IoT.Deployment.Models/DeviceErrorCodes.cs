using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceErrorCode_Title, DeploymentAdminResources.Names.DeviceErrorCode_Help,
        DeploymentAdminResources.Names.DeviceErrorCode_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeviceErrorCode
    {
        public DeviceErrorCode()
        {
            Id = Guid.NewGuid().ToString();
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)] 
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_ErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool TriggerOnEachOccurrence { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description,  FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Description { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Help, WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader ServiceTicketTemplate { get; set; }
    }
}
