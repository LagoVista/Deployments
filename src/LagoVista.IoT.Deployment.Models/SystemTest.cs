// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b5472c53eefe397906333d7200fed23df1c440e5f69a9bae2408c1fc6576a59c
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.SystemTest_Title, DeploymentAdminResources.Names.SystemTest_Title,
        DeploymentAdminResources.Names.SystemTestStep_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-laptop-protection", Cloneable: true,
        ListUIUrl: "/iotstudio/settings/systemtests", EditUIUrl: "/iotstudio/settings/systemtest/{id}", CreateUIUrl: "/iotstudio/settings/systemtest/add",
        GetListUrl: "/api/systemtests", SaveUrl: "/api/systemtest", GetUrl: "/api/systemtest/{id}", DeleteUrl: "/api/systemtest/{id}", FactoryUrl: "/api/systemtest/factory")]
    public class SystemTest : EntityBase, ISummaryFactory, IIconEntity, IDescriptionEntity, IValidateable, IFormDescriptor
    {
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-fo-laptop-protection";

        
        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTest_Steps, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/systemtest/step/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<SystemTestStep> Steps { get; set; } = new List<SystemTestStep>();


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTest_OnFailedInstructions, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public string OnFailureInstructions { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTest_Schedule, FieldType: FieldTypes.Schedule, ResourceType: typeof(DeploymentAdminResources))]
        public Schedule Schedule { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTest_EstimatedDuration, FieldType: FieldTypes.Integer, HelpResource: DeploymentAdminResources.Names.SystemTest_EstimatedDuration_Help, ResourceType: typeof(DeploymentAdminResources))]
        public int EstimatedDuration { get; set; } = 15;

        public SystemTestSummary CreateSummary()
        {
            return new SystemTestSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                Icon = Icon,
                IsPublic = IsPublic,
                Description = Description
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Category),
                nameof(Schedule),
                nameof(OnFailureInstructions),
                nameof(Description),
                nameof(EstimatedDuration),
                nameof(Steps),
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.SystemTests_Title, DeploymentAdminResources.Names.SystemTest_Title,
        DeploymentAdminResources.Names.SystemTestStep_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-fo-laptop-protection",
        ListUIUrl: "/iotstudio/settings/systemtests", EditUIUrl: "/iotstudio/settings/systemtest/{id}", CreateUIUrl: "/iotstudio/settings/systemtest/add",
        GetListUrl: "/api/systemtests", SaveUrl: "/api/systemtest", GetUrl: "/api/systemtest/{id}", DeleteUrl: "/api/systemtest/{id}", FactoryUrl: "/api/systemtest/factory")]
    public class SystemTestSummary : SummaryData
    {

    }
}
