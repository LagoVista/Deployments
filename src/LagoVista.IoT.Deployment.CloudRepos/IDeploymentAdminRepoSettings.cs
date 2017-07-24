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
