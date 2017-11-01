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

        public CloudBlobClient CreateBlobClient(string solutionId)
        {
            var baseuri = $"https://{_repoSettings.AccountId}.blob.core.windows.net";

            var uri = new Uri(baseuri);
            return new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(_repoSettings.AccountId, _repoSettings.AccessKey));
        }

        public async Task<Solution> GetSolutionVersionAsync(string solutionId, string versionId)
        {
            var cloudClient = CreateBlobClient(solutionId);
            var primaryContainer = cloudClient.GetContainerReference($"app.{solutionId}");
            var blob = primaryContainer.GetBlobReference($"{versionId}.json");
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

        public Task<IEnumerable<SolutionVersion>> GetSolutionVersionsAsync(string solutionId)
        {
            return GetByParitionIdAsync(solutionId);
        }

        public async Task<SolutionVersion> PublishSolutionVersionAsync(SolutionVersion solutionVersion, Solution solution)
        {
            Console.WriteLine("HI");
            Console.WriteLine(solutionVersion.SolutionId);
            solutionVersion.RowKey = Guid.NewGuid().ToId();
            solutionVersion.PartitionKey = solution.Id;
            solutionVersion.TimeStamp = DateTime.UtcNow.ToJSONString();
            solutionVersion.Uri = $"https://{_repoSettings.AccountId}.blob.core.windows.net/app.{solution.Id}/{solutionVersion.RowKey}.json";


            Console.WriteLine("HI3");

            if (string.IsNullOrEmpty(solutionVersion.Status))
            {
                solutionVersion.Status = "New";
            }

            Console.WriteLine("HI4");

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

            Console.WriteLine("HI5");

            var cloudClient = CreateBlobClient(solution.Id);
            Console.WriteLine("HI6");

            var primaryContainer = cloudClient.GetContainerReference($"app-{solution.Id}");
            Console.WriteLine("HI7");
            await primaryContainer.CreateIfNotExistsAsync();
            Console.WriteLine("HI8");
            var blob = primaryContainer.GetBlockBlobReference($"{solutionVersion.RowKey}.json");
            Console.WriteLine("HI9");
            var json = JsonConvert.SerializeObject(solution);
            Console.WriteLine("HI10");
            await blob.UploadTextAsync(json);
            Console.WriteLine("HI11");

            await InsertAsync(solutionVersion);
            Console.WriteLine("HI12");
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
