using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admins;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class RemoteServiceManager : IRemoteServiceManager
    {
        private readonly IProxyFactory _proxyFactory;

        public RemoteServiceManager(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }


        public async Task<InvokeResult> AddInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.AddInstanceAccountAsync(instanceId, account);
        }

        public async Task<InvokeResult> ProvisionInstanceAsync(string orgId, string hostId, string instanceId, InstanceService instanceService, List<InstanceAccount> accounts)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.ProvisionInstanceAsync(instanceId, instanceService, accounts);
        }

        public async Task<InvokeResult> RemoteInstancePausingAsync(string orgId, string hostId, string instanceId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.InstanceStatusChangedAsync(instanceId, Models.DeploymentInstanceStates.Pausing.ToString());
        }

        public async Task<InvokeResult> RemoteInstanceStartingAsync(string orgId, string hostId, string instanceId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.InstanceStatusChangedAsync(instanceId, Models.DeploymentInstanceStates.Starting.ToString());
        }

        public async Task<InvokeResult> RemoteInstanceStoppingAsync(string orgId, string hostId, string instanceId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.InstanceStatusChangedAsync(instanceId, Models.DeploymentInstanceStates.Stopping.ToString());
        }

        public async Task<InvokeResult> RemoveInstanceAccountAsync(string orgId, string hostId, string instanceId, string id)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                OrganizationId = orgId,
                HostId = hostId,
            });

            return await remoteService.RemoveInstanceAccountAsync(instanceId, id);
        }

        public async Task<InvokeResult> RemoveInstanceAsync(string orgId, string hostId, string instanceId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                InstanceId = orgId,
                HostId = hostId,
            });

            return await remoteService.RemoveInstanceAsync(instanceId);
        }

        public async Task<InvokeResult> RestartAsync(string orgId, string hostId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                InstanceId = orgId,
                HostId = hostId,
            });

            return await remoteService.RestartAsync();
        }

        public async Task<InvokeResult> StartAsync(string orgId, string hostId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                InstanceId = orgId,
                HostId = hostId,
            });

            return await remoteService.StartAsync();
        }

        public async Task<InvokeResult> StopAsync(string orgId, string hostId)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                InstanceId = orgId,
                HostId = hostId,
            });

            return await remoteService.StopAsync();
        }

        public async Task<InvokeResult> UpdateInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account)
        {
            var remoteService = _proxyFactory.Create<IRemoteHostServices>(new ProxySettings()
            {
                InstanceId = orgId,
                HostId = hostId,
            });

            return await remoteService.UpdateInstanceAccountAsync(instanceId, account);
        }
    }
}
