// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9ad04e102ba91dd77d4bff48be2d17ab8cab9e3f04e0d929d4a9b4fcc399a22a
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Threading;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class MQTTSender : IMqttSender
    {
        private readonly ISecureStorage _secureStorage;
        private readonly IAdminLogger _admminLogger;
        private readonly ITagReplacementService _tagReplacer;

        public MQTTSender(ISecureStorage secureStorage, IAdminLogger adminLogger, ITagReplacementService tagReplacer)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _tagReplacer = tagReplacer ?? throw new ArgumentNullException(nameof(tagReplacer)); 
            _admminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
        }

        public async Task<InvokeResult> SendAsync(Mqtt mqtt, Device device, OrgLocation location, EntityHeader org, EntityHeader user)
        {
            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();
            var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(mqtt.Address, mqtt.Port);

            if (mqtt.SecureConnection)
            {
                optionsBuilder.WithTlsOptions(
                    o => {
                        o.WithCertificateValidationHandler(
                        // The used public broker sometimes has invalid certificates. This sample accepts all
                        // certificates. This should not be used in live environments.
                            _ => true);
                    });
            }

            var clientId = String.IsNullOrEmpty(mqtt.ClientId) ? mqtt.Key : mqtt.ClientId;
            if (!mqtt.Anonymous)
            {
                var passwordResult = await _secureStorage.GetSecretAsync(org, mqtt.PasswordSecretId, user);
                if (!passwordResult.Successful)
                {
                    return InvokeResult.FromError($"Could not get password from secure storage: {passwordResult.ErrorMessage}");
                }
                
                optionsBuilder.WithCredentials(mqtt.UserName, passwordResult.Result);
            }

            var options = optionsBuilder.Build();
            var response = await mqttClient.ConnectAsync(options, CancellationToken.None);
            if (response.ResultCode != MqttClientConnectResultCode.Success)
            {
                return InvokeResult.FromError($"Could not connect to MQTT server: {mqtt.Address} Error: {response.ReasonString}");
            }

            var sendResponse = await mqttClient.PublishAsync(new MqttApplicationMessage()
            {
                Topic = mqtt.Topic,
                PayloadSegment = System.Text.ASCIIEncoding.ASCII.GetBytes(mqtt.Payload),
            });

            await mqttClient.DisconnectAsync();

            if (!sendResponse.IsSuccess)
                return InvokeResult.FromError($"Fail send MQTT message to {mqtt.Address} {sendResponse.ReasonCode} - {sendResponse.ReasonString}");

            return InvokeResult.Success;
        }
    }
}
