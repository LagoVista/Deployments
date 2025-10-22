// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ab5f46be3c1a474a06ae79911bd8d790aec83debd7c8233cd1373005304444c6
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentActionEventHubSettings
    {
        IConnectionSettings DeploymentActivityEventHubConnection { get; set; }
    }
}
