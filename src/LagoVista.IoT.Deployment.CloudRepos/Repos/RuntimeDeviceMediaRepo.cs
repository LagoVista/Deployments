using LagoVista.CloudStorage.Interfaces;
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class RuntimeDeviceMediaOperationalRecord : IOperationalDataRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
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

    public sealed class RuntimeDeviceMediaRepo : IRuntimeDeviceMediaRepo
    {
        private const string TemporaryDeviceId = "UNKOWNDEVICEID";
        private readonly IOperationalDataStore<RuntimeDeviceMediaOperationalRecord> _store;
        private readonly ICloudFileStorageClient _fileStorage;

        public RuntimeDeviceMediaRepo(IOperationalDataStore<RuntimeDeviceMediaOperationalRecord> store, ICloudFileStorageClient fileStorage)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        internal static void ConfigureStorage(StorageDefinition<RuntimeDeviceMediaOperationalRecord> definition)
        {
            definition.Index(record => record.DeviceUniqueId);
            definition.Index(record => record.DeviceId);
            definition.Index(record => record.PemId);
        }

        public async Task<InvokeResult<string>> StoreAsync(DeviceRepository repo, string organizationId, string pemId, byte[] data, string contentType, long length, double? latitude, double? longitude)
        {
            if (repo == null) throw new ArgumentNullException(nameof(repo));
            if (String.IsNullOrWhiteSpace(organizationId)) throw new ArgumentNullException(nameof(organizationId));
            if (String.IsNullOrWhiteSpace(pemId)) return InvokeResult<string>.FromError("PEM id is required.");
            if (data == null) return InvokeResult<string>.FromError("Media data is required.");

            var containerName = repo.GetDeviceMediaStorageName();
            var fileName = GetFileName(pemId, contentType);
            var effectiveLength = length > 0 ? length : data.LongLength;
            if (effectiveLength != data.LongLength)
                return InvokeResult<string>.FromError($"Device media length mismatch. Declared {effectiveLength}, received {data.LongLength}.");

            var fileResult = await _fileStorage.AddFileAsync(containerName, fileName, data, contentType ?? "application/octet-stream").ConfigureAwait(false);
            if (!fileResult.Successful)
                return InvokeResult<string>.FromInvokeResult(fileResult.ToInvokeResult());

            try
            {
                await _store.UpsertAsync(new RuntimeDeviceMediaOperationalRecord
                {
                    Id = pemId,
                    OrganizationId = organizationId,
                    PemId = pemId,
                    DeviceUniqueId = TemporaryDeviceId,
                    DeviceId = "UNKNOWN",
                    Title = "??",
                    TimeStamp = DateTime.UtcNow.ToString("O"),
                    ContentType = contentType ?? "application/octet-stream",
                    ContainerName = containerName,
                    FileName = fileName,
                    Length = effectiveLength,
                    Latitude = latitude,
                    Longitude = longitude,
                    IsAttached = false
                }).ConfigureAwait(false);
            }
            catch
            {
                await _fileStorage.DeleteFileAsync(containerName, fileName).ConfigureAwait(false);
                throw;
            }

            return InvokeResult<string>.Create(pemId);
        }

        public async Task<InvokeResult<string>> AttachAsync(string organizationId, string pemId, string title, string uniqueDeviceId, string deviceId)
        {
            if (String.IsNullOrWhiteSpace(organizationId)) throw new ArgumentNullException(nameof(organizationId));
            if (String.IsNullOrWhiteSpace(pemId)) return InvokeResult<string>.FromError("PEM id is required.");
            if (String.IsNullOrWhiteSpace(uniqueDeviceId)) return InvokeResult<string>.FromError("Unique device id is required.");

            var current = await _store.GetAsync(organizationId, pemId).ConfigureAwait(false);
            if (current == null || current.IsAttached)
                return InvokeResult<string>.FromError($"Could not find temporary device media for PEM [{pemId}].");

            var timestamp = DateTime.TryParse(current.TimeStamp, out var parsed) ? parsed.ToUniversalTime() : DateTime.UtcNow;
            var finalId = $"{DateTime.MaxValue.Ticks - timestamp.Ticks:D19}";

            await _store.UpsertAsync(new RuntimeDeviceMediaOperationalRecord
            {
                Id = finalId,
                OrganizationId = current.OrganizationId,
                PemId = current.PemId,
                DeviceUniqueId = uniqueDeviceId,
                DeviceId = deviceId,
                Title = title,
                TimeStamp = current.TimeStamp,
                ContentType = current.ContentType,
                ContainerName = current.ContainerName,
                FileName = current.FileName,
                Length = current.Length,
                Latitude = current.Latitude,
                Longitude = current.Longitude,
                IsAttached = true
            }).ConfigureAwait(false);

            await _store.DeleteAsync(organizationId, pemId).ConfigureAwait(false);
            return InvokeResult<string>.Create(finalId);
        }

        public async Task<InvokeResult<RuntimeDeviceMediaContent>> GetAsync(string organizationId, string uniqueDeviceId, string mediaItemId)
        {
            if (String.IsNullOrWhiteSpace(organizationId)) throw new ArgumentNullException(nameof(organizationId));
            if (String.IsNullOrWhiteSpace(mediaItemId)) return InvokeResult<RuntimeDeviceMediaContent>.FromError("Media item id is required.");

            var record = await _store.GetAsync(organizationId, mediaItemId).ConfigureAwait(false);
            if (record == null || !record.IsAttached || !String.Equals(record.DeviceUniqueId, uniqueDeviceId, StringComparison.OrdinalIgnoreCase))
                return InvokeResult<RuntimeDeviceMediaContent>.FromError("Device media item was not found.");

            var file = await _fileStorage.GetFileAsync(record.ContainerName, record.FileName).ConfigureAwait(false);
            if (!file.Successful)
                return InvokeResult<RuntimeDeviceMediaContent>.FromInvokeResult(file.ToInvokeResult());

            return InvokeResult<RuntimeDeviceMediaContent>.Create(new RuntimeDeviceMediaContent
            {
                Data = file.Result,
                ContentType = record.ContentType,
                FileName = record.FileName
            });
        }

        private static string GetFileName(string pemId, string contentType)
        {
            var value = (contentType ?? String.Empty).ToLowerInvariant();
            if (value.Contains("gif")) return $"{pemId}.gif";
            if (value.Contains("png")) return $"{pemId}.png";
            if (value.Contains("jpeg")) return $"{pemId}.jpeg";
            if (value.Contains("jpg")) return $"{pemId}.jpg";
            if (value.Contains("3gp")) return $"{pemId}.3gp";
            if (value.Contains("mp3")) return $"{pemId}.mp3";
            if (value.Contains("wav")) return $"{pemId}.wav";
            return $"{pemId}.media";
        }
    }
}
