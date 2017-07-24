using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentActionEventHubSettings
    {
        IConnectionSettings DeploymentActivityEventHubConnection { get; set; }

        string DeploymentActivityHubName { get; set; }
    }
}
