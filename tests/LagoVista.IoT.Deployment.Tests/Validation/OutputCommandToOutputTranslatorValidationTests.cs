using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Validation
{
    [TestClass]
    public class OutputCommandToOutputTranslatorValidationTests : ValidationTestBase
    {
        DeviceMessageDefinition _messageDefinition;
        OutputTranslatorConfiguration _outputTranslatorConfig;
        InputTranslatorConfiguration _inputTranslatorConfig;
        DeviceWorkflow _deviceWorkflow;
        RouteModuleConfig _moduleConfig;

        [TestInitialize]
        public void Init()
        {
            _messageDefinition = GetDeviceMessage();
            _deviceWorkflow = GetWorkflow();
            _moduleConfig = new RouteModuleConfig
            {
                PrimaryOutput = new RouteConnection()
            };
            RefreshMapping();

            _outputTranslatorConfig = new OutputTranslatorConfiguration() { Key = "key1" };
        }

        private void RefreshMapping(string ocKey = null, string fldKey = null)
        {
            var ocm = new OutputCommandMapping()
            {
                OutgoingDeviceMessage = new Core.Models.EntityHeader<DeviceMessageDefinition>() { Id = _messageDefinition.Id, Text = _messageDefinition.Name, Value = _messageDefinition },
                FieldMappings = new List<KeyValuePair<string, string>>()
            };

            ocm.FieldMappings.Add(new KeyValuePair<string, string>(ocKey ?? _deviceWorkflow.OutputCommands[0].Parameters[0].Key,
                fldKey ?? _messageDefinition.Fields[0].Key));
            _moduleConfig.PrimaryOutput.Mappings.Clear();
            _moduleConfig.PrimaryOutput.Mappings.Add(new KeyValuePair<string, object>(_deviceWorkflow.OutputCommands[0].Key, JsonConvert.SerializeObject(ocm)));

        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_Valid()
        {
            var result = new ValidationResult();

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_MissingFieldOnMessage_InValid()
        {
            var result = new ValidationResult();

            var ocm = JsonConvert.DeserializeObject<OutputCommandMapping>(_moduleConfig.PrimaryOutput.Mappings[0].Value.ToString());
            ocm.FieldMappings[0] = new KeyValuePair<string, string>(ocm.FieldMappings[0].Key, "missingobjectvalue");
            _moduleConfig.PrimaryOutput.Mappings[0] =new KeyValuePair<string, object>(_moduleConfig.PrimaryOutput.Mappings[0].Key, JsonConvert.SerializeObject(ocm));

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsInValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_MissingFieldOnOutputCommand_InValid()
        {
            var result = new ValidationResult();

            var ocm = JsonConvert.DeserializeObject<OutputCommandMapping>(_moduleConfig.PrimaryOutput.Mappings[0].Value.ToString());
            ocm.FieldMappings[0] = new KeyValuePair<string, string>("missingkeyvalue", ocm.FieldMappings[0].Value);
            _moduleConfig.PrimaryOutput.Mappings[0] = new KeyValuePair<string, object>(_moduleConfig.PrimaryOutput.Mappings[0].Key, JsonConvert.SerializeObject(ocm));

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsInValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_InvalidMapping_InValid()
        {
            var result = new ValidationResult();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.Integer);
            _deviceWorkflow.OutputCommands[0].Parameters[0].ParameterType = EntityHeader<ParameterTypes>.Create(ParameterTypes.Decimal);

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsInValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_StateSet_Valid()
        {
            var result = new ValidationResult();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _messageDefinition.Fields[0].StateSet = new EntityHeader<StateSet>() { Id = "abc" };
            _deviceWorkflow.OutputCommands[0].Parameters[0].ParameterType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _deviceWorkflow.OutputCommands[0].Parameters[0].StateSet = new EntityHeader<StateSet>() { Id = "abc" };

            RefreshMapping();

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_StateSetMisMatch_InValid()
        {
            var result = new ValidationResult();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _messageDefinition.Fields[0].StateSet = new EntityHeader<StateSet>() { Id = "abc" };
            _deviceWorkflow.OutputCommands[0].Parameters[0].ParameterType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _deviceWorkflow.OutputCommands[0].Parameters[0].StateSet = new EntityHeader<StateSet>() { Id = "abcX" };
            RefreshMapping();

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsInValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_UnitSet_Valid()
        {
            var result = new ValidationResult();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _messageDefinition.Fields[0].UnitSet = new EntityHeader<UnitSet>() { Id = "abc" };
            _deviceWorkflow.OutputCommands[0].Parameters[0].ParameterType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _deviceWorkflow.OutputCommands[0].Parameters[0].UnitSet = new EntityHeader<UnitSet>() { Id = "abc" };

            RefreshMapping();

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);            

            AssertIsValid(result);
        }

        [TestMethod]
        public void OutputCommand_To_OutputTranslator_UnitSetMisMatch_InValid()
        {
            var result = new ValidationResult();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _messageDefinition.Fields[0].UnitSet = new EntityHeader<UnitSet>() { Id = "abc" };
            _deviceWorkflow.OutputCommands[0].Parameters[0].ParameterType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _deviceWorkflow.OutputCommands[0].Parameters[0].UnitSet = new EntityHeader<UnitSet>() { Id = "def" };

            RefreshMapping();

            _moduleConfig.WorkflowToOutputTranslatorValidation(result, _deviceWorkflow, _outputTranslatorConfig);

            AssertIsInValid(result);
        }

    }
}
