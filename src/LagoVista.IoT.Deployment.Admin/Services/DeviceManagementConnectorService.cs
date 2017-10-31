using System;
using System.Collections.Generic;
using LagoVista.Core;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceManagementConnectorService : IDeviceManagementConnector
    {
        public const string CLIENT_VERSION = "2017-04-26";

        IAdminLogger _logger;
        IDeploymentHostManager _deploymentHostManager;
        public DeviceManagementConnectorService(IDeploymentHostManager deploymentHostManager, IAdminLogger logger)
        {
            _logger = logger;
            _deploymentHostManager = deploymentHostManager;
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

        private async Task<InvokeResult<T>> GetAsync<T>(string path, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetHostForInstanceAsync(instanceId, org, user);

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

        private async Task<InvokeResult> PostAsync<TPost>(string path, TPost post, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetHostForInstanceAsync(instanceId, org, user);

            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
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

        private async Task<InvokeResult> PutAsync<TPost>(string path, TPost post, string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostManager.GetHostForInstanceAsync(instanceId, org, user);

            using (var request = GetHttpClient(host, org, user, "GET", path))
            {
                try
                {
                    var uri = new Uri($"{host.AdminAPIUri}{path}");
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

        public Task<InvokeResult> AddDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user)
        {
            var uri = "/api/device";
           return PostAsync<Device>(uri, device, instanceId, org, user);
        }

        public Task DeleteDeviceAsync(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/{id}";
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(string instanceId, Device device, EntityHeader org, EntityHeader user)
        {
        var uri = "/api/device";
            return PutAsync<Device>(uri, device, instanceId, org, user);
        }

        public Task DeleteDeviceByIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/deviceid/{deviceId}";

            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string instanceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/devices";
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string instanceId, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByDeviceIdAsync(string instanceId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/deviceid/{deviceId}";

            throw new NotImplementedException();
        }

        public Task<bool> CheckIfDeviceIdInUse(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByIdAsync(string instanceId, string id, EntityHeader org, EntityHeader user)
        {
            var uri = $"/api/device/{id}";

            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string instanceId, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {

            var uri = $"/api/devices/{status}";
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string instanceId, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(string instanceId, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }
    }
}
