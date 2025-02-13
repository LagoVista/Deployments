﻿using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Admin.Validation;
using LagoVista.IoT.Deployment.Admins;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.Logging;
using System.Resources;

[assembly: NeutralResourcesLanguage("en")]

namespace LagoVista.IoT.Deployment.Admin
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITelemetryManager, TelemetryManager>();
            services.AddTransient<ITelemetryService, TelemetryService>();

            services.AddTransient<IRemoteConfigurationManager, RemoteConfigurationManager>();
            services.AddTransient<ISolutionManager, SolutionManager>();
            services.AddTransient<IContainerRepositoryManager, ContainerRepositoryManager>();
            services.AddTransient<IDeviceConfigurationManager, DeviceConfigurationManager>();
            services.AddTransient<IDeviceConfigHelper, DeviceConfigurationManager>();            
            services.AddTransient<IUsageMetricsManager, UsageMetricsManager>();
            services.AddTransient<IDeploymentHostManager, DeploymentHostManager>();
            services.AddTransient<IDeploymentHostManagerRemote, DeploymentHostManager>();
            services.AddSingleton<IDeploymentInstanceManager, DeploymentInstanceManager>();
            services.AddSingleton<IRemoteDeploymentManager, RemoteDeploymentManager>();
            services.AddTransient<IDeploymentInstanceManagerRemote, DeploymentInstanceManager>();
            services.AddTransient<IDeploymentActivityQueueManager, DeploymentActivityQueueManager>();            
            services.AddTransient<IInstanceValidator, InstanceValidator>();
            services.AddTransient<IDeploymentConnectorService, DeploymentConnectorService>();
            services.AddTransient<IDockerRegisteryServices, DockerRegisteryServices>();
            services.AddTransient<IDeploymentInstanceManagerCore, DeploymentInstanceManagerCore>();
            services.AddTransient<IClientAppManager, ClientAppManager>();
            services.AddTransient<IIntegrationManager, IntegrationManager>();
            services.AddTransient<ITimeZoneServices, TimeZoneService>();
            services.AddTransient<IDeviceArchiveConnector, DeviceArchiveConnectorService>();
            services.AddTransient<IDeviceManagementConnector, DeviceManagementConnectorService>();
            services.AddTransient<ITelemetryConnector, TelemetryConnector>();
            services.AddTransient<IUsageMetricsConnector, UsageMetricsConnector>();
            services.AddTransient<IRemoteServiceManager, RemoteServiceManager>();
            services.AddTransient<IDeviceErrorCodesManager, DeviceErrorCodesManager>();
            services.AddTransient<IDeviceNotificationManager, DeviceNotificationManager>();
            services.AddTransient<IDeviceErrorHandler, DeviceErrorHandler>();
            services.AddTransient<ISystemTestManager, SystemTestManager>();
            services.AddTransient<IIncidentManager, IncidentManager>();
            services.AddTransient<IIncidentProtocolManager, IncidentProtocolManager>();
            services.AddTransient<ITakClient, TakClient>();
            services.AddSingleton<INotificationSender, NotificationSender>();
            services.AddSingleton<ITagReplacementService, TagReplacementService>();
            services.AddSingleton<IRestSender, RESTSender>();
            services.AddSingleton<IMqttSender, MQTTSender>();
            services.AddScoped<ICOTSender, COTSender>();
            services.AddScoped<ISMSSender, SMSSender>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSingleton<INotificationLandingPage, NotificationLandingPage>();
            services.AddSingleton<IDeviceCommandSender, DeviceCommandSender>();
            services.AddSingleton<IDeviceErrorScheduleCheckListener, DeviceErrorScheduleCheckListener>();
            services.AddTransient<IDeviceErrorScheduleCheckSender, DeviceErrorScheduleCheckSender>();
            ErrorCodes.Register(typeof(DeploymentErrorCodes));
        }
    }
}