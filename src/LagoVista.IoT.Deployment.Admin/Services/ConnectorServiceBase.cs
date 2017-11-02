using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using LagoVista.Core;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public abstract class ConnectorServiceBase
    {
        public const string CLIENT_VERSION = "2017-04-26";

        IAdminLogger _logger;
        IDeploymentHostRepo _deploymentHostManager;
        public ConnectorServiceBase(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger)
        {
            _logger = logger;
            _deploymentHostManager = deploymentHostRepo;
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

        protected async Task<InvokeResult<T>> GetAsync<T>(string path, DeploymentHost host, EntityHeader org, EntityHeader user)
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

        protected async Task<InvokeResult<T>> GetAsync<T>(string path, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await GetAsync<T>(path, host, org, user);
        }

        protected async Task<ListResponse<T>> GetListResponseAsync<T>(string path, DeploymentHost host, EntityHeader org, EntityHeader user, ListRequest listRequest = null) where T: class
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                if(listRequest != null)
                {
                    if (!String.IsNullOrEmpty(listRequest.NextRowKey)) request.DefaultRequestHeaders.Add("x-nextrowkey", listRequest.NextRowKey);
                    if (!String.IsNullOrEmpty(listRequest.NextPartitionKey)) request.DefaultRequestHeaders.Add("x-nextpartitionkey", listRequest.NextPartitionKey);
                    if (!String.IsNullOrEmpty(listRequest.StartDate)) request.DefaultRequestHeaders.Add("x-filter-startdate", listRequest.StartDate);
                    if(!String.IsNullOrEmpty(listRequest.EndDate)) request.DefaultRequestHeaders.Add("x-filter-enddate", listRequest.EndDate);
                    request.DefaultRequestHeaders.Add("x-pagesize", listRequest.PageSize.ToString());
                    request.DefaultRequestHeaders.Add("x-pageindex", listRequest.PageIndex.ToString());
                }

                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    var response = await request.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ListResponse<T>>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
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
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
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

        protected async Task<ListResponse<T>> GetListResponseAsync<T>(string path, string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest = null) where T: class
        {
            var host = await _deploymentHostManager.GetDeploymentHostForDedicatedInstanceAsync(instanceId);
            return await GetListResponseAsync<T>(path, host, org, user);
        }

        protected async Task<InvokeResult> GetAsync(string path, DeploymentHost host, EntityHeader org, EntityHeader user, ListRequest listRequest = null)
        {
            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    Console.WriteLine(uri);
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
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
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

        protected async Task<InvokeResult> DeleteAsync(string path, DeploymentHost  host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "DELETE", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    var response = await request.DeleteAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return InvokeResult.Success;
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
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
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(post));
                    var response = await request.PostAsync(uri, jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
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

        protected async Task<InvokeResult> PutAsync<TPost>(string path, TPost post, DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            using (var request = GetHttpClient(host, org, user, "PUT", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(post));
                    var response = await request.PutAsync(uri, jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<InvokeResult>(json);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", $"{response.StatusCode} - {response.ReasonPhrase}",
                            new KeyValuePair<string, string>("hostid", host.Id),
                            new KeyValuePair<string, string>("path", path),
                            new KeyValuePair<string, string>("orgid", org.Id),
                            new KeyValuePair<string, string>("userid", user.Id));

                        return DeploymentErrorCodes.ErrorCommunicatingWithhost.ToFailedInvocation($"{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.AddCustomEvent(LogLevel.Error, "DeploymentConnectorService", ex.Message,
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
