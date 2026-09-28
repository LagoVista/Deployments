using System;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RuntimePemStorageRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string DeviceId { get; set; }
        public string MessageId { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public string ErrorReason { get; set; }
        public string MessageType { get; set; }
        public string CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string Json { get; set; }
        public string TextPayload { get; set; }
        public string Values { get; set; }
        public string OutgoingMessages { get; set; }
        public string ResponseMessage { get; set; }
        public string Log { get; set; }
        public string Instructions { get; set; }
        public string Device { get; set; }
        public string RuntimeVersion { get; set; }
        public string SolutionVersion { get; set; }
        public bool IsFailure { get; set; }
    }
}
