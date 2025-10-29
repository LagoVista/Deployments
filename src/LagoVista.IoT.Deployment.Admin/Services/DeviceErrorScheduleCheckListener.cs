// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d7efe5bcf6ebdd43d4d810a8c129c6e518d69b4dd89a12e87fb04891e2b36b64
// IndexVersion: 0
// --- END CODE INDEX META ---
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceErrorScheduleCheckListener : IDeviceErrorScheduleCheckListener
    {
        IDeviceErrorHandler _errorHandler;
        IDeviceErrorScheduleCheckSettings _errorConnectionSettings;
        private ServiceBusClient _processorClient;
        private ServiceBusProcessor _processor;
        private readonly IAdminLogger _adminLogger;
 
        public DeviceErrorScheduleCheckListener(IDeviceErrorHandler errorHandler, IDeviceErrorScheduleCheckSettings errorConnectionSettings, IAdminLogger adminLogger)
        {
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _errorConnectionSettings = errorConnectionSettings ?? throw new ArgumentNullException(nameof(errorConnectionSettings));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
        }

        private async Task CreateQueue(string queueName)
        {
            var connstr = $"Endpoint=sb://{_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.UserName};SharedAccessKey={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccessKey};";

            var client = new ServiceBusAdministrationClient(connstr);
            if (!await client.QueueExistsAsync(queueName))
            {
                await client.CreateQueueAsync(queueName);
                await client.CreateSubscriptionAsync(queueName, "errorsubs");
            }
        }

        public async Task StartAsync()
        {
            var queuePath = _errorConnectionSettings.DeviceErrorScheduleQueueSettings.ResourceName;
      
            _adminLogger.Trace($"[DeviceErrorScheduleCheckListener__StartAsync] Starting Listener: {queuePath}");

            await CreateQueue(queuePath);
            var connstr = $"Endpoint=sb://{_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.UserName};SharedAccessKey={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccessKey};";

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _processorClient = new ServiceBusClient(connstr, clientOptions);

            _processor = _processorClient.CreateProcessor(queuePath);
            _processor.ProcessMessageAsync += _processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += _processor_ProcessErrorAsync; ;

            await _processor.StartProcessingAsync();
            _adminLogger.Trace($"[DeviceErrorScheduleCheckListener__StartAsync] Started Listener: {queuePath}");
        }

        private async Task _processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var json = arg.Message.Body.ToString();

            var exception = JsonConvert.DeserializeObject<DeviceErrorScheduleCheck>(json);
            exception.DeviceException.FollowUpAttempt++;
            await _errorHandler.HandleDeviceExceptionAsync(exception.DeviceException, exception.Org, exception.User);
        }

        private Task _processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _adminLogger.AddError("[DeviceErrorScheduleCheckListener___processor_ProcessErrorAsync]", arg.Exception.Message);
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
