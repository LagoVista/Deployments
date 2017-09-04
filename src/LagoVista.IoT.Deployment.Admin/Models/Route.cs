using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceMessaging.Admin.Models;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Route_Title, Resources.DeploymentAdminResources.Names.Route_Help, Resources.DeploymentAdminResources.Names.Route_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class Route : IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IFormDescriptor
    {
        public Route()
        {
            CustomPipelineModuleConfigurations = new List<DevicePipelineModuleConfiguration<CustomPipelineModuleConfiguration>>();
            Id = Guid.NewGuid().ToId();
            MessageDefinitions = new List<EntityHeader<DeviceMessageDefinition>>();
        }

        public string Id { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired:true)]
        public string Name { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_IsDefault, HelpResource: Resources.DeploymentAdminResources.Names.Route_IsDefault_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsDefault { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_Messages, HelpResource: Resources.DeploymentAdminResources.Names.Route_Messages_Help, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<DeviceMessageDefinition>> MessageDefinitions { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.DeviceConfiguration_Sentinel, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.Route_SelectSentinel, ResourceType: typeof(DeploymentAdminResources))]
        public DevicePipelineModuleConfiguration<SentinelConfiguration> Sentinel { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_InputTranslator, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.Route_SelectInputTranslator, ResourceType: typeof(DeploymentAdminResources))]
        public DevicePipelineModuleConfiguration<InputTranslatorConfiguration> InputTranslator { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_Workflow, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.Route_SelectWorkflow, ResourceType: typeof(DeploymentAdminResources))]
        public DevicePipelineModuleConfiguration<LagoVista.IoT.DeviceAdmin.Models.DeviceWorkflow> DeviceWorkflow { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_OutputTranslator, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.Route_SelectOutputTranslator, ResourceType: typeof(DeploymentAdminResources))]
        public DevicePipelineModuleConfiguration<OutputTranslatorConfiguration> OutputTranslator { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_Transmitter, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.Route_SelectTransmitter, ResourceType: typeof(DeploymentAdminResources))]
        public DevicePipelineModuleConfiguration<TransmitterConfiguration> Transmitter { get; set; }


        public List<DevicePipelineModuleConfiguration<CustomPipelineModuleConfiguration>> CustomPipelineModuleConfigurations { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string Notes { get; set; }

        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
               nameof(Route.Name),
               nameof(Route.Key),
               nameof(Route.IsDefault),
               nameof(Route.Sentinel),
               nameof(Route.InputTranslator),
               nameof(Route.DeviceWorkflow),
               nameof(Route.OutputTranslator),
               nameof(Route.Transmitter),
               nameof(Route.Notes)
            };
        }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name
            };
        }
    }
}

