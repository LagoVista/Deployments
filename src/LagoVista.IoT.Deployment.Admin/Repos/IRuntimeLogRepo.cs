using LagoVista.IoT.Logging.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRuntimeLogRepo
    {
        Task WriteAsync(string organizationId, string organization, LogRecord record, bool isError);
    }
}
