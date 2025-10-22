// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ed3b2388f64bd19d06bc0c4f513ba91772217352a3f863f7590ef5bf97d4f664
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using LagoVista.IoT.Pipeline.CloudRepos;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    internal class RouteSupportRepo : IRouteSupportRepo
    {
        private readonly IStorageUtils _storageUtils;

        public RouteSupportRepo(IStorageUtils stoageUtils, IPipelineAdminRepoSettings settings)
        {
            stoageUtils.SetConnection(settings.PipelineAdminDocDbStorage);
            _storageUtils = stoageUtils ?? throw new System.ArgumentNullException(nameof(stoageUtils));
        }

        public async Task SetDefaultPipelineModulesAsync(EntityHeader org, Route route)
        {
            var sentinalConfig = await _storageUtils.FindWithKeyAsync<SentinelConfiguration>("anonymous", org);
            if(sentinalConfig != null)
            {
                var sentintal = route.PipelineModules.FirstOrDefault(mod => mod.ModuleType.Value == PipelineModuleType.Sentinel);
                sentintal.Module = new EntityHeader<DeviceAdmin.Interfaces.IPipelineModuleConfiguration>() { Id = sentinalConfig.Id, Key = sentinalConfig.Key, Text = sentinalConfig.Name };
                sentintal.Name = sentintal.Module.Text;
            }

            var inputConfig = await _storageUtils.FindWithKeyAsync<InputTranslatorConfiguration>("default", org);
            if (inputConfig != null)
            {
                var inputTranslator = route.PipelineModules.FirstOrDefault(mod => mod.ModuleType.Value == PipelineModuleType.InputTranslator);
                inputTranslator.Module = new EntityHeader<DeviceAdmin.Interfaces.IPipelineModuleConfiguration>() { Id = inputConfig.Id, Key = inputConfig.Key, Text = inputConfig.Name };
                inputTranslator.Name = inputTranslator.Module.Text;
            }

            var workflowConfig = await _storageUtils.FindWithKeyAsync<DeviceWorkflow>("default", org);
            if(workflowConfig != null)
            {
                var workflow = route.PipelineModules.FirstOrDefault(mod => mod.ModuleType.Value == PipelineModuleType.Workflow);
                workflow.Module = new EntityHeader<DeviceAdmin.Interfaces.IPipelineModuleConfiguration>() { Id = workflowConfig.Id, Key = workflowConfig.Key, Text = workflowConfig.Name };
                workflow.Name = workflow.Module.Text;
            }

            var outputConfig = await _storageUtils.FindWithKeyAsync<OutputTranslatorConfiguration>("default", org);
            if (outputConfig != null)
            {
                var outputTranslator = route.PipelineModules.FirstOrDefault(mod => mod.ModuleType.Value == PipelineModuleType.OutputTranslator);
             
                outputTranslator.Module = new EntityHeader<DeviceAdmin.Interfaces.IPipelineModuleConfiguration>() { Id = outputConfig.Id, Key = outputConfig.Key, Text = outputConfig.Name };
                outputTranslator.Name = outputTranslator.Module.Text;
            }
        }
    }
}
