using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceMessaging.Models.Cot;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ITakClient
    {
        event EventHandler TakServerConnected;
        event EventHandler TakServerDisconnected;
        event EventHandler<Event> MessageReceived;

        Task<InvokeResult> InitAsync(Stream stream);
        Task<InvokeResult> GetIsConnectedAsync();
        Task<InvokeResult> ConnectAsync();
        Task<InvokeResult> ListenAsync(CancellationToken cancellationToken = default);
        Task<InvokeResult> DisconnectAsync();
        Task<InvokeResult> SendAsync(Message message, CancellationToken cancellationToken = default);
        Task<InvokeResult> SendAsync(string rawXml, CancellationToken cancellationToken = default);
    }
}
