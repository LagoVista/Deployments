// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 531fd2fd3fd342f9b271ab71359cae40b0d080e2d0f9e781aa33de8c4e3e71b4
// IndexVersion: 2
// --- END CODE INDEX META ---
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
