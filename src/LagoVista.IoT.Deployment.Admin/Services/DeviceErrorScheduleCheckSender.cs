using Azure.Messaging.ServiceBus;
using LagoVista.Core;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DeviceErrorScheduleCheckSender : IDeviceErrorScheduleCheckSender
    {
        IDeviceErrorScheduleCheckSettings _errorConnectionSettings;
        private ServiceBusClient _senderClient;
        private ServiceBusSender _sender;
        private IAdminLogger _adminLogger;
        private string _transmitterConnectionSettings;

        public DeviceErrorScheduleCheckSender(IDeviceErrorScheduleCheckSettings errorConnectionSettings, IAdminLogger adminLogger)
        {
            _errorConnectionSettings = errorConnectionSettings ?? throw new ArgumentNullException(nameof(errorConnectionSettings));
            _adminLogger = adminLogger;
            _transmitterConnectionSettings = $"Endpoint=sb://{_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccountId}.servicebus.windows.net/;SharedAccessKeyName={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.UserName};SharedAccessKey={_errorConnectionSettings.DeviceErrorScheduleQueueSettings.AccessKey};";

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

        public async Task<InvokeResult> ScheduleAsync(DeviceErrorScheduleCheck errorCheck)
        {
            var entityPath = "deviceerrorescalationqueue";
            await CreateQueue(entityPath);

            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            _senderClient = new ServiceBusClient(_transmitterConnectionSettings, clientOptions);
            _sender = _senderClient.CreateSender(entityPath);

            var json = JsonConvert.SerializeObject(errorCheck);
            var jsonBuffer = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
            try
            {
                    var sbMsg = new ServiceBusMessage(json);
                    sbMsg.ContentType = errorCheck.GetType().FullName;
                    sbMsg.Subject = errorCheck.DeviceException.ErrorCode;
                    //        sbMsg.CorrelationId = message.CorrelationId;
                    //        sbMsg.Subject = message.DestinationPath;
                    //        sbMsg.MessageId = Guid.NewGuid().ToId();
                    //        sbMsg.
                    await _sender.ScheduleMessageAsync(sbMsg, errorCheck.DueTimeStamp.ToDateTime());
                    //        return InvokeResult.Success;
             
            }
            catch (Exception ex)
            {
                _adminLogger.AddException("[ServiceBusProxyClient__CustomTransmitMessageAsync]", ex, _transmitterConnectionSettings.ToKVP("txconnstr"), entityPath.ToKVP("entityPath"));
                throw;
            }
            finally
            {
                await _sender.DisposeAsync();
                await _senderClient.DisposeAsync();
            }

            return InvokeResult.Success;
        }
    }
}
