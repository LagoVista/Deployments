using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceNotificationRecipientRepo : TableStorageBase<DeviceNotificationRecipientTSEntity>, IDeviceNotificationRecipientRepo
    {

        public DeviceNotificationRecipientRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {
        }

        public async Task AddNotificationRecipientAsync(DeviceNotificationRecipient recipient)
        {
            await InsertAsync(DeviceNotificationRecipientTSEntity.FromEntity(recipient));
        }

        public async Task<ListResponse<Models.DeviceNotificationRecipient>> GetRecipientsForDeviceAsync(ListRequest listRequest, string deviceRepoId, string deviceId)
        {
            var result = await GetByFilterAsync(FilterOptions.Create(nameof(DeviceNotificationRecipientTSEntity.DeviceRepoId), FilterOptions.Operators.Equals, deviceRepoId),
                FilterOptions.Create(nameof(DeviceNotificationRecipientTSEntity.DeviceUniqueId), FilterOptions.Operators.Equals, deviceId));
            return ListResponse<Models.DeviceNotificationRecipient>.Create(listRequest, result.Select(res => res.ToEntity()));
        }

        public async Task<ListResponse<Models.DeviceNotificationRecipient>> GetRecipientsForDeviceAndNotificationKeyAsync(ListRequest listRequest, string deviceRepoId, string deviceId, string notitificationKey)
        {
            var result = await GetByFilterAsync(FilterOptions.Create(nameof(DeviceNotificationRecipientTSEntity.DeviceRepoId), FilterOptions.Operators.Equals, deviceRepoId),
                            FilterOptions.Create(nameof(DeviceNotificationRecipientTSEntity.DeviceUniqueId), FilterOptions.Operators.Equals, deviceId),
                            FilterOptions.Create(nameof(DeviceNotificationRecipientTSEntity.NotificationKey), FilterOptions.Operators.Equals, notitificationKey));
            return ListResponse<Models.DeviceNotificationRecipient>.Create(listRequest, result.Select(rec => rec.ToEntity()));
        }

        public async Task DeleteNotificationRecipientAsync(string deviceRepoId, string notificationRecipientId)
        {
            await base.RemoveAsync(deviceRepoId, notificationRecipientId);
        }
    }

    public class DeviceNotificationRecipientTSEntity : TableStorageEntity
    {
        public static DeviceNotificationRecipientTSEntity FromEntity(DeviceNotificationRecipient recipient)
        {
            return new DeviceNotificationRecipientTSEntity()
            {
                RowKey = recipient.Id,
                PartitionKey = recipient.DeviceRepoId.ToString(),
                DeviceUniqueId = recipient.DeviceUniqueId,
                DeviceRepoId = recipient.DeviceRepoId,
                Email = recipient.Email,
                FirstName = recipient.FirstName,
                LastName = recipient.LastName,
                Phone = recipient.Phone,
                SendEmail = recipient.SendEmail,
                SendSms = recipient.SendSms,
                NotificationId = recipient.NotificationId,
                NotificationKey = recipient.NotificationKey,
                OrgId = recipient.Org.Id,
                OrgName = recipient.Org.Text,
                CreatedByUser = recipient.User.Text,
                CreatedByUserId = recipient.User.Id,
                CreationDate = UtcTimestamp.Now,
            };
        }

        public DeviceNotificationRecipient ToEntity()
        {
            return new DeviceNotificationRecipient()
            {
                NotificationKey = NotificationKey,
                Org = EntityHeader.Create(OrgId, OrgName),
                User = EntityHeader.Create(CreatedByUserId, CreatedByUser),
                CreationDate = UtcTimestamp.Parse(this.CreationDate),
                NotificationId = NotificationId,
                SendSms = SendSms,
                SendEmail = SendEmail,
                Phone = Phone,
                LastName = LastName,
                Email = Email,
                DeviceUniqueId = DeviceUniqueId,
                DeviceRepoId = DeviceRepoId,
                Id = RowKey,
                FirstName = FirstName,
            };
        }

        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; }

        public string CreationDate { get; set; }
        public NormalizedId32 DeviceRepoId { get; set; }
        public NormalizedId32 DeviceUniqueId { get; set; }
        public string NotificationKey { get; set; }

        public string NotificationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
    }
}
