using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.IoT.DeviceAdmin.Interfaces;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class RouteModuleConfig : EntityHeader
    {
        public RouteModuleConfig()
        {
            SecondaryPipelineModuleConfigurations = new List<EntityHeader>();
            Mappings = new List<KeyValuePair<string, string>>();
            Id = Guid.NewGuid().ToId();
            Text = Resources.DeploymentAdminResources.RouteModuleConfig_Unassigned; 
        }

        public List<KeyValuePair<string, string>> Mappings { get; set; }

        public EntityHeader PrimaryOutputPipelineModuleConfiguration { get; set; }

        public List<EntityHeader> SecondaryPipelineModuleConfigurations { get; set; }

        public EntityHeader<PipelineModuleType> ModuleType { get; set; }

        public DiagramLocation DiagramLocation { get; set; }

        public EntityHeader<IPipelineModuleConfiguration> Value { get; set; }
    }
}
