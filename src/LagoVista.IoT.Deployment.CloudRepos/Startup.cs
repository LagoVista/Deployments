using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.CloudRepos.Repos;

namespace LagoVista.IoT.Deployment.CloudRepos
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDeviceConfigurationRepo, DeviceConfigurationRepo>();
            services.AddTransient<ISolutionRepo, SolutionRepo>();
            services.AddTransient<IUsageMetricsRepo, UsageMetricsRepo>();
            services.AddTransient<IDeploymentActivityRepo, DeploymentActivityRepo>();
            services.AddTransient<IRemoteDeploymentRepo, RemoteDeploymentRepo>();
            services.AddTransient<IFailedDeploymentActivityRepo, FailedDeploymentActivityRepo>();
            services.AddTransient<ICompletedDeploymentActivityRepo, CompletedDeploymentActivityRepo>();
            services.AddTransient<IContainerRepositoryRepo, ContainerRepositoryRepo>();
            services.AddTransient<IDeploymentInstanceRepo, DeploymentInstanceRepo>();
            services.AddTransient<IDeploymentHostRepo, DeploymentHostRepo>();
            services.AddTransient<ISolutionVersionRepo, SolutionVersionsRepo>();
            services.AddTransient<IDeploymentHostStatusRepo, DeploymentHostStatusRepo>();
            services.AddTransient<IClientAppRepo, ClientAppRepo>();
            services.AddTransient<IIntegrationRepo, IntegrationRepo>();
            services.AddTransient<IDeploymentInstanceStatusRepo, DeploymentInstanceStatusRepo>();
            services.AddTransient<IInstanceAccountsRepo, InstanceAccountRepo>();
            services.AddTransient<IRouteSupportRepo, RouteSupportRepo>();
            services.AddTransient<IDeviceConfigurationRepo, DeviceConfigurationRepo>();
            services.AddTransient<IDeviceErrorCodesRepo, DeviceErrorCodesRepo>();
            services.AddTransient<IDeviceNotificationRepo, DeviceNotificationRepo>();
            services.AddTransient<IDeviceNotificationTracking, DeviceNotificationTracking>();
            services.AddTransient<IDeviceStatusRepo, DeviceStatusRepo>();
            services.AddTransient<ISystemTestRepo, SystemTestRepo>();
            services.AddTransient<IIncidentRepo, IncidentRepo>();
            services.AddTransient<IIncidentProtocolRepo, IncidentProtocolRepo>();
        }
    }
}