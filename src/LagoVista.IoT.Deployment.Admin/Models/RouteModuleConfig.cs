using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.IoT.DeviceAdmin.Interfaces;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RouteModuleConfig
    {
        public RouteModuleConfig()
        {
            SecondaryOutputs = new List<EntityHeader>();
            Mappings = new List<KeyValuePair<string, object>>();
            Id = Guid.NewGuid().ToId();

            Name = Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned;
        }

        public string Id { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Name, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        public List<KeyValuePair<string, object>> Mappings { get; set; }

        public EntityHeader PrimaryOutput { get; set; }

        public List<EntityHeader> SecondaryOutputs { get; set; }

        public EntityHeader<PipelineModuleType> ModuleType { get; set; }

        public DiagramLocation DiagramLocation { get; set; }

        public EntityHeader<IPipelineModuleConfiguration> Module { get; set; }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();
            if (EntityHeader.IsNullOrEmpty(ModuleType)) result.Errors.Add(Resources.DeploymentErrorCodes.RouteModuleEmptyType.ToErrorMessage());
            if (EntityHeader.IsNullOrEmpty(Module)) result.Errors.Add(Resources.DeploymentErrorCodes.RouteModuleEmptyType.ToErrorMessage());
            return result;
        }

        public void InputTranslatorToWorkflowValidation(ValidationResult result, DeviceMessageDefinition message, DeviceWorkflow workflow)
        {
            foreach (var mapping in Mappings)
            {
                var mapResult = new ValidationResult();

                var fieldKey = mapping.Key;
                if (mapping.Value == null)
                {
                    mapResult.AddUserError($"Missing Input Key for mapping on work flow {workflow.Name} for input mapping.");
                }
                else
                {
                    var inputKey = mapping.Value.ToString();
                    if(String.IsNullOrEmpty(inputKey))
                    {
                        mapResult.AddUserError($"Missing Input Key for mapping on work flow {workflow.Name} for input mapping.");
                    }

                    if(String.IsNullOrEmpty(fieldKey))
                    {
                        mapResult.AddUserError($"Missing Message Field Key for mapping on work flow {workflow.Name} for input mapping.");
                    }

                    if(mapResult.Successful)
                    {
                        var input = workflow.Inputs.Where(inp => inp.Key == inputKey).FirstOrDefault();
                        var field = message.Fields.Where(fld => fld.Key == fieldKey).FirstOrDefault();

                        if(input == null)
                        {
                            mapResult.AddUserError($"Could not find input {inputKey} on workflow {workflow.Name}");
                        }

                        if(field == null)
                        {
                            mapResult.AddUserError($"Could not find field {fieldKey} on message {message.Name} on workflow {workflow.Name} for input mapping.");
                        }

                        if(field != null && input != null)
                        {
                            if(EntityHeader.IsNullOrEmpty(field.StorageType))
                            {
                                mapResult.AddUserError($"On field {field.Name} for message {message.Name} storage type is empty.");
                            }
                            else if(EntityHeader.IsNullOrEmpty(input.InputType))
                            {
                                mapResult.AddUserError($"On input {field.Name} for workflow {workflow.Name} storage type is empty.");
                            }
                            else 
                            {
                                if(field.StorageType.Id != input.InputType.Id)
                                {
                                    mapResult.AddUserError($"On Mapping for field {field.Name} for message {message.Name} and input {input.Name} type mismatch, {field.StorageType.Text} can not be mapped to {input.InputType.Text}.");
                                }
                                else if(field.StorageType.Value == ParameterTypes.State)
                                {
                                    if (EntityHeader.IsNullOrEmpty(field.StateSet))
                                    {
                                        result.AddUserError($"State Set Not provided on Message {message.Name}, Field {field.Name}.");                                        
                                    }
                                    else if (EntityHeader.IsNullOrEmpty(input.StateSet))
                                    {
                                        result.AddUserError($"State Set Not provided on Input {input.Name}, for work flow {workflow.Name}");
                                    }
                                    else if (field.StateSet.Id != input.StateSet.Id)
                                    {
                                        result.AddUserError($"State Set are different on mapping between Message Field {message.Name}/{field.Name} and input {input.Name} on workflow {workflow.Name} (Field={field.StateSet.Text}, Input={input.StateSet.Text}).");
                                    }
                                }
                                else if(field.StorageType.Value == ParameterTypes.ValueWithUnit)
                                {
                                    if (EntityHeader.IsNullOrEmpty(field.UnitSet))
                                    {
                                        result.AddUserError($"Unit Set Not provided on Message {message.Name}, Field {field.Name}.");
                                    }
                                    else if (EntityHeader.IsNullOrEmpty(input.UnitSet))
                                    {
                                        result.AddUserError($"Unit Set Not provided on Input {input.Name}, for work flow {workflow.Name}");
                                    }
                                    else if (field.UnitSet.Id != input.UnitSet.Id)
                                    {
                                        result.AddUserError($"Units Set are different on mapping between message {message.Name}/{field.Name} and input {input.Name} on workflow {workflow.Name} (Field={field.UnitSet.Text}, Input={input.UnitSet.Text}).");
                                    }
                                }
                            }
                    
                        }

                    }

                    result.Concat(mapResult);                    
                }
            }
        }

        public void WorkflowToOutputTranslatorValidation(ValidationResult result, DeviceWorkflow workflow, OutputTranslatorConfiguration outputTranslator)
        {
            foreach (var mapping in Mappings)
            {
                if (mapping.Value != null)
                {
                    var outputCommandMapping = JsonConvert.DeserializeObject<OutputCommandMapping>(mapping.Value.ToString());
                    if (EntityHeader.IsNullOrEmpty(outputCommandMapping.OutgoingDeviceMessage) || outputCommandMapping.FieldMappings == null)
                    {
                        result.Errors.Add(Resources.DeploymentErrorCodes.InvalidInputCommandMapping.ToErrorMessage($"Workflow={workflow.Key};OutputTranslator={outputTranslator.Key}"));
                    }
                    else
                    {
                        var outputCommand = workflow.OutputCommands.Where(cmd => cmd.Key == mapping.Key).FirstOrDefault();
                        if (outputCommand == null)
                        {
                            result.AddUserError($"Could not find output command for key {mapping.Key}, for work flow {workflow.Name}");
                        }
                        else
                        {
                            var msg = outputCommandMapping.OutgoingDeviceMessage.Value;

                            foreach (var fieldMapping in outputCommandMapping.FieldMappings)
                            {
                                var outputParameter = outputCommand.Parameters.Where(param => param.Key == fieldMapping.Key).FirstOrDefault();
                                var field = msg.Fields.Where(fld => fld.Key == fieldMapping.Value).FirstOrDefault();

                                if (outputParameter == null)
                                {
                                    result.AddUserError($"Could not find output parameter for key {fieldMapping.Key} on output {outputCommand.Name}, for work flow {workflow.Name}.");
                                }
                                else if (field == null)
                                {
                                    result.AddUserError($"Could not find field on message {msg.Name} with key {fieldMapping.Value} for output command {outputCommand.Name}, for work flow {workflow.Name}.");
                                }
                                else if (EntityHeader.IsNullOrEmpty(outputParameter.ParameterType))
                                {
                                    result.AddUserError($"Parameter type not found on output parameter {outputParameter.Name} for output command {outputCommand.Name}, for work flow {workflow.Name}.");
                                }
                                else if (EntityHeader.IsNullOrEmpty(field.StorageType))
                                {
                                    result.AddUserError($"Storage type not found on message {msg.Name} for field {field.Name} with command {outputCommand.Name}, for work flow {workflow.Name}.");
                                }
                                else if (outputParameter.ParameterType.Id != field.StorageType.Id)
                                {
                                    result.AddUserError($"Mis-match storage type not for output command {outputCommand.Name}, {outputParameter.Name}/{outputParameter.ParameterType.Text} to message {msg.Name} for field {field.Name}/{field.StorageType.Text} with command {outputParameter.Name}, for work flow {workflow.Name}.");
                                }
                                else if (outputParameter.ParameterType.Value == ParameterTypes.State)
                                {
                                    if (EntityHeader.IsNullOrEmpty(outputParameter.StateSet))                                       
                                    {
                                        result.AddUserError($"State Set Not provided on Output Command {outputCommand.Name}, Output Parameter {outputParameter.Name}"  );
                                    }
                                    else if(EntityHeader.IsNullOrEmpty(field.StateSet))
                                    {
                                        result.AddUserError($"State Set Not provided on Message {msg.Name}, Field {field.Name}.");
                                    }
                                    else if (outputParameter.StateSet.Id != field.StateSet.Id)
                                    {
                                        result.AddUserError($"State Set are different on mapping between Output Command {outputCommand.Name}/{outputParameter.Name} and Message {msg.Name}/{field.Name} on workflow {workflow.Name}.");
                                    }
                                }
                                else if (outputParameter.ParameterType.Value == ParameterTypes.ValueWithUnit)
                                {
                                    if (EntityHeader.IsNullOrEmpty(outputParameter.UnitSet))
                                    {
                                        result.AddUserError($"Unit Set Not provided on Output Command {outputCommand.Name}, Output Parameter {outputParameter.Name}.");
                                    }
                                    else if(EntityHeader.IsNullOrEmpty(field.UnitSet))
                                    {
                                        result.AddUserError($"Unit Set Not provided on Message {msg.Name}, Field {field.Name}.");
                                    }
                                    else if (outputParameter.UnitSet.Id != field.UnitSet.Id)
                                    {
                                        result.AddUserError($"Units Set are different on mapping between Output Command {outputCommand.Name}/{outputParameter.Name} and Message {msg.Name}/{field.Name} on workflow {workflow.Name}.");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    result.Errors.Add(Resources.DeploymentErrorCodes.InvalidInputCommandMapping.ToErrorMessage($"Workflow={workflow.Key};OutputTranslator={outputTranslator.Key}"));
                }
            }
        }
    }

    /* Will be stuffed into the Values of the Mappings list, will deserialize and validate when it's used */
    public class OutputCommandMapping
    {

        public EntityHeader<DeviceMessageDefinition> OutgoingDeviceMessage { get; set; }

        public List<KeyValuePair<string, string>> FieldMappings { get; set; }
    }
}
