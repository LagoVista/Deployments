using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
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
        public string Details { get; set; }
        public string TransitionById { get; set; }
        public string TransitionByName { get; set; }
    }
}
