using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.IoT.DeviceAdmin.Interfaces;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.Core.Attributes;


namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RouteModuleConfig
    {
        public RouteModuleConfig()
        {
            SecondaryOutputs = new List<EntityHeader>();
            Mappings = new List<KeyValuePair<string, string>>();
            Id = Guid.NewGuid().ToId();
           
            Name = Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned;
        }

        public string Id { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Name, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        public List<KeyValuePair<string, string>> Mappings { get; set; }

        public EntityHeader PrimaryOutput { get; set; }

        public List<EntityHeader> SecondaryOutputs { get; set; }

        public EntityHeader<PipelineModuleType> ModuleType { get; set; }

        public DiagramLocation DiagramLocation { get; set; }

        public EntityHeader<IPipelineModuleConfiguration> Module { get; set; }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if(EntityHeader.IsNullOrEmpty(ModuleType)) result.Errors.Add(Resources.DeploymentErrorCodes.RouteModuleEmptyType.ToErrorMessage());
            if (EntityHeader.IsNullOrEmpty(Module)) result.Errors.Add(Resources.DeploymentErrorCodes.RouteModuleEmptyType.ToErrorMessage());
            return result;
        }
    }
}
