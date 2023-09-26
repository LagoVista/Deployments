using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    /*
var credentials = new TokenCloudCredentials("", "");
var client = new Microsoft.Azure.Management.Resources.ResourceManagementClient(credentials);
var result = c.ResourceGroups.CreateOrUpdateAsync("MyResourceGroup", new Microsoft.Azure.Management.ResourceGroup("West US"), new System.Threading.CancellationToken()).Result */


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Solution_Title, DeploymentAdminResources.Names.Solution_Help, DeploymentAdminResources.Names.Solution_Description, 
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        GetUrl: "/api/deployment/solution/{id}", GetListUrl: "/api/deployment/solutions", SaveUrl: "/api/deployment/solution", FactoryUrl: "/api/deployment/solution/factory", DeleteUrl: "/api/deployment/solution/{id}")]
    public class Solution : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IFormDescriptor
    {
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public Solution()
        {
            DeviceConfigurations = new List<EntityHeader<DeviceConfiguration>>();
            Listeners = new List<EntityHeader<ListenerConfiguration>>();
            Settings = new List<CustomField>();
            Icon = "icon-ae-deployment-instance";
        }

        public EntityHeader Environment
        {
            get;
            set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_IsPublic, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources))]
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name,
            };
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_DefaultListener, HelpResource: DeploymentAdminResources.Names.Solution_DefaultListener_Help, WaterMark: DeploymentAdminResources.Names.Solution_DefaultListener_Select, IsRequired:false, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader DefaultListener { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Deployment_Listeners, HelpResource: DeploymentAdminResources.Names.Deployment_Listeners_Help, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<ListenerConfiguration>> Listeners { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Deployment_Planner, WaterMark: DeploymentAdminResources.Names.Deployment_Planner_Select, HelpResource: DeploymentAdminResources.Names.Deployment_Planner_Help, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:true)]
        public EntityHeader<PlannerConfiguration> Planner { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_Settings, HelpResource: DeploymentAdminResources.Names.Solution_Settings_Help, ResourceType: typeof(DeploymentAdminResources))]
        public List<CustomField> Settings { get; set; }

        public string Icon { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_DeviceConfigurations, HelpResource: DeploymentAdminResources.Names.Solution_DeviceConfigurations_Help, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<DeviceConfiguration>> DeviceConfigurations { get; set; }

        public string MonitoringEndpoint { get; set; }

        public SolutionSummary CreateSummary()
        {
            return new SolutionSummary()
            {
                Id = Id,
                Key = Key,
                Name = Name,
                IsPublic = IsPublic,
                Description = Description,
                Icon = Icon,
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Solution.Name),
                nameof(Solution.Key),
                nameof(Solution.Planner),
                nameof(Solution.Description),
                nameof(Solution.DeviceConfigurations),
            };
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Solution_Title, DeploymentAdminResources.Names.Solution_Help, DeploymentAdminResources.Names.Solution_Description,
     EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources),
     GetUrl: "/api/deployment/solution/{id}", GetListUrl: "/api/deployment/solutions", SaveUrl: "/api/deployment/solution", FactoryUrl: "/api/deployment/solution/factory", DeleteUrl: "/api/deployment/solution/{id}")]
    public class SolutionSummary : LagoVista.Core.Models.SummaryData
    { 
        public string Icon { get; set; }
    }
}
