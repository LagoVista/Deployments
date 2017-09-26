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
using LagoVista.IoT.DeviceAdmin.Models;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Route_Title, Resources.DeploymentAdminResources.Names.Route_Help, Resources.DeploymentAdminResources.Names.Route_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class Route : IKeyedEntity, IIDEntity, INamedEntity, IAuditableEntity, IFormDescriptor, IValidateable
    {
        public Route()
        {
            PipelineModules = new List<RouteModuleConfig>();
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

        public List<RouteModuleConfig> PipelineModules { get; set; }


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

            var sentinel = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Sentinel),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 50, Y = 25 }
            };

            route.PipelineModules.Add(sentinel);

            var inputTranslator = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.InputTranslator),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 200, Y = 125 }
            };

            sentinel.PrimaryOutput = EntityHeader.Create(inputTranslator.Id, Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(inputTranslator);

            var workflow = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Workflow),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 350, Y = 225 }
            };

            inputTranslator.PrimaryOutput = EntityHeader.Create(workflow.Id, Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(workflow);

            var outputTranslator = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.OutputTranslator),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 500, Y = 325 }
            };

            workflow.PrimaryOutput = EntityHeader.Create(outputTranslator.Id, Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(outputTranslator);


            var transmitter = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Transmitter),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 650, Y = 425 }
            };

            outputTranslator.PrimaryOutput = EntityHeader.Create(transmitter.Id, Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(transmitter);

            return route;
        }


        /// <summary>
        /// Perform a deep validation,normal validation only ensures that the correct properties are set but doesn't load
        /// and objects that are loaded via relationships.  This assumes that all the modules and their dependencies have been loaded.
        /// </summary>
        /// <param name="result"></param>
        public void DeepValidation(ValidationResult result)
        {
            // Make sure basic validation checksout.
            foreach (var moduleConfig in PipelineModules)
            {
                Validate(result);
            }

            if (!result.Successful)
            {
                return;
            }

            if (MessageDefinition.Value == null)
            {
                result.Errors.Add(Resources.DeploymentErrorCodes.RouteDestinationModuleNull.ToErrorMessage());
            }

            foreach (var moduleConfig in PipelineModules)
            {
                if (!EntityHeader.IsNullOrEmpty(moduleConfig.PrimaryOutput))
                {
                    var destModuleConfig = PipelineModules.Where(mod => mod.Id == moduleConfig.PrimaryOutput.Id).FirstOrDefault();
                    if (destModuleConfig == null)
                    {
                        result.Errors.Add(Resources.DeploymentErrorCodes.RouteCouldNotFindLinkedModule.ToErrorMessage($"SourceModule={moduleConfig.Name}"));
                    }
                    else
                    {
                        if (moduleConfig.Module.Value == null)
                        {
                            result.Errors.Add(Resources.DeploymentErrorCodes.RouteSourceModuleNull.ToErrorMessage($"SourceModule={moduleConfig.Name}"));
                        }
                        else if (destModuleConfig.Module.Value == null)
                        {
                            result.Errors.Add(Resources.DeploymentErrorCodes.RouteDestinationModuleNull.ToErrorMessage($"SourceModule={moduleConfig.Name}"));
                        }
                        else
                        {
                            if (moduleConfig.ModuleType.Value == PipelineModuleType.InputTranslator &&
                                destModuleConfig.ModuleType.Value == PipelineModuleType.Workflow)
                            {
                                moduleConfig.InputTranslatorToWorkflowValidation(result, MessageDefinition.Value, destModuleConfig.Module.Value as DeviceWorkflow);
                            }
                            else if (moduleConfig.ModuleType.Value == PipelineModuleType.InputTranslator &&
                               destModuleConfig.ModuleType.Value == PipelineModuleType.Workflow)
                            {
                                moduleConfig.WorkflowToOutputTranslatorValidation(result, moduleConfig.Module.Value as DeviceWorkflow, destModuleConfig.Module.Value as OutputTranslatorConfiguration);
                            }
                        }
                    }
                }
            }
        }

        [CustomValidator]
        public void Validate(ValidationResult result)
        {
            if (EntityHeader.IsNullOrEmpty(MessageDefinition))
            {
                result.Errors.Add(DeploymentErrorCodes.NoMessageOnRoute.ToErrorMessage());
            }

            if (!PipelineModules.Any())
            {
                result.Errors.Add(DeploymentErrorCodes.EmptyRoute.ToErrorMessage());
            }

            foreach (var module in PipelineModules)
            {
                result.Concat(module.Validate());
            }
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