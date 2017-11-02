using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage.Blob;
using LagoVista.Core.Interfaces;
using Newtonsoft.Json;
using LagoVista.Core;
using System.Linq;
using System.IO;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class SolutionVersionsRepo : TableStorageBase<SolutionVersion>, ISolutionVersionRepo
    {
        IConnectionSettings _repoSettings;
        IAdminLogger _adminLogger;

        public SolutionVersionsRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {
            _repoSettings = repoSettings.DeploymentAdminTableStorage;
            _adminLogger = adminLogger;
        }

        private const string CONTAINER_ID = "solutions";

        private CloudBlobClient CreateBlobClient(string solutionId)
        {
            var baseuri = $"https://{_repoSettings.AccountId}.blob.core.windows.net";

            var uri = new Uri(baseuri);
            return new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(_repoSettings.AccountId, _repoSettings.AccessKey));
        }

        public async Task<Solution> GetSolutionVersionAsync(string solutionId, string versionId)
        {
            var cloudClient = CreateBlobClient(solutionId);
            var primaryContainer = cloudClient.GetContainerReference(CONTAINER_ID);
            var blob = primaryContainer.GetBlobReference( GetBlobName(solutionId, versionId));
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                using (var rdr = new StreamReader(ms))
                using (var jsonTextReader = new JsonTextReader(rdr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<Solution>(jsonTextReader);
                }
            }
        }

        private string GetBlobName(string solutionId, string versionId)
        {
            return $"{solutionId.ToLower()}/{versionId.ToLower()}.solution";
        }

        public Task<IEnumerable<SolutionVersion>> GetSolutionVersionsAsync(string solutionId)
        {
            return GetByParitionIdAsync(solutionId);
        }

        public async Task<SolutionVersion> PublishSolutionVersionAsync(SolutionVersion solutionVersion, Solution solution)
        {
            Console.WriteLine(solutionVersion.SolutionId);
            solutionVersion.RowKey = Guid.NewGuid().ToId();
            solutionVersion.PartitionKey = solution.Id;
            solutionVersion.TimeStamp = DateTime.UtcNow.ToJSONString();
            solutionVersion.Uri = $"https://{_repoSettings.AccountId}.blob.core.windows.net/{CONTAINER_ID}/{GetBlobName(solutionVersion.SolutionId,solutionVersion.RowKey)}";

            if (string.IsNullOrEmpty(solutionVersion.Status))
            {
                solutionVersion.Status = "New";
            }
           
            if (solutionVersion.Version == 0)
            {
                var versions = await GetSolutionVersionsAsync(solution.Id);
                if (versions.Any())
                {
                    solutionVersion.Version = versions.Max(ver => ver.Version) + 1;
                }
                else
                {
                    solutionVersion.Version = 1.0;
                }
            }

            var cloudClient = CreateBlobClient(solution.Id);
            var primaryContainer = cloudClient.GetContainerReference(CONTAINER_ID);
            await primaryContainer.CreateIfNotExistsAsync();
            var blob = primaryContainer.GetBlockBlobReference(GetBlobName(solutionVersion.SolutionId, solutionVersion.RowKey));
            var json = JsonConvert.SerializeObject(solution);
            await blob.UploadTextAsync(json);
            await InsertAsync(solutionVersion);
            return solutionVersion;
        }

        public async Task UpdateSolutionVersionStatusAsync(string solutionId, string versionId, string newStatus)
        {
            var solutionVersion = await GetAsync(solutionId, versionId);
            solutionVersion.Status = newStatus;
            await UpdateAsync(solutionVersion);
        }
    }
}
