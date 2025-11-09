// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f9f72cf824beebeeb698895bf5b4f37f92f42de6c91f38d2a56b62df66dbdee3
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System.Text;
using LagoVista.Core.Models;
using System.Collections.Generic;
using LagoVista.Core.Managers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentActivityQueueManager : ManagerBase, IDeploymentActivityQueueManager
    {
        ICompletedDeploymentActivityRepo _completedRepo;
        IFailedDeploymentActivityRepo _failedRepo;
        IDeploymentActivityRepo _repo;
        IDeploymentActionEventHubSettings _settings;

        const string EhConnectionString = "Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName={1};SharedAccessKey={2}";
        EventHubProducerClient _eventHubClient;

        public DeploymentActivityQueueManager(IDeploymentActivityRepo repo, IFailedDeploymentActivityRepo failedRepo,
                ICompletedDeploymentActivityRepo completedRepo, IDeploymentActionEventHubSettings settings, IAdminLogger logger,
                IAppConfig appConfig, IDependencyManager depmanager, ISecurity security)
            : base(logger, appConfig, depmanager, security)
        {
            _repo = repo;
            _failedRepo = failedRepo;
            _completedRepo = completedRepo;
            
            _settings = settings;

            var connectionString = string.Format(EhConnectionString,
                _settings.DeploymentActivityEventHubConnection.AccountId,
                _settings.DeploymentActivityEventHubConnection.UserName,
                _settings.DeploymentActivityEventHubConnection.AccessKey);

            _eventHubClient = new EventHubProducerClient(connectionString, _settings.DeploymentActivityEventHubConnection.ResourceName);        }

        public async Task Enqueue(DeploymentActivity deploymentActivity)
        {
            await _repo.AddDeploymentActivityAsync(deploymentActivity);

            var buffer = Encoding.UTF8.GetBytes(deploymentActivity.RowKey);
            var eventData = new EventData(buffer);
            await _eventHubClient.SendAsync(new List<EventData>() { eventData });
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
