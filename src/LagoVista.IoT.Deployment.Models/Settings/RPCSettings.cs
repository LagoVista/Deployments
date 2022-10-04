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
