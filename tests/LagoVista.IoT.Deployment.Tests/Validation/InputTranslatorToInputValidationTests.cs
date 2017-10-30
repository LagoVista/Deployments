using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Validation
{
    [TestClass]
    public class InputTranslatorToInputValidationTests : ValidationTestBase
    {
        DeviceMessageDefinition _messageDefinition;
        DeviceWorkflow _deviceWorkflow;


        [TestInitialize]
        public void Init()
        {
            _messageDefinition = GetDeviceMessage();
            _deviceWorkflow = GetWorkflow();
            
        }

        private RouteModuleConfig GetModuleConfig()
        {
            var moduleConfig = new RouteModuleConfig
            {
                PrimaryOutput = new RouteConnection()
            };
            moduleConfig.PrimaryOutput.Mappings.Add(new KeyValuePair<string, object>(_messageDefinition.Fields[0].Key, _deviceWorkflow.Inputs[0].Key));

            return moduleConfig;
        }

        [TestMethod]
        public void InputTranslator_To_Input_Valid()
        {
            var result = new ValidationResult();
            var moduleConfig = GetModuleConfig();
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsValid(result);
        }

        [TestMethod]
        public void InputTranslator_To_Input_MissingField_InValid()
        {
            var result = new ValidationResult();
            var moduleConfig = GetModuleConfig();
            moduleConfig.PrimaryOutput.Mappings[0] = new KeyValuePair<string, object>(_messageDefinition.Fields[0].Key + "XYZ", _deviceWorkflow.Inputs[0].Key);

            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsInValid(result);
        }

        [TestMethod]
        public void InputTranslator_To_Input_MissingInput_InValid()
        {
            var result = new ValidationResult();
            var moduleConfig = GetModuleConfig();
            moduleConfig.PrimaryOutput.Mappings[0] = new KeyValuePair<string, object>(_messageDefinition.Fields[0].Key, _deviceWorkflow.Inputs[0].Key + "KYZ");
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsInValid(result);
        }

        [TestMethod]
        public void InputTranslator_To_Input_TypeMisMatch_InValid()
        {
            var result = new ValidationResult();
            var moduleConfig = GetModuleConfig();

            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.Integer);
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsInValid(result);
        }


        [TestMethod]
        public void InputTranslator_To_Input_StateSet_Valid()
        {
            var result = new ValidationResult();

            _deviceWorkflow.Inputs[0].InputType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _deviceWorkflow.Inputs[0].StateSet = CreateStateSet();
            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _messageDefinition.Fields[0].StateSet = CreateStateSet();

            var moduleConfig = GetModuleConfig();
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsValid(result);
        }


        [TestMethod]
        public void InputTranslator_To_Input_UnitSet_Valid()
        {
            var result = new ValidationResult();

            _deviceWorkflow.Inputs[0].InputType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _deviceWorkflow.Inputs[0].UnitSet = CreateUnitSet();
            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _messageDefinition.Fields[0].UnitSet = CreateUnitSet();

            var moduleConfig = GetModuleConfig();
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsValid(result);
        }


        [TestMethod]
        public void InputTranslator_To_Input_StateSetMisMatch_InValid()
        {
            var result = new ValidationResult();

            _deviceWorkflow.Inputs[0].InputType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _deviceWorkflow.Inputs[0].StateSet = CreateStateSet();
            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.State);
            _messageDefinition.Fields[0].StateSet = CreateStateSet();
            _messageDefinition.Fields[0].StateSet.Id += "abc123";

            var moduleConfig = GetModuleConfig();
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsInValid(result);
        }

        [TestMethod]
        public void InputTranslator_To_Input_UnitSetMisMatch_InValid()
        {
            var result = new ValidationResult();

            _deviceWorkflow.Inputs[0].InputType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _deviceWorkflow.Inputs[0].UnitSet = CreateUnitSet();
            _messageDefinition.Fields[0].StorageType = EntityHeader<ParameterTypes>.Create(ParameterTypes.ValueWithUnit);
            _messageDefinition.Fields[0].UnitSet = CreateUnitSet();
            _messageDefinition.Fields[0].UnitSet.Id += "abc123";

            var moduleConfig = GetModuleConfig();
            moduleConfig.InputTranslatorToWorkflowValidation(result, _messageDefinition, _deviceWorkflow);
            AssertIsInValid(result);
        }

    }
}
