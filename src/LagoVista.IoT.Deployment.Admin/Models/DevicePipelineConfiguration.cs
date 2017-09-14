using LagoVista.Core.Models;
using LagoVista.IoT.DeviceAdmin.Interfaces;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class DevicePipelineModuleConfiguration : EntityHeader
    {
        public DevicePipelineModuleConfiguration()
        {
            SecondaryPipelineModuleConfigurations = new List<EntityHeader>();
            Mappings = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> Mappings { get; set; }

        public EntityHeader PrimaryOutputPipelineModuleConfiguration { get; set; }

        public List<EntityHeader> SecondaryPipelineModuleConfigurations { get; set; }

        public EntityHeader<PipelineModuleType> ModuleType { get; set; }

        public DiagramLocation DiagramLocation { get; set; }

        public IPipelineModuleConfiguration Value { get; set; }
    }
}
