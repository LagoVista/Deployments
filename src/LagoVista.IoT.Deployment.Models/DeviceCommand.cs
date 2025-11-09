// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 657d4a0083f0f671190f5f6f87100b6fb26b01425da0a3e5c4bb18cb416111e9
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.ObjectModel;
using LagoVista.Core.Interfaces;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeviceAdminDomain.DeviceAdmin, DeploymentAdminResources.Names.DeviceCommand_TItle, DeploymentAdminResources.Names.DeviceCommand_Description, DeploymentAdminResources.Names.DeviceCommand_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel,
        typeof(DeviceLibraryResources), FactoryUrl: "/api/deviceconfig/command/factory", Icon: "icon-fo-internet-3")]
    public class DeviceCommand : IFormDescriptor, IFormDescriptorCol2
    {
        public DeviceCommand()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, HelpResource: LagoVistaCommonStrings.Names.Common_Key_Help, FieldType: FieldTypes.Key,
            RegExValidationMessageResource: LagoVistaCommonStrings.Names.Common_Key_Validation, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceConfiguration_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-internet-3";

        [FormField(LabelResource: DeviceLibraryResources.Names.InputCommand_Parameters, HelpResource: DeviceLibraryResources.Names.Parameter_Help,
        FactoryUrl: "/api/deviceconfig/command/factory", FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeviceLibraryResources))]
        public ObservableCollection<Parameter> Parameters { get; set; } = new ObservableCollection<Parameter>();

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Description { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Description),
            };
        }

        public List<string> GetFormFieldsCol2()
        {
            return new List<string>()
            {
                nameof(Parameters),
            };
        }
    }
}
