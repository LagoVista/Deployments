// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: da3f864585ad9123e2b9a96726faff3f8a5584bec18c276db0d1b38dfa5a0d24
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeploymentInstanceStatus_Title, DeploymentAdminResources.Names.DeploymentInstanceStatus_Description,
     DeploymentAdminResources.Names.DeploymentInstanceStatus_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-pz-report-4",
     GetListUrl: "/api/deployment/instance/{id}/statushistory")]
    public class DeploymentInstanceStatus : TableStorageEntity
    {
        public static DeploymentInstanceStatus Create(string instanceId, EntityHeader user)
        {
            var row = new DeploymentInstanceStatus()
            {
                PartitionKey = instanceId,
                TimeStamp = DateTime.UtcNow.ToJSONString(),
                RowKey = DateTime.UtcNow.ToInverseTicksRowKey(),
                TransitionById = user.Id,
                TransitionByName = user.Text
            };

            return row;
        }

        public bool OldDeploy { get; set; }
        public bool NewDeploy { get; set; }

        public string TimeStamp { get; set; }
        public string OldState { get; set; }
        public string NewState { get; set; }
        public string Version { get; set; }
        public string Details { get; set; }
        public string TransitionById { get; set; }
        public string TransitionByName { get; set; }
    }
}
