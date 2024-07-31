using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Route_Title, DeploymentAdminResources.Names.Route_Help, DeploymentAdminResources.Names.Route_Description,
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources), FactoryUrl: "/api/deviceconfig/route/factory", Icon: "icon-fo-message-route")]
    public class Route : IKeyedEntity, IIDEntity, INamedEntity, IFormDescriptor, IFormDescriptorCol2, IValidateable
    {
        public Route()
        {
            PipelineModules = new List<RouteModuleConfig>();
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Route_IsDefault, HelpResource: DeploymentAdminResources.Names.Route_IsDefault_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsDefault { get; set; }

        [FKeyProperty(nameof(DeviceMessageDefinition), typeof(DeviceMessageDefinition), "ARRAY_CONTAINS(c.Routes, {MessageDefinition:{Id:@id}}, true)")]
        [FormField(LabelResource: DeploymentAdminResources.Names.Route_Messages, HelpResource: DeploymentAdminResources.Names.Route_Messages_Help, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/devicemessagetypes",
          IsRequired:true, WaterMark:DeploymentAdminResources.Names.Route_Message_Help,  ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeviceMessageDefinition> MessageDefinition { get; set; }

        public List<RouteModuleConfig> PipelineModules { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
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
               nameof(Route.MessageDefinition)
            };
        }

        public List<string> GetFormFieldsCol2()
        {

            return new List<string>() {
               nameof(Route.IsDefault),
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

            sentinel.PrimaryOutput = RouteConnection.Create(inputTranslator.Id, DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(inputTranslator);

            var workflow = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.Workflow),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 350, Y = 225 }
            };

            inputTranslator.PrimaryOutput = RouteConnection.Create(workflow.Id, DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(workflow);

            var outputTranslator = new RouteModuleConfig()
            {
                ModuleType = EntityHeader<PipelineModuleType>.Create(PipelineModuleType.OutputTranslator),
                DiagramLocation = new DeviceAdmin.Models.DiagramLocation() { Page = 1, X = 500, Y = 325 }
            };

            workflow.PrimaryOutput = RouteConnection.Create(outputTranslator.Id, DeploymentAdminResources.RouteModuleConfig_Unassigned);
            route.PipelineModules.Add(outputTranslator);

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
                if (moduleConfig.PrimaryOutput != null)
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

        /// <summary>
        /// The mappings between routes go from the source to the destination,
        /// in most cases the destination is the one that needs the mappings
        /// this method will make sure the destination has the mappings as well
        /// </summary>
        /// <param name="module"></param>
        public void BackPopulateMappings(RouteModuleConfig module)
        {
            var primaryIncomingModule = this.PipelineModules.Where(pm => (pm.PrimaryOutput != null && pm.PrimaryOutput.Id == module.Id)).FirstOrDefault();

            if (primaryIncomingModule != null)
            {
                if (primaryIncomingModule.PrimaryOutput != null)
                {
                    foreach (var mapping in primaryIncomingModule.PrimaryOutput.Mappings)
                    {
                        if (!module.IncomingMappings.Where(mod => mod.Key == mapping.Key).Any())
                        {
                            module.IncomingMappings.Add(mapping);
                        }
                    }
                }
            }

            var secondaryIncomingModule = this.PipelineModules.Where(pm => (pm.SecondaryOutputs != null && pm.SecondaryOutputs.Where(rt => rt.Id == module.Id).Any())).FirstOrDefault();
            if (secondaryIncomingModule?.SecondaryOutputs != null)
            {
                var incomingPath = secondaryIncomingModule.SecondaryOutputs.Where(mod => mod.Id == module.Id).FirstOrDefault();
                if (incomingPath != null)
                {
                    foreach (var mapping in incomingPath.Mappings)
                    {
                        if (!module.IncomingMappings.Where(mod => mod.Key == mapping.Key).Any())
                        {
                            module.IncomingMappings.Add(mapping);
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