using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ITagReplacementService
    {
        Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device, OrgLocation location);
    }
}
