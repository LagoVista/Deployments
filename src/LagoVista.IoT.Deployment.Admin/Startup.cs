using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.IoT.Deployment.Admin
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ISolutionManager, SolutionManager>();
            services.AddTransient<IDeviceConfigurationManager, DeviceConfigurationManager>();
            services.AddTransient<IDeploymentHostManager, DeploymentHostManager>();
            services.AddTransient<IDeploymentInstanceManager, DeploymentInstanceManager>();
            services.AddTransient<IInstanceValidator, InstanceValidator>();
        }
    }
}