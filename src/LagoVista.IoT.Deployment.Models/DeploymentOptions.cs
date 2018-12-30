using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentTypes
    {
        [EnumLabel(DeploymentInstance.DeploymentType_Managed, DeploymentAdminResources.Names.DeploymentType_Managed, typeof(DeploymentAdminResources))]
        Managed,
        [EnumLabel(DeploymentInstance.DeploymentType_OnPremise, DeploymentAdminResources.Names.DeploymentType_OnPremise, typeof(DeploymentAdminResources))]
        OnPremise,
    }

    public enum DeploymentConfigurations
    {
        [EnumLabel(DeploymentInstance.DeploymentConfiguration_SingleInstance, DeploymentAdminResources.Names.DeploymentConfiguration_SingleInstance, typeof(DeploymentAdminResources))]
        SingleInstance,
        [EnumLabel(DeploymentInstance.DeploymentConfiguration_Kubernetes, DeploymentAdminResources.Names.DeploymentConfiguration_Kubernetes, typeof(DeploymentAdminResources))]
        Kubernetes,
        [EnumLabel(DeploymentInstance.DeploymentConfiguration_DockerSwarm, DeploymentAdminResources.Names.DeploymentConfiguration_DockerSwarm, typeof(DeploymentAdminResources))]
        DockerSwarm,
    }

    public enum QueueTypes
    {
        [EnumLabel(DeploymentInstance.DeploymentQueueType_InMemory, DeploymentAdminResources.Names.DeploymentQueueType_InMemory, typeof(DeploymentAdminResources))]
        InMemory,
        [EnumLabel(DeploymentInstance.DeploymentQueueType_Kafka, DeploymentAdminResources.Names.DeploymentQueueType_Kafka, typeof(DeploymentAdminResources))]
        Kafka,
        [EnumLabel(DeploymentInstance.DeploymentQueueType_ServiceBus, DeploymentAdminResources.Names.DeploymentQueueType_ServiceBus, typeof(DeploymentAdminResources))]
        ServiceBus,
        [EnumLabel(DeploymentInstance.DeploymentQueueType_RabbitMQ, DeploymentAdminResources.Names.DeploymentQueueType_RabbitMQ, typeof(DeploymentAdminResources))]
        RabbitMQ,
    }

    public enum LogStorage
    {
        [EnumLabel(DeploymentInstance.Deployment_Logging_Local, DeploymentAdminResources.Names.Deployment_Logging_Local, typeof(DeploymentAdminResources))]
        Local,
        [EnumLabel(DeploymentInstance.Deployment_Logging_Cloud, DeploymentAdminResources.Names.Deployment_Logging_Cloud, typeof(DeploymentAdminResources))]
        Cloud,
    }
}
