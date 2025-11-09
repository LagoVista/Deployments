// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e4aa6ab00ef1dc58a77b3be31a45bd1614474d784d88f9e047509a8f3c4e442b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.CloudRepos
{
    public interface IDeploymentRepoSettings
    {
        IConnectionSettings DeploymentAdminDocDbStorage { get; set; }
        IConnectionSettings DeploymentAdminTableStorage { get; set; }
        bool ShouldConsolidateCollections { get; }
    }

    public interface IUsageMetricsRepoSettings
    {
        IConnectionSettings UsageMetricsTableStorage { get; set; }
    }

    public interface IDeploymentInstanceRepoSettings
    {
        IConnectionSettings InstanceDocDbStorage { get; set; }
        bool ShouldConsolidateCollections { get; }
    }


    public interface IDeviceConfigurationSettings
    {
        IConnectionSettings DeviceConfigurationtAdminDocDbStorage { get; set; }
        IConnectionSettings DeviceConfigurationAdminTableStorage { get; set; }

        bool ShouldConsolidateCollections { get; }
    }
}
