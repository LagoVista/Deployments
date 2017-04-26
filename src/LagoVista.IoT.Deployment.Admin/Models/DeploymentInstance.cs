using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentHostStates
    {
        New,
        Initializing,
        Running,
        Paused,
        Stopping,
        Stopped,
        Degraded,
        FatalError,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Instance_Title, Resources.DeploymentAdminResources.Names.Instance_Help, Resources.DeploymentAdminResources.Names.Instance_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentInstance : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity
    {

        public const string Status_New = "new";
        public const string Status_Initializing = "initializing";
        public const string Status_Running = "running";
        public const string Status_Paused = "paused";
        public const string Status_Stopping = "stopping";
        public const string Status_Stopped = "stopped";
        public const string Status_Degraded = "degraded";
        public const string Status_FatalError = "fatalerror";

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_IsDeployed, HelpResource:DeploymentAdminResources.Names.Instance_IsDeployed_Help, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources), IsUserEditable:false)]
        public bool IsDeployed { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Status, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader<DeploymentHostStates> Status { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Host, HelpResource:Resources.DeploymentAdminResources.Names.Instance_Host_Help, WaterMark: DeploymentAdminResources.Names.Instance_Host_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeploymentHost> Host { get; set; }

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

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Solution, WaterMark:DeploymentAdminResources.Names.Instance_Solution_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired:true)]
        public EntityHeader<Solution> Solution { get; set; }
        public DeploymentInstanceSummary CreateSummary()
        {
            return new DeploymentInstanceSummary()
            {
                Description = Description,
                Name = Name,
                Key = Key,
                Id = Id,
                IsPublic = IsPublic
            };
        }

    }

    public class DeploymentInstanceSummary : SummaryData
    {

    }
}
