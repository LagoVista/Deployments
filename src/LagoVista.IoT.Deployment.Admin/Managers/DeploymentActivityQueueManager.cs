using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using Microsoft.Azure.EventHubs;
using System.Text;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using LagoVista.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentActivityQueueManager : ManagerBase, IDeploymentActivityQueueManager
    {
        ICompletedDeploymentActivityRepo _completedRepo;
        IFailedDeploymentActivityRepo _failedRepo;
        IDeploymentActivityRepo _repo;
        IDeploymentActionEventHubSettings _settings;

        const string EhConnectionString = "Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={1}";
        EventHubClient _eventHubClient;


        public DeploymentActivityQueueManager(IDeploymentActivityRepo repo, IFailedDeploymentActivityRepo failedRepo,
                ICompletedDeploymentActivityRepo completedRepo, IDeploymentActionEventHubSettings settings, IAdminLogger logger,
                IAppConfig appConfig, IDependencyManager depmanager, ISecurity security)
            : base(logger, appConfig, depmanager, security)
        {
            _repo = repo;
            _failedRepo = failedRepo;
            _completedRepo = completedRepo;
            
            _settings = settings;

            var bldr = new EventHubsConnectionStringBuilder(string.Format(EhConnectionString, _settings.DeploymentActivityEventHubConnection.Name, _settings.DeploymentActivityEventHubConnection.AccessKey))
            {
                EntityPath = _settings.DeploymentActivityHubName
            };

            _eventHubClient = EventHubClient.CreateFromConnectionString(bldr.ToString());
        }

        public async Task Enqueue(DeploymentActivity deploymentActivity)
        {
            await _repo.AddDeploymentActivityAsync(deploymentActivity);

            var buffer = Encoding.UTF8.GetBytes(deploymentActivity.RowKey);
            var eventData = new EventData(buffer);
            await _eventHubClient.SendAsync(eventData);
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetCompletedActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentActivity));
            return await _completedRepo.GetCompletedDeploymentActivitiesForResourceIdAsync(resourceId);
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetFailedActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentActivity));
            return await _failedRepo.GetFailedDeploymentActivitiesForResourceIdAsync(resourceId);
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetActiveActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(DeploymentActivity));
            return await _repo.GetForResourceIdAsync(resourceId);
        }
    }
}
