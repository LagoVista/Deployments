﻿using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using System;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentActivityRepo :  TableStorageBase<DeploymentActivity>, IDeploymentActivityRepo
    {
        public DeploymentActivityRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) : 
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }
       
        public Task AddDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return InsertAsync(deploymentActivity);
        }

        public Task<IEnumerable<DeploymentActivity>> GetActiveDeploymentActivitiesAsync()
        {
            return GetByFilterAsync(FilterOptions.Create(nameof(DeploymentActivity.Status), FilterOptions.Operators.Equals, DeploymentActivityStatus.Running.ToString()));
        }

        public Task<DeploymentActivity> GetDeploymentActivityAsync(string deploymentActivityId)
        {
            return GetAsync(deploymentActivityId);
        }

        public Task<IEnumerable<DeploymentActivity>> GetForResourceIdAsync(string resourceId)
        {
            return GetByParitionIdAsync(resourceId);
        }

        public Task RemoveDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return RemoveAsync(deploymentActivity);
        }

        public Task UpdateDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return UpdateAsync(deploymentActivity);
        }
    }
}