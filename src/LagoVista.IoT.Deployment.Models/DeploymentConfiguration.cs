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
    }
}
