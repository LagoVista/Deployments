using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using Microsoft.Azure.EventHubs;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentActivityQueueManager : IDeploymentActivityQueueManager
    {
        IDeploymentActivityRepo _repo;
        IDeploymentActionEventHubSettings _settings;

        const string EhConnectionString = "Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={1}";
        EventHubClient _eventHubClient;


        public DeploymentActivityQueueManager(IDeploymentActivityRepo repo, IDeploymentActionEventHubSettings settings)
        {
            _repo = repo;
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
    }
}
