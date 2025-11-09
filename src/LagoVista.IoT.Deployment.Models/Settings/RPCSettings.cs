// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 33729291032f44d9513855d45400a607d041eea6b54682d0906b17b033d1c0e7
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models.Settings
{
    public class RPCSettings
    {
        public ConnectionSettings Receiver { get; set; }
        public ConnectionSettings Transmitter { get; set; }

        /// <summary>
        /// TO be used with Modern Azure SB library. 
        /// </summary>
        public ConnectionSettings ReceiverV2 { get; set; }

        /// <summary>
        /// TO be used with Modern Azure SB library. 
        /// </summary>
        public ConnectionSettings TransmitterV2 { get; set; }
    }
}
