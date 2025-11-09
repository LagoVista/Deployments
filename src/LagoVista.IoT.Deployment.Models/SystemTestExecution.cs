// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f6a7f82a0869b54ae49286764aa51fb1f3c1c3069ebe5bc90d0b670c8e6d515c
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public enum TestExecutionStates
    {
        [EnumLabel(SystemTestExecution.SystemTestExecution_State_New, DeploymentAdminResources.Names.SystemTestExecution_State_New, typeof(DeploymentAdminResources))]
        New,
        [EnumLabel(SystemTestExecution.SystemTestExecution_State_InProcess, DeploymentAdminResources.Names.SystemTestExecution_State_InProcess, typeof(DeploymentAdminResources))]
        InProcess,
        [EnumLabel(SystemTestExecution.SystemTestExecution_State_Passed, DeploymentAdminResources.Names.SystemTestExecution_State_Passed, typeof(DeploymentAdminResources))]
        Passed,
        [EnumLabel(SystemTestExecution.SystemTestExecution_State_Failed, DeploymentAdminResources.Names.SystemTestExecution_State_Failed, typeof(DeploymentAdminResources))]
        Failed,
        [EnumLabel(SystemTestExecution.SystemTestExecution_State_Aborted, DeploymentAdminResources.Names.SystemTestExecution_State_Aborted, typeof(DeploymentAdminResources))]
        Aborted,
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.SystemTestExecutions_Title, DeploymentAdminResources.Names.SystemTestExecutions_Description,
    DeploymentAdminResources.Names.SystemTestStep_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-laptop-protection",
        GetListUrl: "/api/systemtest/executions", SaveUrl: "/api/systemtest/execution", GetUrl: "/api/systemtest/execution/{id}", DeleteUrl: "/api/systemtest/execution/{id}", FactoryUrl: "/api/systemtest/execution/factory")]
    public class SystemTestExecution : EntityBase, ISummaryFactory, IIconEntity, IDescriptionEntity, IValidateable
    {

        public const string SystemTestExecution_State_New = "new";
        public const string SystemTestExecution_State_InProcess = "inprocess";
        public const string SystemTestExecution_State_Passed = "passed";
        public const string SystemTestExecution_State_Failed = "failed";
        public const string SystemTestExecution_State_Aborted = "aborted";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader SystemTest { get; set; }
        
        
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-laptop-protection";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Description { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader<TestExecutionStates> Status { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestExecution_StartTimeStamp, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string StartTimeStamp { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestExecution_EndTimeStamp, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string EndTimeStamp { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestExecution_StartedBy, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader StartedBy { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestExecution_CompletedBy, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader CompletedBy { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestExecution_StartedBy, FieldType: FieldTypes.ReadonlyLabel, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader AbortedBy { get; set; }


        public EntityHeader Customer { get; set; }


        public SystemTestExecutionSummary CreateSummary()
        {
            return new SystemTestExecutionSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                Description = Description,
                Icon = Icon,
                IsPublic = IsPublic,
                StartTimeStamp = StartTimeStamp,
                EndTimeStamp = EndTimeStamp,
                Status = Status,
                StartedBy = StartedBy,
                CompletedBy = CompletedBy,
            };
        }

        public List<SystemTestStepResult> StepResults
        {
            get; set;
        } = new List<SystemTestStepResult>();

        
        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    public class SystemTestStepResult
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public EntityHeader CompletedBy { get; set; }
        public EntityHeader SystemTestStep { get; set; }
        public string TimeStamp { get; set; }

        public bool? Passed { get; set; }
        public string ReasonFailed { get; set; }
        public string Notes { get; set; }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.SystemTestExecutions_Title, DeploymentAdminResources.Names.SystemTestExecutions_Description,
        DeploymentAdminResources.Names.SystemTestStep_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-laptop-protection",
           GetListUrl: "/api/systemtest/executions", SaveUrl: "/api/systemtest/execution", GetUrl: "/api/systemtest/execution/{id}", DeleteUrl: "/api/systemtest/execution/{id}", FactoryUrl: "/api/systemtest/execution/factory")]
    public class SystemTestExecutionSummary : SummaryData
    {
        public EntityHeader Status { get; set; }
        public EntityHeader StartedBy { get; set; }
        public EntityHeader CompletedBy { get; set; }
        public string StartTimeStamp { get; set; }
        public string EndTimeStamp { get; set; }
    }

    public class TestStepUpdate
    {
        public string Notes { get; set; }
        public bool Passed { get; set; }
        public string ReasonFailed { get; set; }
    }
}
