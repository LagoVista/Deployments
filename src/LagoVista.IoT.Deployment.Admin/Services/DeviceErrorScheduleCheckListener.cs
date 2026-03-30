using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceErrorScheduleCheckListener : IDeviceErrorScheduleCheckListener
    {
        private readonly IDeviceErrorScheduleCheckSettings _errorConnectionSettings;
        private ServiceBusClient _processorClient;
        private ServiceBusProcessor _processor;
        private readonly IAdminLogger _adminLogger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeviceErrorScheduleCheckListener(IServiceScopeFactory scopeFactory, IDeviceErrorScheduleCheckSettings errorConnectionSettings, IAdminLogger adminLogger)
        {
            _errorConnectionSettings = errorConnectionSettings ?? throw new ArgumentNullException(nameof(errorConnectionSettings));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        private async Task CreateQueue(string queueName)
        {
            var connstr = $"Endpoint=sb://{_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.UserName};SharedAccessKey={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccessKey};";

            var client = new ServiceBusAdministrationClient(connstr);
            if (!await client.QueueExistsAsync(queueName))
            {
                await client.CreateQueueAsync(queueName);
            }
        }

        public async Task StartAsync()
        {
            if (_processor != null)
            {
                return;
            }

            var queuePath = _errorConnectionSettings.DeviceErrorScheduleQueueSettings.ResourceName;

            _adminLogger.Trace($"{this.Tag()} Starting Listener: {queuePath}");

            await CreateQueue(queuePath);

            var connstr = $"Endpoint=sb://{_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.UserName};SharedAccessKey={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccessKey};";

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _processorClient = new ServiceBusClient(connstr, clientOptions);

            _processor = _processorClient.CreateProcessor(queuePath);
            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync;

            await _processor.StartProcessingAsync();
            _adminLogger.Trace($"{this.Tag()} Started Listener: {queuePath}");
        }

        private async Task _processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                var json = arg.Message.Body.ToString();

                using var scope = _scopeFactory.CreateScope();

                var errorCheck = JsonConvert.DeserializeObject<DeviceErrorScheduleCheck>(json);
                if (errorCheck?.DeviceException == null)
                {
                    _adminLogger.AddError(this.Tag(), $"Invalid message payload for queue [{arg.EntityPath}]");
                    return;
                }

                errorCheck.DeviceException.FollowUpAttempt++;

                var errorHandler = scope.ServiceProvider.GetRequiredService<IDeviceErrorHandler>();
                await errorHandler.HandleDeviceExceptionAsync(errorCheck.DeviceException, errorCheck.Org, errorCheck.User);
            }
            catch (Exception ex)
            {
                _adminLogger.AddException(this.Tag(), ex);
                throw;
            }
        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _adminLogger.AddError(this.Tag(), arg.Exception.Message);
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (_processor != null)
            {
                await _processor.CloseAsync();
                await _processor.DisposeAsync();
                _processor = null;
            }

            if (_processorClient != null)
            {
                await _processorClient.DisposeAsync();
                _processorClient = null;
            }
        }
    }
}