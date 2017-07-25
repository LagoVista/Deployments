using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models;
using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Models
{

    public enum HostTypes
    {
        [EnumLabel(DeploymentHost.HostType_Free, DeploymentAdminResources.Names.Host_Type_Free, typeof(DeploymentAdminResources))]
        Free,
        [EnumLabel(DeploymentHost.HostType_Community, DeploymentAdminResources.Names.Host_Type_Community, typeof(DeploymentAdminResources))]
        Community,
        [EnumLabel(DeploymentHost.HostType_Shared, DeploymentAdminResources.Names.Host_Type_Shared, typeof(DeploymentAdminResources))]
        Shared,
        [EnumLabel(DeploymentHost.HostType_SharedHighPerformacne, DeploymentAdminResources.Names.Host_Type_SharedHighPerformance, typeof(DeploymentAdminResources))]
        SharedHighPerformance,
        [EnumLabel(DeploymentHost.HostType_Dedicated, DeploymentAdminResources.Names.Host_Type_Dedicated, typeof(DeploymentAdminResources))]
        Dedicated,
        [EnumLabel(DeploymentHost.HostType_Clustered, DeploymentAdminResources.Names.Host_Type_Clustered, typeof(DeploymentAdminResources))]
        Clustered,
        [EnumLabel(DeploymentHost.HostType_MCP, DeploymentAdminResources.Names.Host_Type_MCP, typeof(DeploymentAdminResources))]
        MCP,
        [EnumLabel(DeploymentHost.HostType_Notifications, DeploymentAdminResources.Names.Host_Type_Notifications, typeof(DeploymentAdminResources))]
        Notification,
        [EnumLabel(DeploymentHost.HostType_Development, DeploymentAdminResources.Names.HostType_Development, typeof(DeploymentAdminResources))]
        Development

    }

    public enum HostStatus
    {
        [EnumLabel(DeploymentHost.HostStatus_Offline, DeploymentAdminResources.Names.HostStatus_Offline, typeof(DeploymentAdminResources))]
        Offline,
        [EnumLabel(DeploymentHost.HostStatus_Failed, DeploymentAdminResources.Names.HostStatus_Failed, typeof(DeploymentAdminResources))]
        Failed,
        [EnumLabel(DeploymentHost.HostStatus_Stopped, DeploymentAdminResources.Names.HostStatus_Stopped, typeof(DeploymentAdminResources))]
        Stopped,
        [EnumLabel(DeploymentHost.HostStatus_Stopping, DeploymentAdminResources.Names.HostStatus_Stopping, typeof(DeploymentAdminResources))]
        Stopping,
        [EnumLabel(DeploymentHost.HostStatus_Deploying, DeploymentAdminResources.Names.HostStatus_Deploying, typeof(DeploymentAdminResources))]
        Deploying,
        [EnumLabel(DeploymentHost.HostStatus_Starting, DeploymentAdminResources.Names.HostStatus_Starting, typeof(DeploymentAdminResources))]
        Starting,
        [EnumLabel(DeploymentHost.HostStatus_Running, DeploymentAdminResources.Names.HostStatus_Running, typeof(DeploymentAdminResources))]
        Running,
        [EnumLabel(DeploymentHost.HostStatus_StoppingDegraded, DeploymentAdminResources.Names.HostStatus_StoppingDegraded, typeof(DeploymentAdminResources))]
        StoppingHostStatus_Degraded,
        [EnumLabel(DeploymentHost.HostStatus_Degraded, DeploymentAdminResources.Names.HostStatus_Degraded, typeof(DeploymentAdminResources))]
        Degraded,
        [EnumLabel(DeploymentHost.HostStatus_Destorying, DeploymentAdminResources.Names.HostStatus_Destroying, typeof(DeploymentAdminResources))]
        Destroying,
    }

    public enum HostCapacityStatus
    {
        [EnumLabel(DeploymentHost.HostCapacity_underutilized, DeploymentAdminResources.Names.HostCapacity_Underutlized, typeof(DeploymentAdminResources))]
        UnderUtilized,
        [EnumLabel(DeploymentHost.HostCapacity_Ok, DeploymentAdminResources.Names.HostCapacity_OverCapacity, typeof(DeploymentAdminResources))]
        Ok,
        [EnumLabel(DeploymentHost.HostCapacity_75Percent, DeploymentAdminResources.Names.HostCapacity_75Percent, typeof(DeploymentAdminResources))]
        At75Percent,
        [EnumLabel(DeploymentHost.HostCapacity_90Percent, DeploymentAdminResources.Names.HostCapacity_90Percent, typeof(DeploymentAdminResources))]
        At90Percent,
        [EnumLabel(DeploymentHost.HostCapacity_atcapacity, DeploymentAdminResources.Names.HostCapacity_AtCapacity, typeof(DeploymentAdminResources))]
        AtCapacity,
        [EnumLabel(DeploymentHost.HostCapacity_overcapacity, DeploymentAdminResources.Names.HostCapacity_OverCapacity, typeof(DeploymentAdminResources))]
        OverCapacity,
        [EnumLabel(DeploymentHost.HostCapacity_failureimmintent, DeploymentAdminResources.Names.HostCapacity_FailureImminent, typeof(DeploymentAdminResources))]
        FailureImminent
    }

    public enum HostSizes
    {
        [EnumLabel(DeploymentHost.HostSize_ExtraSmall, DeploymentAdminResources.Names.HostSize_ExtraSmall, typeof(DeploymentAdminResources))]
        ExtraSmall,
        [EnumLabel(DeploymentHost.HostSize_Small, DeploymentAdminResources.Names.HostSize_Small, typeof(DeploymentAdminResources))]
        Small,
        [EnumLabel(DeploymentHost.HostSize_Medium, DeploymentAdminResources.Names.HostSize_Medium, typeof(DeploymentAdminResources))]
        Medium,
        [EnumLabel(DeploymentHost.HostSize_Large, DeploymentAdminResources.Names.HostSize_Large, typeof(DeploymentAdminResources))]
        Large,
        [EnumLabel(DeploymentHost.HostSize_ExtraLarge, DeploymentAdminResources.Names.HostSize_ExtraLarge, typeof(DeploymentAdminResources))]
        ExtraLarge
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Host_Title, Resources.DeploymentAdminResources.Names.Host_Help, Resources.DeploymentAdminResources.Names.Host_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class DeploymentHost : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity
    {
        public const string HostSize_ExtraSmall = "extrasmall";
        public const string HostSize_Small = "small";
        public const string HostSize_Medium = "medium";
        public const string HostSize_Large = "large";
        public const string HostSize_ExtraLarge = "extralarge";

        public const string HostType_Free = "free";
        public const string HostType_Community = "community";
        public const string HostType_Shared = "shared";
        public const string HostType_SharedHighPerformacne = "shared_highperformance";
        public const string HostType_Dedicated = "dedicated";
        public const string HostType_Clustered = "clustered";
        public const string HostType_Notifications = "notifications";
        public const string HostType_MCP = "mcp";
        public const string HostType_Development = "development";

        public const string HostStatus_Offline = "offline";
        public const string HostStatus_Failed = "failed";
        public const string HostStatus_Stopped = "stopped";
        public const string HostStatus_Deploying = "deploying";
        public const string HostStatus_Destorying = "destroying";
        public const string HostStatus_Starting = "starting";
        public const string HostStatus_Running = "running";
        public const string HostStatus_Stopping = "stopping";
        public const string HostStatus_Degraded = "degraded";
        public const string HostStatus_StoppingDegraded = "stopping-degraded";

        public const string HostCapacity_underutilized = "underutilized";
        public const string HostCapacity_Ok = "ok";
        public const string HostCapacity_75Percent = "75percent";
        public const string HostCapacity_90Percent = "90percent";

        public const string HostCapacity_atcapacity = "atcapacity";
        public const string HostCapacity_overcapacity = "overcapacity";
        public const string HostCapacity_failureimmintent = "failureimminent";

        public DeploymentHost()
        {
            Status = new EntityHeader<HostStatus>();
            CapacityStatus = new EntityHeader<HostCapacityStatus>();
            HostType = new EntityHeader<HostTypes>();

            SetCapacityStatus(HostCapacityStatus.UnderUtilized);
            SetHostState(HostStatus.Offline);

            CloudProvider = new EntityHeader() { Text = "Digital Ocean", Id = "378463ADF57B4C02B60FEF4DCB30F7E2" };
            GenerateAccessKeys();
        }

        public void SetHostState(HostStatus hostState)
        {
            Status.Value = hostState;


            switch (hostState)
            {
                case HostStatus.StoppingHostStatus_Degraded:
                    Status.Id = HostStatus_StoppingDegraded;
                    Status.Text = DeploymentAdminResources.HostStatus_StoppingDegraded;
                    break;
                case HostStatus.Offline:
                    Status.Id = HostStatus_Offline;
                    Status.Text = DeploymentAdminResources.HostStatus_Offline;
                    break;
                case HostStatus.Failed:
                    Status.Id = HostStatus_Failed;
                    Status.Text = DeploymentAdminResources.HostStatus_Failed;
                    break;
                case HostStatus.Running:
                    Status.Id = HostStatus_Running;
                    Status.Text = DeploymentAdminResources.HostStatus_Running;
                    break;
                case HostStatus.Starting:
                    Status.Id = HostStatus_Starting;
                    Status.Text = DeploymentAdminResources.HostStatus_Starting;
                    break;
                case HostStatus.Stopped:
                    Status.Id = HostStatus_Stopped;
                    Status.Text = DeploymentAdminResources.HostStatus_Stopped;
                    break;
                case HostStatus.Stopping:
                    Status.Id = HostStatus_Stopping;
                    Status.Text = DeploymentAdminResources.HostStatus_Stopping;
                    break;
                case HostStatus.Degraded:
                    Status.Id = HostStatus_Degraded;
                    Status.Text = DeploymentAdminResources.HostStatus_Degraded;
                    break;
            }
        }

        public void SetCapacityStatus(HostCapacityStatus capacityStatus)
        {
            CapacityStatus.Value = capacityStatus;

            switch (capacityStatus)
            {
                case HostCapacityStatus.Ok:
                    CapacityStatus.Id = HostCapacity_Ok;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_Ok;
                    break;
                case HostCapacityStatus.At75Percent:
                    CapacityStatus.Id = HostCapacity_75Percent;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_75Percent;
                    break;
                case HostCapacityStatus.At90Percent:
                    CapacityStatus.Id = HostCapacity_90Percent;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_90Percent;
                    break;
                case HostCapacityStatus.AtCapacity:
                    CapacityStatus.Id = HostCapacity_atcapacity;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_AtCapacity;
                    break;
                case HostCapacityStatus.FailureImminent:
                    CapacityStatus.Id = HostCapacity_failureimmintent;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_FailureImminent;
                    break;
                case HostCapacityStatus.OverCapacity:
                    CapacityStatus.Id = HostCapacity_overcapacity;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_OverCapacity;
                    break;
                case HostCapacityStatus.UnderUtilized:
                    CapacityStatus.Id = HostCapacity_underutilized;
                    CapacityStatus.Text = DeploymentAdminResources.HostCapacity_Underutlized;
                    break;
            }
        }

        public void GenerateAccessKeys()
        {
            HostAccessKey1 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
            HostAccessKey2 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Common_Key, HelpResource: Resources.DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Host_Type, EnumType: (typeof(HostTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Host_Type_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<HostTypes> HostType { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Host_Size, EnumType: (typeof(HostSizes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Host_SelectSize, IsRequired: true)]
        public EntityHeader<HostSizes> Size { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Host_Status, EnumType: (typeof(HostStatus)), FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Host_Type_Select, IsRequired: false, IsUserEditable: false)]
        public EntityHeader<HostStatus> Status { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Host_CapacityStatus, EnumType: (typeof(HostCapacityStatus)), FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), WaterMark: DeploymentAdminResources.Names.Host_Type_Select, IsRequired: false, IsUserEditable: false)]
        public EntityHeader<HostCapacityStatus> CapacityStatus { get; set; }


        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_ContainerRepository, WaterMark: Resources.DeploymentAdminResources.Names.Host_ContainerRepository_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public EntityHeader ContainerRepository { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_ContainerTag, WaterMark: Resources.DeploymentAdminResources.Names.Host_ContainerTag_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public EntityHeader ContainerTag { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_DNSName, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: true)]
        public string DnsHostName { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_IPv4_Address, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: false)]
        public string Ipv4Address { get; set; }


        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_CloudProvider, HelpResource: Resources.DeploymentAdminResources.Names.Host_CloudProvider_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: true)]
        public EntityHeader CloudProvider { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_AdminAPIUri, HelpResource: Resources.DeploymentAdminResources.Names.Host_AdminAPIUri_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: false)]
        public string AdminAPIUri { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_ComputeResourceId, HelpResource: Resources.DeploymentAdminResources.Names.Host_ComputeResourceId_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false, IsRequired: false)]
        public string ComputeResourceId { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_ComputeResource_Uri, HelpResource: Resources.DeploymentAdminResources.Names.Host_ComputeResource_Uri_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: false)]
        public string ComputeResourceUri { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_AverageCPU_30_Minutes, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string AverageCPU { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_AverageNetwork_30_Minutes, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string AverageNetwork { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_AverageMemory_30_Minutes, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public string AverageMemory { get; set; }

        [FormField(LabelResource: Resources.DeploymentAdminResources.Names.Host_Subscription, WaterMark: Resources.DeploymentAdminResources.Names.Host_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }

        public String HostAccessKey1 { get; set; }

        public String HostAccessKey2 { get; set; }

        public DeploymentHostSummary CreateSummary()
        {
            return new DeploymentHostSummary()
            {
                Id = Id,
                Name = Name,
                Status = Status.Text,
                CapacityStatus = CapacityStatus.Text,
                HostType = HostType.Text,
                Key = Key,
                Description = Description,
                IsPublic = IsPublic
            };
        }
    }

    public class DeploymentHostSummary : SummaryData
    {
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.Host_Type, ResourceType: typeof(DeploymentAdminResources))]
        public string HostType { get; set; }


        [ListColumn(HeaderResource: DeploymentAdminResources.Names.Host_Status, ResourceType: typeof(DeploymentAdminResources))]
        public string Status { get; set; }

        [ListColumn(HeaderResource: DeploymentAdminResources.Names.Host_CapacityStatus, ResourceType: typeof(DeploymentAdminResources))]
        public string CapacityStatus { get; set; }
    }

}
