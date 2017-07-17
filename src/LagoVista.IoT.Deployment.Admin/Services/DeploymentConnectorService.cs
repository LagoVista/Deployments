using System;
using System.Collections.Generic;
using LagoVista.Core;
using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.PlatformSupport;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeploymentConnectorService : IDeploymentConnectorService
    {
        public const string CLIENT_VERSION = "2017-04-26";

        IAdminLogger _logger;
        public DeploymentConnectorService(IAdminLogger logger)
        {
            _logger = logger;
        }

        private HttpClient GetHttpClient(DeploymentHost host, EntityHeader org, EntityHeader usr, String method, String path)
        {
            var request = new HttpClient();
            var requestId = Guid.NewGuid().ToId();
            var requestDate = DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.DefaultRequestHeaders.Add("x-nuviot-client-request-id", requestId);
            request.DefaultRequestHeaders.Add("x-nuviot-orgid", org.Id);
            request.DefaultRequestHeaders.Add("x-nuviot-org", org.Text);
            request.DefaultRequestHeaders.Add("x-nuviot-userid", usr.Id);
            request.DefaultRequestHeaders.Add("x-nuviot-user", usr.Text);
            request.DefaultRequestHeaders.Add("x-nuviot-date", requestDate);
            request.DefaultRequestHeaders.Add("x-nuviot-version", CLIENT_VERSION);
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKey", RequestSigningService.GetAuthHeaderValue(host, requestId, org.Id, usr.Id, method, path, requestDate));

            return request;
        }

        private async Task<InvokeResult> Execute(string path, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                try
                {
                    var uriStr = $"{host.AdminAPIUri}{path}";
                    var uri = new Uri(uriStr);
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }
        }

        private async Task<InvokeResult<T>> Execute<T>(string path, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult<T>>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>(ex.Message);
                }
            }
        }

        public Task<InvokeResult<string>> GetRemoteMonitoringUriAsync(DeploymentHost host, string channel, string id, string verbosity, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/websocket/{channel}/{id}/{verbosity}";
            return Execute<string>(path, host, org, user);
        }


        public async Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/instances";
            var callResponse = await Execute<ListResponse<InstanceRuntimeSummary>>(path, host, org, user);
            if(callResponse.Successful)
            {
                if (callResponse.Result == null)
                {
                    var failedResponse = ListResponse<InstanceRuntimeSummary>.Create(null);
                    failedResponse.Errors.Add(new ErrorMessage("Null Response From Server."));
                    return failedResponse;
                }
                else
                {
                    return callResponse.Result;
                }
            }
            else
            {
                var failedResponse =  ListResponse<InstanceRuntimeSummary>.Create(null);
                failedResponse.Concat(callResponse);
                return failedResponse;
            }
        }

        public Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/{instanceId}";
            return Execute<InstanceRuntimeDetails>(path, host, org, user);
        }

        public Task<InvokeResult> DeployAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/deploy/{instanceId}";
            return Execute(path, host, org, user);
        }

        public Task<InvokeResult> PauseAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/pause/{instanceId}";
            return Execute(path, host, org, user);
        }

        public Task<InvokeResult> RemoveAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/remove/{instanceId}";
            return Execute(path, host, org, user);
        }

        public Task<InvokeResult> StartAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/start/{instanceId}";
            return Execute(path, host, org, user);
        }

        public Task<InvokeResult> StopAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/stop/{instanceId}";
            return Execute(path, host, org, user);
        }

        public Task<InvokeResult> UpdateAsync(DeploymentHost host, string instanceId, EntityHeader org, EntityHeader user)
        {
            var path = $"/api/instancemanager/update/{instanceId}";
            return Execute(path, host, org, user);
        }
    }
}
