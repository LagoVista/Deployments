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
