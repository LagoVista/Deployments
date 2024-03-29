﻿using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    enum RemoteDeploymentType
    {
        Normal,
        FaultTolerant,
        SingleInstance,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.RemoteDeployment_Title, 
        DeploymentAdminResources.Names.RemoteDeployment_Help, DeploymentAdminResources.Names.RemoteDeployment_Description,
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        SaveUrl: "/api/deployment/remotedeployment", GetListUrl: "/api/deployment/remotedeployments", GetUrl: "/api/deployment/remotedeployment/{id}", FactoryUrl: "/api/deployment/remotedeployment/factory",
        DeleteUrl: "/api/deployment/remotedeployment/{id}", Icon: "icon-pz-server-cloud")]
    public class RemoteDeployment : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase,  IValidateable, IFormDescriptor, ISummaryFactory
    {
        public RemoteDeployment()
        {
            Instances = new List<DeploymentInstance>();
            Icon = "icon-pz-server-cloud";
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, IsRequired: true, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_PrimaryMCP, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public DeploymentHost PrimaryMCP { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_SecondaryMCP, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public DeploymentHost SecondaryMCP { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_Instances, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public List<DeploymentInstance> Instances { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(DeploymentHost.Name),
                nameof(DeploymentHost.Key),
                nameof(Icon),
                nameof(DeploymentHost.Status),
                nameof(DeploymentHost.HostType),
                nameof(DeploymentHost.Size),
                nameof(DeploymentHost.DnsHostName),
                nameof(DeploymentHost.Ipv4Address),
                nameof(DeploymentHost.Subscription),
                nameof(DeploymentHost.CloudProvider),
                nameof(DeploymentHost.ContainerRepository),
                nameof(DeploymentHost.ContainerTag),
            };
        }

        public RemoteDeploymentSummary CreateSummary()
        {
            return new RemoteDeploymentSummary()
            {
                Id = Id,
                Name = Name,
                Icon = Icon,
                Key = Key,
                Description = Description,
                IsPublic = IsPublic
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }


    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.RemoteDeployment_Title,
        DeploymentAdminResources.Names.RemoteDeployment_Help, DeploymentAdminResources.Names.RemoteDeployment_Description,
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
        SaveUrl: "/api/deployment/remotedeployment", GetListUrl: "/api/deployment/remotedeployments", GetUrl: "/api/deployment/remotedeployment/{id}", FactoryUrl: "/api/deployment/remotedeployment/factory",
        DeleteUrl: "/api/deployment/remotedeployment/{id}", Icon: "icon-pz-server-cloud")]
    public class RemoteDeploymentSummary : SummaryData
    {

    }
}
