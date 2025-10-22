// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2053f653e317b7ab811df7cd910e7f638a47f2434e84e756334428a127d78dac
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class RESTSender : IRestSender
    {
        private readonly IAdminLogger _admminLogger;
        private readonly ITagReplacementService _tagReplacer;
        private readonly ISecureStorage _secureStorage;

        public RESTSender(ISecureStorage secureStorage, IAdminLogger adminLogger, ITagReplacementService tagReplacer)
        {
            _admminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        public async Task<InvokeResult> SendAsync(Rest rest, Device device, OrgLocation location, EntityHeader org, EntityHeader user)
        {

            var body = !String.IsNullOrEmpty(rest.Payload) ? await _tagReplacer.ReplaceTagsAsync(rest.Payload, false, device, location) : string.Empty; 

            using (var client = new HttpClient())
            {
                if (!rest.Anonymous)
                {
                    var pwdResponse = await _secureStorage.GetSecretAsync(org, rest.BasicAuthPasswordSecretId, user);
                    if (!pwdResponse.Successful) return pwdResponse.ToInvokeResult();

                    string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(rest.BasicAuthUserName + ":" + pwdResponse));

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                }

                HttpResponseMessage response;

                StringContent content = new StringContent(body, System.Text.Encoding.UTF8, rest.ContentType);

                switch(rest.Method.Value)
                {
                    case RestMethods.GET: response = await client.GetAsync(rest.Address); break;
                    case RestMethods.POST: response = await client.PostAsync(rest.Address, content); break;
                    case RestMethods.PUT: response = await client.PutAsync(rest.Address, content); break;
                    case RestMethods.PATCH: response = await client.PatchAsync(rest.Address, content); break;
                    case RestMethods.DELETE: response = await client.DeleteAsync(rest.Address); break;
                    default: return InvokeResult.FromError($"Unsupported HTTP Verb: {rest.Method.Text}.");
                }

                if(!response.IsSuccessStatusCode)
                {
                    return InvokeResult.FromError($"Could not send REST notification: {response.StatusCode}");
                }

                return InvokeResult.Success;

            }
        }
    }
}
