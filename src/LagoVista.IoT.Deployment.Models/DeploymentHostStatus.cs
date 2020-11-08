using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class DeploymentHostStatus : TableStorageEntity
    {
        public static DeploymentHostStatus Create(string hostId, EntityHeader user)
        {
            var row = new DeploymentHostStatus()
            {
                PartitionKey = hostId,
                TimeStamp = DateTime.UtcNow.ToJSONString(),
                RowKey = DateTime.UtcNow.ToInverseTicksRowKey(),
                TransitionById = user.Id,
                TransitionByName = user.Text
            };

            return row;
        }

        public string TimeStamp { get; set; }
        public string OldState { get; set; }
        public string NewState { get; set; }
        public string Version { get; set; }
        public string Details { get; set; }
        public string TransitionById { get; set; }
        public string TransitionByName { get; set; }
    }
}
