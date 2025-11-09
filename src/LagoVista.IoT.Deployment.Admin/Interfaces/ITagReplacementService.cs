// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f17eba4d030df38de0b57a86e029256710924a01a7b9f6e87f596c68ee28cfc3
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ITagReplacementService
    {
        Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device, OrgLocation location);
    }

    public interface ICustomerTagReplacementService
    {
        Task<string> ReplaceTagsAsync(string template, bool isHtmlContent, Device device);
    }
}
