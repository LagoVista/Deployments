using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Route_Title, Resources.DeploymentAdminResources.Names.Route_Help, Resources.DeploymentAdminResources.Names.Route_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class Route : IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IFormDescriptor
    {
        public Route()
        {
            PipelineModules = new List<DevicePipelineModuleConfiguration>();
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_IsDefault, HelpResource: Resources.DeploymentAdminResources.Names.Route_IsDefault_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsDefault { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Route_Messages, HelpResource: Resources.DeploymentAdminResources.Names.Route_Messages_Help, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeviceMessageDefinition> MessageDefinition { get; set; }

        public List<DevicePipelineModuleConfiguration> PipelineModules { get; set; }


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
                nameof(Route.MessageDefinition),
               nameof(Route.IsDefault),
               nameof(Route.Notes)
            };
        }

        public static Route Create()
        {
            var route = new Route();

            route.PipelineModules.Add(new DevicePipelineModuleConfiguration()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Sentinel),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 25 }                
            });

            route.PipelineModules.Add(new DevicePipelineModuleConfiguration()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.InputTranslator),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 150 }
            });

            route.PipelineModules.Add(new DevicePipelineModuleConfiguration()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Workflow),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 275 }
            });

            route.PipelineModules.Add(new DevicePipelineModuleConfiguration()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.OutputTranslator),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 425 }
            });

            route.PipelineModules.Add(new DevicePipelineModuleConfiguration()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Transmitter),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 575 }
            });

            return route;
        }

        [CustomValidator]
        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (EntityHeader.IsNullOrEmpty(MessageDefinition))
            {
                result.Errors.Add(DeploymentErrorCodes.NoMessageOnRoute.ToErrorMessage());
            }

            if (!PipelineModules.Any())
            {
                result.Errors.Add(DeploymentErrorCodes.EmptyRoute.ToErrorMessage());
            }

            return result;
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

