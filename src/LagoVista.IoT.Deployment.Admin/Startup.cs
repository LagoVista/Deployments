using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Deployment.Admin.Validation;
using LagoVista.IoT.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.IoT.Deployment.Admin
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITelemetryManager, TelemetryManager>();
            services.AddTransient<ITelemetryService, TelemetryService>();

            services.AddTransient<ISolutionManager, SolutionManager>();
            services.AddTransient<IContainerManager, ContainerManager>();
            services.AddTransient<IDeviceConfigurationManager, DeviceConfigurationManager>();
            services.AddTransient<IDeploymentHostManager, DeploymentHostManager>();
            services.AddTransient<IDeploymentInstanceManager, DeploymentInstanceManager>();
            services.AddTransient<IInstanceValidator, InstanceValidator>();
            services.AddTransient<IDeploymentConnectorService, DeploymentConnectorService>();

            ErrorCodes.Register(typeof(DeploymentErrorCodes));
        }
    }
}