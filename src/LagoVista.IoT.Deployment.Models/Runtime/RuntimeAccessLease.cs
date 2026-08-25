using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models.Runtime
{
    /// <summary>
    /// Short-lived capability lease used by a remote runtime to call the Runtime Data Service.
    /// The token is opaque to the runtime; cluster infrastructure credentials are never returned.
    /// </summary>
    public class RuntimeAccessLease
    {
        public string Token { get; set; }
        public string Endpoint { get; set; }
        public string TokenType { get; set; } = "RuntimeLease";
        public string Version { get; set; } = "1";
        public DateTime IssuedUtc { get; set; }
        public DateTime ValidThroughUtc { get; set; }
        public List<string> Capabilities { get; set; } = new List<string>();
    }

    public static class RuntimeDataCapabilities
    {
        public const string DeviceRead = "device.read";
        public const string DeviceWrite = "device.write";
        public const string DeviceEventsWrite = "device-events.write";
        public const string UsageWrite = "usage.write";
        public const string TransactionsWrite = "transactions.write";
        public const string NotificationsPublish = "notifications.publish";

        public static readonly string[] All =
        {
            DeviceRead,
            DeviceWrite,
            DeviceEventsWrite,
            UsageWrite,
            TransactionsWrite,
            NotificationsPublish
        };
    }
}
