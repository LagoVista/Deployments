using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.Core.Managers;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Rpc.Client;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class TelemetryManager : ManagerBase, ITelemetryManager
    {
        ITelemetryService _telemetryService;
        IProxyFactory _proxyFactory;
        IDeploymentInstanceRepo _deploymentInstanceRepo;

        public TelemetryManager(IAdminLogger adminLogger, IAppConfig appConfig, IDeploymentInstanceRepo deploymentInstanceRepo,
            IProxyFactory proxyFactory, ITelemetryService telemetryService, IDependencyManager dependencyManager, ISecurity security) : 
            base(adminLogger, appConfig, dependencyManager, security)
        {
            _telemetryService = telemetryService;
            _deploymentInstanceRepo = deploymentInstanceRepo;
            _proxyFactory = proxyFactory;
        }
        
        public Task<ListResponse<TelemetryReportData>> GetAllErrorsAsync(ListRequest request, EntityHeader org, EntityHeader user)
        {
            return _telemetryService.GetAllErrorsasync(request);
        }

        public async Task<ListResponse<TelemetryReportData>> GetForDeploymentActivityAsync(string activityid, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return await _telemetryService.GetForDeploymentActviityAsync(activityid, recordType, request);
        }

        public async Task<ListResponse<TelemetryReportData>> GetForDeviceAsync(DeviceRepository deviceRepo, string deviceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            if(deviceRepo.Instance == null)
            {
                return ListResponse<TelemetryReportData>.FromError("No associated instance.");
            }

            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            var instance = await _deploymentInstanceRepo.GetInstanceAsync(deviceRepo.Instance.Id);

            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = deviceRepo.Instance.Id });
                return await proxy.GetForPemAsync(deviceId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForDeviceAsync(deviceId, recordType, request);
            }
        }

        public async Task<ListResponse<TelemetryReportData>> GetForDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            if (deviceRepo.Instance == null)
            {
                return ListResponse<TelemetryReportData>.FromError("No associated instance.");
            }

            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            var instance = await _deploymentInstanceRepo.GetInstanceAsync(deviceRepo.Instance.Id);

            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = deviceRepo.Instance.Id });
                return await proxy.GetForDeviceTypeAsync(deviceTypeId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForDeviceTypeAsync(deviceTypeId, recordType, request);
            }
        }

        public async Task<ListResponse<TelemetryReportData>> GetForHostAsync(string hostId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await  base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            return await  _telemetryService.GetForHostAsync(hostId, recordType, request);
        }

        public async Task<ListResponse<TelemetryReportData>> GetForInstanceAsync(string instanceId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await  base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));

            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);
            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetForInstanceAsync(instanceId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForInstanceAsync(instanceId, recordType, request);
            }
        }

        public async Task<ListResponse<TelemetryReportData>> GetForPemAsync(string instanceId, string pemId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {        
            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));
            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);
            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetForPemAsync(pemId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForPemAsync(pemId, recordType, request);
            }
        }

        public async Task<ListResponse<TelemetryReportData>> GetForPipelineModuleAsync(string instanceId, string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));

            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);
            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetForPipelineModuleAsync(pipelineModuleId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForPipelineModuleAsync(pipelineModuleId, recordType, request);
            }
        }

        public async Task<ListResponse<TelemetryReportData>> GetForPipelineQueueAsync(string instanceId, string pipelineModuleId, string recordType, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await base.AuthorizeOrgAccessAsync(user, org, typeof(TelemetryReportData));

            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);
            if (instance.DeploymentType.Value == DeploymentTypes.OnPremise)
            {
                var proxy = _proxyFactory.Create<ITelemetryService>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetForPipelineQueueAsync(pipelineModuleId, recordType, request);
            }
            else
            {
                return await _telemetryService.GetForPipelineQueueAsync(pipelineModuleId, recordType, request);
            }
        }        
    }
}
