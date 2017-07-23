using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class Container : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, INoSQLEntity
    {
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public string Key { get; set; }

        public string Status { get; set; }

        public string Repository{ get; set; }
        public string Tag { get; set; }

        public EntityHeader RepositoryType
        {
            get; set;
        }

        public ContainerSummary CreateSummary()
        {
            return new ContainerSummary()
            {
                Description = Description,
                Id = Id,
                IsPublic = IsPublic,
                Name = Name,
            };
        }
    }

    public class ContainerSummary : SummaryData
    {

    }

}
