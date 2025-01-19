using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRaisedNotificationHistoryRepo
    {
        Task<ListResponse<RaisedNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest);
        Task<ListResponse<RaisedNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest);

        Task AddHistoryAsync(RaisedNotificationHistory history);

        Task<RaisedNotificationHistory> GetRaisedNotificationHistoryAsync(string rowKey, string paritionKey);
    }
}
