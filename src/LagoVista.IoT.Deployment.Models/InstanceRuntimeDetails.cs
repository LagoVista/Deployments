// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 472a0e99a3eaffc54d070495c58f01dcf90ad51796051087bd01a99fc1a8c387
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

/// <summary>
/// These are some runtime classes used to get data from a running host and instnce. 
/// </summary>
namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class InstanceRuntimeSummary
    {
        [JsonProperty("hostId")]
        public string HostId { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentInstanceStates Status { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }
        [JsonProperty("instanceName")]
        public string InstanceName { get; set; }
        [JsonProperty("solutionId")]
        public string SolutionId { get; set; }
        [JsonProperty("solutionName")]
        public string SolutionName { get; set; }
    }

    public abstract class InstanceDetailsBase
    {
        [JsonProperty("fullId")]
        public String FullId { get; set; }

        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("key")]
        public String Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public class InstanceRuntimeDetails : InstanceDetailsBase
    {
        public InstanceRuntimeDetails()
        {
            Listeners = new ObservableCollection<ListenerSummary>();
            DeviceConfigurations = new ObservableCollection<DeviceConfigSummary>();
        }

        [JsonProperty("planner")]
        public PlannerSummary Planner { get; set; }

        [JsonProperty("archiveStorage")]
        public EntityHeader ArchiveStorage { get; set; }

        [JsonProperty("deviceRepoReader")]
        public EntityHeader DeviceRepoReader { get; set; }

        [JsonProperty("deviceRepoWriter")]
        public EntityHeader DeviceRepoWriter{ get; set; }

        [JsonProperty("pemStorage")]
        public EntityHeader PEMStorage { get; set; }


        [JsonProperty("listeners")]
        public ObservableCollection<ListenerSummary> Listeners { get; set; }

        [JsonProperty("deviceConfigurations")]
        public ObservableCollection<DeviceConfigSummary> DeviceConfigurations { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentInstanceStates Status { get; set; }
    }

    public class PlannerSummary : InstanceDetailsBase
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineModuleStatus Status { get; set; }
        [JsonProperty("deviceIdParsers")]
        public List<MessageAttributeParser> DeviceIdParsers { get; private set; }
        [JsonProperty("messageIdParsers")]
        public List<MessageAttributeParser> MessageTypeIdParsers { get; private set; }

        public static PlannerSummary Create(PlannerConfiguration planner)
        {
            var summary = new PlannerSummary();
            summary.Id = planner.Id;
            summary.Key = planner.Key;
            summary.Name = planner.Name;
            summary.MessageTypeIdParsers = planner.MessageTypeIdParsers;
            summary.DeviceIdParsers = planner.DeviceIdParsers;
            return summary;
        }
    }

    public class WorkflowInstanceSummary : PipelineInstanceSummary
    {
        [JsonProperty("attributes")]
        public List<LagoVista.IoT.DeviceAdmin.Models.Attribute> Attributes { get; set; }
        [JsonProperty("inputCommands")]
        public List<InputCommand> InputCommands { get; set; }
    }

    public class DeviceConfigSummary : InstanceDetailsBase
    {
        public DeviceConfigSummary()
        {
            Routes = new ObservableCollection<RouteSummary>();
        }

        [JsonProperty("routes")]
        public ObservableCollection<RouteSummary> Routes { get; set; }
    }

    public class DeviceMessageTypeSummary : InstanceDetailsBase
    { 
        [JsonProperty("value")]
        public DeviceMessageDefinition Value { get; set; }
    }

    public class RouteSummary : InstanceDetailsBase
    {
        public RouteSummary()
        {
            PipelineModules = new ObservableCollection<PipelineInstanceSummary>();
            MessageTypes = new ObservableCollection<DeviceMessageTypeSummary>();
        }

        [JsonProperty("pipelineModules")]
        public ObservableCollection<PipelineInstanceSummary> PipelineModules { get; set; }

        [JsonProperty("messageTypes")]
        public ObservableCollection<DeviceMessageTypeSummary> MessageTypes { get; set; }
    }

    public class PipelineInstanceSummary : InstanceDetailsBase
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PipelineModuleStatus Status { get; set; }
        [JsonProperty("type")]
        public String Type { get; set; }
        [JsonProperty("usgaeMetrics")]
        public UsageMetrics UsageMetrics { get; set; }
    }

    public class ListenerSummary : PipelineInstanceSummary
    {
        [JsonProperty("port")]
        public int? Port { get; set; }

        [JsonProperty("listenerType")]
        public string ListenerType{ get; set; }
    }

}
