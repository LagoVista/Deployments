using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IDeploymentActivityQueueManager
    {
        Task Enqueue(Models.DeploymentActivity deploymentActivity);
    }
}
