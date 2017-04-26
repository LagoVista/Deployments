using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core;
using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.PlatformSupport;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Resources;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeploymentConnectorService : IDeploymentConnectorService
    {
        public const string CLIENT_VERSION = "2017-04-26";

        ILogger _logger;
        public DeploymentConnectorService(ILogger logger)
        {
            _logger = logger;
        }

        private HttpClient GetHttpClient(DeploymentHost host, String instanceId, EntityHeader org, EntityHeader usr, String method, String resource)
        {
            var request = new HttpClient();
            var requestId = Guid.NewGuid().ToId();
            var requestDate = DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Add("x-nuviot-client-request-id", Guid.NewGuid().ToId());
            request.DefaultRequestHeaders.Add("x-nuviot-orgid", org.Id);
            request.DefaultRequestHeaders.Add("x-nuviot-org", org.Text);
            request.DefaultRequestHeaders.Add("x-nuviot-userid", usr.Id);
            request.DefaultRequestHeaders.Add("x-nuviot-user", usr.Text);
            request.DefaultRequestHeaders.Add("x-nuviot-date", requestDate);
            request.DefaultRequestHeaders.Add("x-nuviot-version", CLIENT_VERSION);
            request.DefaultRequestHeaders.Authorization = new  AuthenticationHeaderValue("SharedKey", RequestSigningService.GetAuthHeaderValue(host, instanceId, requestId, org.Id, usr.Id, method, resource, requestDate));

            return request;
        }

        private async Task<InvokeResult> Execute(string path, DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, instanceId, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri("{host.AdminAPIUri}{path}");
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("instanceid", instanceId),
                            new KeyValuePair<string, string>("userid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch(Exception ex)
                {
                    _logger.Log(LogLevel.Error, "DeploymentConnectorService", ex.Message,
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("instanceid", instanceId),
                        new KeyValuePair<string, string>("userid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }        
        }

        private async Task<InvokeResult<T>> Execute<T>(string path, DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, instanceId, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri("{host.AdminAPIUri}{path}");
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult<T>>(json);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("instanceid", instanceId),
                            new KeyValuePair<string, string>("userid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "DeploymentConnectorService", ex.Message,
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("instanceid", instanceId),
                        new KeyValuePair<string, string>("userid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>(ex.Message);
                }
            }
        }

        public Task<InvokeResult<Uri>> GetRemoteMonitoringUriAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/deploy/{instanceId}";
            return Execute<Uri>(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> DeployAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/deploy/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> PauseAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/pause/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> RemoveAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/remove/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> StartAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/start/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> StopAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/stop/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }

        public Task<InvokeResult> UpdateAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/update/{instanceId}";
            return Execute(path, host, instanceId, org, user);
        }
    }
}
