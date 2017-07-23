using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class ContainerRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, INoSQLEntity
    {
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        public string Key { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public String SecurePasswordId { get; set; }

        public string Namespace { get; set; }

        public ContainerRepositorySummary CreateSummary()
        {
            return new ContainerRepositorySummary()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsPublic = IsPublic,
                Key = Key

            };
        }
    }

    public class ContainerRepositorySummary : SummaryData
    {

    }
}
