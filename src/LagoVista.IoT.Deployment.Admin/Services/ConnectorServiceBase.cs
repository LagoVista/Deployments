using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public abstract class ConnectorServiceBase
    {
        public const string CLIENT_VERSION = "2017-04-26";
        private readonly IAdminLogger _logger;
        private readonly IDeploymentHostRepo _deploymentHostManager;

        public ConnectorServiceBase(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger)
        {
            _logger = logger;
            _deploymentHostManager = deploymentHostRepo;
        }

        private HttpClient GetHttpClient(DeploymentHost host, EntityHeader org, EntityHeader usr, string method, string path)
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

        protected async Task<InvokeResult<T>> GetAsync<T>(string path, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                var uri = new Uri($"{host.AdminAPIUri}{path}");

                try
                {
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult<T>>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService_GetAsync<T>] - Non Success Code", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("uri", uri.ToString()),
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService_GetAsync<T>] - Exception", ex.Message,
                          new KeyValuePair<string, string>("uri", uri.ToString()),
                          new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation<T>(ex.Message);
                }
            }
        }

        protected async Task<InvokeResult<T>> GetAsync<T>(string path, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await GetAsync<T>(path, host, org, user);
        }

        protected async Task<ListResponse<T>> GetListResponseAsync<T>(string path, DeploymentHost host, EntityHeader org, EntityHeader user, ListRequest listRequest = null) where T : class
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                if (listRequest != null)
                {
                    if (!string.IsNullOrEmpty(listRequest.NextRowKey))
                    {
                        request.DefaultRequestHeaders.Add("x-nextrowkey", listRequest.NextRowKey);
                    }

                    if (!string.IsNullOrEmpty(listRequest.NextPartitionKey))
                    {
                        request.DefaultRequestHeaders.Add("x-nextpartitionkey", listRequest.NextPartitionKey);
                    }

                    if (!string.IsNullOrEmpty(listRequest.StartDate))
                    {
                        request.DefaultRequestHeaders.Add("x-filter-startdate", listRequest.StartDate);
                    }

                    if (!string.IsNullOrEmpty(listRequest.EndDate))
                    {
                        request.DefaultRequestHeaders.Add("x-filter-enddate", listRequest.EndDate);
                    }

                    request.DefaultRequestHeaders.Add("x-pagesize", listRequest.PageSize.ToString());
                    request.DefaultRequestHeaders.Add("x-pageindex", listRequest.PageIndex.ToString());
                }

                var uri = new Uri($"{host.AdminAPIUri}{path}");
                try
                {
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ListResponse<T>>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__GetListResponseAsync<T>] - Non Success Code", $"{response.StatusCode} - {response.ReasonPhrase}",
                           new KeyValuePair<string, string>("uri", uri.ToString()),
                             new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));


                        var errResponse = new ListResponse<T>();
                        errResponse.Errors.Add(DeploymentErrorCodes.ErrorCommunicatingWithhost.ToErrorMessage($"{response.StatusCode} - {response.ReasonPhrase}"));
                        return errResponse;
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService___GetListResponseAsync<T>] - Exception", ex.Message,
                          new KeyValuePair<string, string>("uri", uri.ToString()),
                          new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    var errResponse = new ListResponse<T>();
                    errResponse.Errors.Add(DeploymentErrorCodes.ErrorCommunicatingWithhost.ToErrorMessage(ex.Message));
                    return errResponse;
                }
            }
        }

        protected async Task<ListResponse<T>> GetListResponseAsync<T>(string path, string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest = null) where T : class
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await GetListResponseAsync<T>(path, host, org, user);
        }

        protected async Task<InvokeResult> GetAsync(string path, DeploymentHost host, EntityHeader org, EntityHeader user, ListRequest listRequest = null)
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                var uri = new Uri($"{host.AdminAPIUri}{path}");

                try
                {
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__GetAsync] - Non Success", $"{response.StatusCode} - {response.ReasonPhrase}",
                          new KeyValuePair<string, string>("uri", uri.ToString()),
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__GetAsync] - Exception", ex.Message,
                          new KeyValuePair<string, string>("uri", uri.ToString()),
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }
        }

        protected async Task<InvokeResult> GetAsync(string path, string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest = null)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await GetAsync(path, host, org, user);
        }

        protected async Task<InvokeResult> DeleteAsync(string path, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "DELETE", path))
            {
                var uri = new Uri($"{host.AdminAPIUri}{path}");

                try
                {
                    var response = await request.DeleteAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return InvokeResult.Success;
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__DeleteAsync] - Non Success", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("uri", uri.ToString()),
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__DeleteAsync] - Exception", ex.Message,
                        new KeyValuePair<string, string>("uri", uri.ToString()),
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }
        }

        protected async Task<InvokeResult> DeleteAsync(string path, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await DeleteAsync(path, host, org, user);
        }

        protected async Task<InvokeResult> PostAsync<TPost>(string path, TPost post, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "POST", path))
            {
                var uri = new Uri($"{host.AdminAPIUri}{path}");

                try
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(post), System.Text.Encoding.UTF8, "application/json");
                    var response = await request.PostAsync(uri, jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__PostAsync<TPost>] - Non Success", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("uri", uri.ToString()),
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__PostAsync<TPost>] - Exception", ex.Message,
                        new KeyValuePair<string, string>("uri", uri.ToString()),
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }
        }

        protected async Task<InvokeResult> PostAsync<TPost>(string path, TPost post, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await PostAsync<TPost>(path, post, host, org, user);
        }

        protected async Task<InvokeResult> PutAsync<TPut>(string path, TPut post, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "PUT", path))
            {
                var uri = new Uri($"{host.AdminAPIUri}{path}");

                try
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(post), System.Text.Encoding.UTF8, "application/json");
                    var response = await request.PutAsync(uri, jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__PutAsync<TPut>] - Non Success", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("uri", uri.ToString()),
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "[DeploymentConnectorService__PutAsync<TPut>] - Exception", ex.Message,
                        new KeyValuePair<string, string>("uri", uri.ToString()),
                        new KeyValuePair<string, string>("hostid", host.Id),
                        new KeyValuePair<string, string>("path", path),
                        new KeyValuePair<string, string>("orgid", org.Id),
                        new KeyValuePair<string, string>("userid", user.Id));

                    return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation(ex.Message);
                }
            }
        }

        protected async Task<InvokeResult> PutAsync<TPost>(string path, TPost post, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await PutAsync<TPost>(path, post, host, org, user);
        }
    }
}
