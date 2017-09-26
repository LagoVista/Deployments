using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceMessaging.Admin.Models;

namespace LagoVista.IoT.Deployment.Tests.Validation
{
    public class ValidationTestBase
    {
        protected void WriteResults(ValidationResult result)
        {
            Console.WriteLine("Errors (Expected if test Passed");
            Console.WriteLine("==================================================");

            foreach (var err in result.Errors)
            {
                Console.WriteLine(err.Message);
            }

            Console.WriteLine("");
            Console.WriteLine("Warnings (Expected if test Passed");
            Console.WriteLine("==================================================");

            if (result.Warnings.Count > 0)
            {
                foreach (var err in result.Warnings)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        protected void AssertIsValid(ValidationResult result)
        {
            WriteResults(result);
            Assert.IsTrue(result.Successful);
        }


        protected void AssertIsInValid(ValidationResult result, int errorCount = 1, int warningCount = 0)
        {
            WriteResults(result);
            Assert.IsFalse(result.Successful);
            //TODO: Right now we are just checking for valid/invalid, to do thig 100% right we should make sure the error message is the one expected for the condition, our error messages needs to be put into resources and use formatting for parameters to ensure this works right, right now we just assume that there is one error...short cut, probalby burn us, but need to ship!
            Assert.AreEqual(errorCount, result.Errors.Count);
            Assert.AreEqual(warningCount, result.Warnings.Count);
        }

        private void SetAuditParams(IAuditableEntity entity)
        {
            entity.CreatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "dontcare");
            entity.LastUpdatedBy = entity.CreatedBy;
            entity.CreationDate = DateTime.Now.ToJSONString();
            entity.LastUpdatedDate = entity.CreationDate;
        }

        protected Parameter GetParameter(string name, string key, ParameterTypes parameterType = ParameterTypes.String, string parameterName = "String")
        {
            var parameter = new Parameter();
            parameter.Name = name;
            parameter.Key = key;
            parameter.ParameterType = EntityHeader<ParameterTypes>.Create(parameterType);
            return parameter;
        }

        protected DeviceMessageDefinitionField GetDeviceMessageField(string name, string key, ParameterTypes parameterType = ParameterTypes.String)
        {
            var field = new DeviceMessageDefinitionField();
            field.Name = name;
            field.Key = key;
            field.StorageType = EntityHeader<ParameterTypes>.Create(parameterType);
            return field;
        }

        public DeviceMessageDefinition GetDeviceMessage(string name = "msg1", string key = "msg1")
        {
            var msg = new DeviceMessageDefinition();
            msg.Name = name;
            msg.Key = key;
            msg.Fields.Add(GetDeviceMessageField(name, key));

            return msg;
        }

        protected OutputCommand GetOutputCommand(string name = "oc1", string key = "oc1")
        {
            var outputCommand = new OutputCommand();
            outputCommand.Name = name;
            outputCommand.Key = key;
            SetAuditParams(outputCommand);
            outputCommand.Parameters.Add(GetParameter("param1", "param1"));
            return outputCommand;
        }

        protected WorkflowInput GetInput(string name = "in1", string key = "in1", ParameterTypes parameterType = ParameterTypes.String)
        {
            var input = new WorkflowInput();
            input.Name = name;
            input.Key = key;
            SetAuditParams(input);
            input.InputType = EntityHeader<ParameterTypes>.Create(parameterType);
            return input;
        }

        protected DeviceWorkflow GetWorkflow(string name = "wf1", string key = "wf1")
        {
            var workflow = new DeviceWorkflow();
            workflow.Name = name;
            workflow.Key = key;
            SetAuditParams(workflow);
            workflow.OutputCommands.Add(GetOutputCommand());
            workflow.Inputs.Add(GetInput());

            return workflow;
        }

        protected EntityHeader<UnitSet> CreateUnitSet()
        {
            var unitSet = new UnitSet()
            {
                Id = "unitset1",
                Key = "abc123",
                Name = "MyUnitSet"
            };

            return new EntityHeader<UnitSet>() { Id = unitSet.Id, Text = unitSet.Name, Value = unitSet };
        }

        protected EntityHeader<StateSet> CreateStateSet()
        {
            var stateSet = new StateSet()
            {
                Id = "stateset1",
                Key = "sts123",
                Name = "MyStateSet"
            };

            return new EntityHeader<StateSet>() { Id = stateSet.Id, Text = stateSet.Name, Value = stateSet };
        }

    }
}
