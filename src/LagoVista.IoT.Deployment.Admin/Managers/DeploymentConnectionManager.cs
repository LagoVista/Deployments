using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentConnectionManager : ManagerBase, IDeploymentConnectionManager
    {
        public DeploymentConnectionManager(ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {

        }
    }
}
