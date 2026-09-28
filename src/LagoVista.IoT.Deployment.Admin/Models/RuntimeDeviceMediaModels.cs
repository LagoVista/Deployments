namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RuntimeDeviceMediaRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string PemId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Title { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsAttached { get; set; }
    }

    public class RuntimeDeviceMediaAttachRequest
    {
        public string Title { get; set; }
        public string UniqueDeviceId { get; set; }
        public string DeviceId { get; set; }
    }

    public class RuntimeDeviceMediaContent
    {
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
