// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 27731d55a9e67d46d128372b677d9fccce084c8903a70666111c27b94525f868
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentHostStatusRepo
    {
        Task AddDeploymentHostStatusAsync(DeploymentHostStatus hostStatus);

        Task<ListResponse<DeploymentHostStatus>> GetStatusHistoryForHostAsync(string hostId, ListRequest listRequest);
    }
}
