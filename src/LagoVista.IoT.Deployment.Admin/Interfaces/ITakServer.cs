// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 64547900292477bddfa2dea45805730bb86b6b27594b01a1590a6cf8620c41ba
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        Task<InvokeResult> ConnectAsync(byte[] customCert);
        Task<InvokeResult> ListenAsync(CancellationToken cancellationToken = default);
        Task<InvokeResult> DisconnectAsync();
        Task<InvokeResult> SendAsync(Message message, CancellationToken cancellationToken = default);
        Task<InvokeResult> SendAsync(string rawXml, CancellationToken cancellationToken = default);
    }
}
