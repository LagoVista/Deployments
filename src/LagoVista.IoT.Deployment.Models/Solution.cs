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
        EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-pz-solution",  Cloneable:true,
        GetUrl: "/api/deployment/solution/{id}", GetListUrl: "/api/deployment/solutions", SaveUrl: "/api/deployment/solution", FactoryUrl: "/api/deployment/solution/factory", DeleteUrl: "/api/deployment/solution/{id}",
        ListUIUrl: "/iotstudio/manage/solutions", EditUIUrl: "/iotstudio/manage/solution/{id}", CreateUIUrl: "/iotstudio/manage/solution/add",
        HelpUrl: "https://docs.nuviot.com/Deployment/Solution.html")]
    public class Solution : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IIconEntity, ISummaryFactory, ICategorized
    {

        public Solution()
        {
            DeviceConfigurations = new List<EntityHeader<DeviceConfiguration>>();
            Listeners = new List<EntityHeader<ListenerConfiguration>>();
            Settings = new List<CustomField>();
            Icon = "icon-pz-solution";
            Version = "1.0";
        }

        public EntityHeader Environment
        {
            get;
            set;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FKeyProperty(nameof(ListenerConfiguration), nameof(DefaultListener) + ".Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_DefaultListener, HelpResource: DeploymentAdminResources.Names.Solution_DefaultListener_Help,  WaterMark: DeploymentAdminResources.Names.Solution_DefaultListener_Select, IsRequired:false, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader DefaultListener { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_Version, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources))]
        public string Version { get; set; }

        [FKeyProperty(nameof(ListenerConfiguration), nameof(Listeners) + "[*].Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.Deployment_Listeners, HelpResource: DeploymentAdminResources.Names.Deployment_Listeners_Help,
            ChildListDisplayMember: nameof(EntityHeader.Text), FieldType: FieldTypes.ChildListInlinePicker, EntityHeaderPickerUrl: "/api/pipeline/admin/listeners", ResourceType: typeof(DeploymentAdminResources))]
        public List<EntityHeader<ListenerConfiguration>> Listeners { get; set; }


        [FKeyProperty(nameof(PlannerConfiguration), nameof(Planner) + ".Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.Deployment_Planner, WaterMark: DeploymentAdminResources.Names.Deployment_Planner_Select, EntityHeaderPickerUrl: "/api/pipeline/admin/planners", 
            HelpResource: DeploymentAdminResources.Names.Deployment_Planner_Help, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:true)]
        public EntityHeader<PlannerConfiguration> Planner { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_Settings, ChildListDisplayMember:"name", FieldType: FieldTypes.ChildListInline, HelpResource: DeploymentAdminResources.Names.Solution_Settings_Help, ResourceType: typeof(DeploymentAdminResources))]
        public List<CustomField> Settings { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_Icon, FieldType:FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }

        [FKeyProperty(nameof(DeviceConfiguration), nameof(DeviceConfigurations) + "[*].Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.Solution_DeviceConfigurations, HelpResource: DeploymentAdminResources.Names.Solution_DeviceConfigurations_Help, 
            ChildListDisplayMember:nameof(EntityHeader.Text), FieldType: FieldTypes.ChildListInlinePicker, EntityHeaderPickerUrl: "/api/deviceconfigs", ResourceType: typeof(DeploymentAdminResources))]
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
                Category = Category
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Solution.Name),
                nameof(Solution.Version),
                nameof(Solution.Icon),
                nameof(Solution.Key),
                nameof(Solution.Category),
                nameof(Solution.DefaultListener),
                nameof(Solution.Planner),
                nameof(Solution.Description),
                nameof(Solution.Settings),
                nameof(Solution.Listeners),
                nameof(Solution.DeviceConfigurations),
            };
        }

        Core.Interfaces.ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Solutions_Title, DeploymentAdminResources.Names.Solution_Help, DeploymentAdminResources.Names.Solution_Description,
     EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-pz-solution", Cloneable: true,
     GetUrl: "/api/deployment/solution/{id}", GetListUrl: "/api/deployment/solutions", SaveUrl: "/api/deployment/solution", FactoryUrl: "/api/deployment/solution/factory", DeleteUrl: "/api/deployment/solution/{id}")]
    public class SolutionSummary : LagoVista.Core.Models.CategorizedSummaryData
    { 
    }
}
