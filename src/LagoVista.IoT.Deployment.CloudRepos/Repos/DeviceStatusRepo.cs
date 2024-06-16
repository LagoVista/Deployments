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

        public async Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryForDevceAsync(ListRequest listRequest, DeviceRepository repo, string deviceUniqueId)
        {
        
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

        public string Id { get; set; }
        public DateTime Datestamp { get; set; }
        public string DeviceId { get; set; }
        public string DeviceUnqiueId { get; set; }
        public string Details { get; set; }
        public string CurrentStatus { get; set; }
        public string PreviousStatus { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string WatchdogCheckPoint { get; set; }

        public DeviceStatus ToDeviceStatus()
        {
            return new DeviceStatus()
            {
                DeviceId = this.DeviceId,
                DeviceUniqueId = this.DeviceUnqiueId,
                CurrentStatus = this.CurrentStatus,
                PreviouStatus = this.PreviousStatus,
                Details = this.Details,
                LastNotified = this.LastNotified,
                WatchdogCheckPoint = this.WatchdogCheckPoint,
                LastContact = this.LastContact,
                Timestamp = this.Datestamp.ToJSONString(),
            };
        }
    }
}
