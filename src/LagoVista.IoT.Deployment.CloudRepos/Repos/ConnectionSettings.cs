using LagoVista.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentRepoSettings : IDeploymentRepoSettings
    {
        public IConnectionSettings DeploymentAdminDocDbStorage { get; }
        public IConnectionSettings DeploymentAdminTableStorage { get; }

        public DeploymentRepoSettings(IConfiguration configuration)
        {
            DeploymentAdminDocDbStorage = configuration.CreateDefaultDBStorageSettings();
            DeploymentAdminTableStorage = configuration.CreateDefaultTableStorageSettings();
        }
    }

    public class UsageMetricsRepoSettings : IUsageMetricsRepoSettings
    {
        public IConnectionSettings UsageMetricsTableStorage { get; }

        public UsageMetricsRepoSettings(IConfiguration configuration)
        {
            UsageMetricsTableStorage = configuration.CreateDefaultTableStorageSettings();
        }   
    }

    public class DeploymentInstanceRepoSettings : IDeploymentInstanceRepoSettings
    {
        public IConnectionSettings InstanceDocDbStorage { get; }

        public DeploymentInstanceRepoSettings(IConfiguration configuration)
        {
            InstanceDocDbStorage = configuration.CreateDefaultDBStorageSettings();
        }
    }


    public class DeviceConfigurationSettings : IDeviceConfigurationSettings
    {
        public IConnectionSettings DeviceConfigurationtAdminDocDbStorage { get; }
        public IConnectionSettings DeviceConfigurationAdminTableStorage { get; }

        public DeviceConfigurationSettings(IConfiguration configuration)
        {
            DeviceConfigurationtAdminDocDbStorage = configuration.CreateDefaultDBStorageSettings();
            DeviceConfigurationAdminTableStorage = configuration.CreateDefaultTableStorageSettings();
        }
    }
}
