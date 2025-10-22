// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 81d00cbfc0b9d166ee61587841852feedf51399f52672c8b8c9cdb222bcd79b4
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IMessageWatchdogService
    {
        Task<ListResponse<WatchdogMessageStatus>> GetWatchdogMessageStatusAsync(ListRequest request);

        Task<ListResponse<WatchdogMessageStatus>> GetTimedOutWatchdogMessageStatusAsync(ListRequest listRequest);
    }
}
