using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;
using Newtonsoft.Json;
using LagoVista.Core;
using System.Linq;
using System.IO;
using LagoVista.Core.Validation;
using Azure.Storage.Blobs;

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

        private async Task<BlobContainerClient> CreateBlobContainerClient()
        {

            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={_repoSettings.AccountId};AccountKey={_repoSettings.AccessKey}";
            var blobClient = new BlobServiceClient(connectionString);
            try
            {
                var blobContainerClient = blobClient.GetBlobContainerClient(CONTAINER_ID);
                return blobContainerClient;
            }
            catch (Exception)
            {
                var container = await blobClient.CreateBlobContainerAsync(CONTAINER_ID);

                return container.Value;
            }
        }

        public async Task<InvokeResult<Solution>> GetSolutionVersionAsync(string solutionId, string versionId)
        {
            try
            {
                var containerClient = await CreateBlobContainerClient();

                var blobName = GetBlobName(solutionId, versionId);

                var blobClient = containerClient.GetBlobClient(blobName);

                var content = await blobClient.DownloadStreamingAsync();
            
                using (var rdr = new StreamReader(content.Value.Content))
                using (var jsonTextReader = new JsonTextReader(rdr))
                {
                    var serializer = JsonSerializer.Create(_jsonSettings);
                    var solution = serializer.Deserialize<Solution>(jsonTextReader);
                    if (solution == null)
                    {
                        return InvokeResult<Solution>.FromError("Could not deserialize solution.");
                    }
                    else
                    {
                        return InvokeResult<Solution>.Create(solution);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception deserializeing: " + ex.Message);
                return InvokeResult<Solution>.FromError(ex.Message);
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

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            // note: do not change this to auto or array or all - only works with TypeNameHandling.Objects
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Newtonsoft.Json.Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        public async Task<SolutionVersion> PublishSolutionVersionAsync(SolutionVersion solutionVersion, Solution solution)
        {
            solutionVersion.RowKey = Guid.NewGuid().ToId();
            solutionVersion.PartitionKey = solution.Id;
            solutionVersion.TimeStamp = DateTime.UtcNow.ToJSONString();
            solutionVersion.Uri = $"https://{_repoSettings.AccountId}.blob.core.windows.net/{CONTAINER_ID}/{GetBlobName(solutionVersion.SolutionId, solutionVersion.RowKey)}";

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


            var containerClient = await CreateBlobContainerClient();

            var blobName = GetBlobName(solutionVersion.SolutionId, solutionVersion.RowKey);

            var blobClient = containerClient.GetBlobClient(blobName);

            var json = JsonConvert.SerializeObject(solution, _jsonSettings);
            
            var buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
            
            await blobClient.UploadAsync(new BinaryData(buffer));
            
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
