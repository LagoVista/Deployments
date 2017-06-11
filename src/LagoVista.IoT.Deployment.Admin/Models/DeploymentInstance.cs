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
    public enum DeploymentInstanceStates
    {
        [EnumLabel(DeploymentInstance.Status_NotDeployed, DeploymentAdminResources.Names.InstanceStates_NotDeployed, typeof(DeploymentAdminResources))]
        NotDeployed,
        [EnumLabel(DeploymentInstance.Status_Deploying, DeploymentAdminResources.Names.InstanceStates_Deploying, typeof(DeploymentAdminResources))]
        Deploying,
        [EnumLabel(DeploymentInstance.Status_Initializing, DeploymentAdminResources.Names.InstanceStates_Initializing, typeof(DeploymentAdminResources))]
        Initializing,
        [EnumLabel(DeploymentInstance.Status_Ready, DeploymentAdminResources.Names.InstanceStates_Ready, typeof(DeploymentAdminResources))]
        Ready,
        [EnumLabel(DeploymentInstance.Status_Starting, DeploymentAdminResources.Names.InstanceStates_Starting, typeof(DeploymentAdminResources))]
        Starting,
        [EnumLabel(DeploymentInstance.Status_Running, DeploymentAdminResources.Names.InstanceStates_Running, typeof(DeploymentAdminResources))]
        Running,
        [EnumLabel(DeploymentInstance.Status_Pausing, DeploymentAdminResources.Names.InstanceStates_Pausing, typeof(DeploymentAdminResources))]
        Pausing,
        [EnumLabel(DeploymentInstance.Status_Paused, DeploymentAdminResources.Names.InstanceStates_Paused, typeof(DeploymentAdminResources))]
        Paused,
        [EnumLabel(DeploymentInstance.Status_Stopping, DeploymentAdminResources.Names.InstanceStates_Stopping, typeof(DeploymentAdminResources))]
        Stopping,
        [EnumLabel(DeploymentInstance.Status_Stopped, DeploymentAdminResources.Names.InstanceStates_Stopped, typeof(DeploymentAdminResources))]
        Stopped,
        [EnumLabel(DeploymentInstance.Status_Degraded, DeploymentAdminResources.Names.InstanceStates_Degraded, typeof(DeploymentAdminResources))]
        Degraded,
        [EnumLabel(DeploymentInstance.Status_FatalError, DeploymentAdminResources.Names.InstanceStates_FatalError, typeof(DeploymentAdminResources))]
        FatalError,
        [EnumLabel(DeploymentInstance.Status_Undeploying, DeploymentAdminResources.Names.InstanceStates_Undeploying, typeof(DeploymentAdminResources))]
        Undeploying,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Instance_Title, Resources.DeploymentAdminResources.Names.Instance_Help, Resources.DeploymentAdminResources.Names.Instance_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentInstance : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity
    {
        public DeploymentInstance()
        {
            Status = new EntityHeader<DeploymentInstanceStates>();
            SetState(DeploymentInstanceStates.NotDeployed);
        }

        public void SetState(DeploymentInstanceStates newState)
        {            
            Status.Value = newState;

            switch (newState)
            {
                case DeploymentInstanceStates.Undeploying:
                    Status.Id = Status_Undeploying;
                    Status.Text = DeploymentAdminResources.InstanceStates_Undeploying;
                    break;

                case DeploymentInstanceStates.NotDeployed:
                    Status.Id = Status_NotDeployed;
                    Status.Text = DeploymentAdminResources.InstanceStates_NotDeployed;
                    break;
                case DeploymentInstanceStates.Deploying:
                    Status.Id = Status_Deploying;
                    Status.Text = DeploymentAdminResources.InstanceStates_Deploying;
                    break;
                case DeploymentInstanceStates.Initializing:
                    Status.Id = Status_Initializing;
                    Status.Text = DeploymentAdminResources.InstanceStates_Initializing;
                    break;
                case DeploymentInstanceStates.Ready:
                    Status.Id = Status_Ready;
                    Status.Text = DeploymentAdminResources.InstanceStates_Ready;
                    break;
                case DeploymentInstanceStates.Starting:
                    Status.Id = Status_Starting;
                    Status.Text = DeploymentAdminResources.InstanceStates_Starting;
                    break;
                case DeploymentInstanceStates.Running:
                    Status.Id = Status_Running;
                    Status.Text = DeploymentAdminResources.InstanceStates_Running;
                    break;
                case DeploymentInstanceStates.Pausing:
                    Status.Id = Status_Pausing;
                    Status.Text = DeploymentAdminResources.InstanceStates_Pausing;
                    break;
                case DeploymentInstanceStates.Paused:
                    Status.Id = Status_Paused;
                    Status.Text = DeploymentAdminResources.InstanceStates_Paused;
                    break;
                case DeploymentInstanceStates.Stopping:
                    Status.Id = Status_Stopping;
                    Status.Text = DeploymentAdminResources.InstanceStates_Stopping;
                    break;
                case DeploymentInstanceStates.Stopped:
                    Status.Id = Status_Stopped;
                    Status.Text = DeploymentAdminResources.InstanceStates_Stopped;
                    break;
                case DeploymentInstanceStates.Degraded:
                    Status.Id = Status_Degraded;
                    Status.Text = DeploymentAdminResources.InstanceStates_Degraded;
                    break;
                case DeploymentInstanceStates.FatalError:
                    Status.Id = Status_FatalError;
                    Status.Text = DeploymentAdminResources.InstanceStates_FatalError;
                    break;
            }
        }

        public const string Status_NotDeployed = "notdeployed";
        public const string Status_Deploying = "deploying";
        public const string Status_Initializing = "initializing";
        public const string Status_Ready = "ready";
        public const string Status_Starting = "starting";
        public const string Status_Running = "running";
        public const string Status_Pausing = "pausing";
        public const string Status_Paused = "paused";
        public const string Status_Stopping = "stopping";
        public const string Status_Stopped = "stopped";
        public const string Status_Degraded = "degraded";
        public const string Status_FatalError = "fatalerror";
        public const string Status_Undeploying = "undeployingr";

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public String Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_IsDeployed, HelpResource: DeploymentAdminResources.Names.Instance_IsDeployed_Help, FieldType: FieldTypes.Bool, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public bool IsDeployed { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Status, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public EntityHeader<DeploymentInstanceStates> Status { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Host, HelpResource: Resources.DeploymentAdminResources.Names.Instance_Host_Help, WaterMark: DeploymentAdminResources.Names.Instance_Host_Watermark, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources))]
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

        [FormField(LabelResource: DeploymentAdminResources.Names.Instance_Solution, WaterMark: DeploymentAdminResources.Names.Instance_Solution_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public EntityHeader<Solution> Solution { get; set; }
        public DeploymentInstanceSummary CreateSummary()
        {
            return new DeploymentInstanceSummary()
            {
                Description = Description,
                Name = Name,
                Key = Key,
                Id = Id,
                IsPublic = IsPublic,
                IsDeployed = IsDeployed,
                Status = Status
            };
        }

    }

    public class DeploymentInstanceSummary : SummaryData
    {
        public EntityHeader<DeploymentInstanceStates> Status { get; set; }
        public bool IsDeployed { get; set; }
    }
}
