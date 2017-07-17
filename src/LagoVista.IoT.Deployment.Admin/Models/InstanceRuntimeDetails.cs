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
            Listeners = new ObservableCollection<PipelineInstanceSummary>();
            DeviceConfigurations = new ObservableCollection<DeviceConfigSummary>();
        }


        [JsonProperty("planner")]
        public PipelineInstanceSummary Planner { get; set; }

        [JsonProperty("listeners")]
        public ObservableCollection<PipelineInstanceSummary> Listeners { get; set; }

        [JsonProperty("deviceConfigurations")]
        public ObservableCollection<DeviceConfigSummary> DeviceConfigurations { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentInstanceStates Status { get; set; }
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
}
