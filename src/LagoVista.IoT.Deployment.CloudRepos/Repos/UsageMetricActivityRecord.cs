using LagoVista.Core.Interfaces;
using System;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class UsageMetricActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }

        public string SourceId { get; set; }
        public string HostId { get; set; }
        public string InstanceId { get; set; }
        public string PipelineModuleId { get; set; }

        public string StartTimeStamp { get; set; }
        public string EndTimeStamp { get; set; }
        public double ElapsedMS { get; set; }
        public double MessagesPerSecond { get; set; }
        public double AverageProcessingMS { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public int MessagesProcessed { get; set; }
        public int DeadLetterCount { get; set; }
        public long BytesProcessed { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int ActiveCount { get; set; }
        public double ProcessingMS { get; set; }
    }
}
