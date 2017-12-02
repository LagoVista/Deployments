using LagoVista.Core.Models;
using Newtonsoft.Json;
using System;
using LagoVista.Core;
using System.Diagnostics;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class UsageMetrics : TableStorageEntity
    {
        public UsageMetrics(String hostId, String instanceId, string pipelineModuleId)
        {
            if (!String.IsNullOrEmpty(PipelineModuleId))
            {
                PartitionKey = PipelineModuleId;
            }
            else if (!String.IsNullOrEmpty(InstanceId))
            {
                PartitionKey = InstanceId;
            }
            else if (!String.IsNullOrEmpty(HostId))
            {
                PartitionKey = HostId;
            }

            HostId = HostId;
            InstanceId = instanceId;
            PipelineModuleId = pipelineModuleId;
        }

        public UsageMetrics()
        {

        }

        [JsonProperty("startTimeStamp")]
        public String StartTimeStamp { get; set; }
        [JsonProperty("endTimeStamp")]
        public String EndTimeStamp { get; set; }
        [JsonProperty("elapsedMS")]
        public double ElapsedMS { get; set; }
        [JsonProperty("messagesPerSecond")]
        public double MessagesPerSecond { get; set; }
        [JsonProperty("averageProcessingMS")]
        public double AverageProcessingMS { get; set; }
        [JsonProperty("version")]
        public String Version { get; set; }
        [JsonProperty("instanceId")]
        public String InstanceId { get; set; }

        [JsonProperty("hostId")]
        public String HostId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("pipelineModuleId")]
        public String PipelineModuleId { get; set; }
        [JsonProperty("messagesProcessed")]
        public int MessagesProcessed { get; set; }
        [JsonProperty("deadLetterCount")]
        public int DeadLetterCount { get; set; }
        [JsonProperty("bytesProccessed")]
        public long BytesProcessed { get; set; }
        [JsonProperty("errorCount")]
        public int ErrorCount { get; set; }
        [JsonProperty("warningCount")]
        public int WarningCount { get; set; }
        [JsonProperty("activeCount")]
        public int ActiveCount { get; set; }
        [JsonProperty("processingMS")]
        public double ProcessingMS { get; set; }

        public void Concat(UsageMetrics metric)
        {
            Debug.Assert(metric == null, "Must provide a value for metric to be clone");

            MessagesProcessed += metric.MessagesProcessed;
            ErrorCount += metric.ErrorCount;
            BytesProcessed += metric.BytesProcessed;
            WarningCount += metric.WarningCount;
            ProcessingMS += metric.ProcessingMS;
            DeadLetterCount += metric.DeadLetterCount;
            ActiveCount += metric.ActiveCount;
        }

        public UsageMetrics Clone()
        {
            var clonedMetric = new UsageMetrics()
            {
                Version = Version,
                HostId = HostId,
                InstanceId = InstanceId,
                PipelineModuleId = PipelineModuleId,

                MessagesProcessed = MessagesProcessed,
                ErrorCount = ErrorCount,
                BytesProcessed = BytesProcessed,
                WarningCount = WarningCount,
                ProcessingMS = ProcessingMS,
                DeadLetterCount = DeadLetterCount,
                ActiveCount = ActiveCount,
            };

            return clonedMetric;
        }

        public void Calculate()
        {
            ElapsedMS = Math.Round((EndTimeStamp.ToDateTime() - StartTimeStamp.ToDateTime()).TotalMilliseconds, 3);

            if (ElapsedMS > 1)
            {
                MessagesPerSecond = (MessagesProcessed / ElapsedMS) * 1000.0;
            }

            if (MessagesProcessed > 0)
            {
                AverageProcessingMS = Math.Round(ProcessingMS / MessagesProcessed, 3);
            }
        }


        public void Reset(String previousEndTime = null)
        {
            StartTimeStamp = previousEndTime == null ? DateTime.UtcNow.ToJSONString() : previousEndTime;
            EndTimeStamp = String.Empty;

            ElapsedMS = 0.0;

            MessagesProcessed = 0;
            BytesProcessed = 0;
            ErrorCount = 0;
            WarningCount = 0;
            ProcessingMS = 0;
            DeadLetterCount = 0;

            /* Don't reset ActiveCount, this is carried between metrics and is always live */
            //ActiveCount = NA;
        }
    }
}
