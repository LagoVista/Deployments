using LagoVista.Core.Interfaces;
using System;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    /// <summary>
    /// Immutable Cassandra projection of a deployment host's health sample.
    /// </summary>
    public sealed class ComputeResourceMetricActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }

        public string HostId { get; set; }
        public string StartTimeStamp { get; set; }
        public string EndTimeStamp { get; set; }
        public double ElapsedMS { get; set; }
        public string Status { get; set; }
        public string StatusDetails { get; set; }
        public string CpuPercent { get; set; }
        public string MemoryPercent { get; set; }
    }
}
