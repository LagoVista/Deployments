// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c95655c8faccc29d9dc02a347967a3214fb1b95cfaff55ac1b81b2c624518781
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceStatusRepo : TableStorageBase<DeviceStatusDTO>, IDeviceStatusRepo
    {
        private readonly IDeviceRepositoryRepo _deviceRepoRepo;

        public DeviceStatusRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger,
                    IDeviceRepositoryRepo deviceRepoRepo) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {
            _deviceRepoRepo = deviceRepoRepo;
        }

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusForInstanceAsync(ListRequest listRequest, DeploymentInstance instance)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceCurrentStatusStorageName());
            var result =  await GetByFilterAsync();
            return ListResponse<DeviceStatus>.Create(listRequest, result.Select(ds => ds.ToDeviceStatus()));        
        }

        public async Task AddDeviceStatusAsync(DeploymentInstance instance, DeviceStatus status)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceCurrentStatusStorageName());
            await InsertAsync(new DeviceStatusDTO(status, status.DeviceUniqueId));
        }

        public async Task UpdateDeviceStatusAsync(DeploymentInstance instance, DeviceStatus status)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceCurrentStatusStorageName());
            await UpdateAsync(new DeviceStatusDTO(status, status.DeviceUniqueId));
        }

        public async Task<DeviceStatus> GetDeviceStatusAsync(DeploymentInstance instance, string deviceUniqueId)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceCurrentStatusStorageName());
            var dto = await GetAsync(deviceUniqueId);
            return dto.ToDeviceStatus();
        }

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryForDeviceAsync(ListRequest listRequest, DeploymentInstance instance, string deviceUniqueId)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceStatusHistoryStorageName());
            var result = await GetPagedResultsAsync(deviceUniqueId, listRequest);
            return ListResponse<DeviceStatus>.Create(listRequest, result.Model.Select(ds => ds.ToDeviceStatus()));
        }

        public async Task<ListResponse<DeviceStatus>> GetTimedOutDeviceStatusForInstanceAsync(ListRequest listRequest, DeploymentInstance instance)
        {
            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            SetTableName(repo.GetDeviceCurrentStatusStorageName());
            var result = await GetByFilterAsync(FilterOptions.Create(nameof(DeviceStatusDTO.CurrentStatus ), FilterOptions.Operators.Equals, DeviceStatus.DeviceStatus_TimeedOut));
            return ListResponse<DeviceStatus>.Create(listRequest, result.Select(ds => ds.ToDeviceStatus()));
        }
    }

    public class DeviceStatusDTO : TableStorageEntity
    {
        public DeviceStatusDTO()
        {

        }

        public DeviceStatusDTO(DeviceStatus status, string rowKey)
        {
            RowKey = rowKey;

            this.DeviceUniqueId = status.DeviceUniqueId;
            PartitionKey = status.DeviceUniqueId;
            this.SilenceAlarm = status.SilenceAlarm;
            this.CurrentStatus = status.CurrentStatus;
            this.PreviousStatus = status.PreviousStatus;
            this.Details = status.Details;
            this.DeviceId = status.DeviceId;
            this.LastNotified = status.LastNotified;
            this.LastContact = status.LastContact;
            this.WatchdogCheckPoint = status.WatchdogCheckPoint;
        }

        public string Id { get; set; }
        public string Timestamp { get; set; }
        public string DeviceId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string Details { get; set; }
        public string CurrentStatus { get; set; }
        public string PreviousStatus { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string WatchdogCheckPoint { get; set; }
        public bool SilenceAlarm { get; set; }

        public DeviceStatus ToDeviceStatus()
        {
            return new DeviceStatus()
            {
                DeviceId = this.DeviceId,
                DeviceUniqueId = this.DeviceUniqueId,
                CurrentStatus = this.CurrentStatus,
                PreviousStatus = this.PreviousStatus,
                Details = this.Details,
                LastNotified = this.LastNotified,
                WatchdogCheckPoint = this.WatchdogCheckPoint,
                LastContact = this.LastContact,
                Timestamp = this.Timestamp,
                SilenceAlarm = this.SilenceAlarm,
            };
        }
    }
}
