using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Logging.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public enum RuntimeLogResourceType
    {
        Host,
        Instance,
        Pem,
        Device,
        DeviceType,
        PipelineModule,
        Activity
    }

    public interface IRuntimeLogRepo
    {
        Task WriteAsync(string organizationId, string organization, LogRecord record, bool isError);
        Task<ListResponse<LogRecord>> QueryAsync(EntityHeader organization, RuntimeLogResourceType resourceType, string resourceId, bool isError, ListRequest request);
        Task<ListResponse<LogRecord>> QueryErrorsAsync(EntityHeader organization, ListRequest request);
    }
}
