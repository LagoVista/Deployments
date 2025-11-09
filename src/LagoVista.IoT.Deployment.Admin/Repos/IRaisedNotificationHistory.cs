// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 60e022f3cb4e5dc64eb43bf4cee838b29f1034bad63c805d9b9fc92cb499fa14
// IndexVersion: 2
// --- END CODE INDEX META ---
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
