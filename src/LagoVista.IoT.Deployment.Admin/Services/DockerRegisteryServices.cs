// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0fa90791998a04a6500f219ef6bbeb185d5eb2658147d102735f1451d740c1df
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;
using LagoVista.IoT.Deployment.Admin.Interfaces;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class DockerRegisteryServices : IDockerRegisteryServices
    {
        const string DOCKER_REPO_URI = "https://hub.docker.com/";

        IAdminLogger _hostLogger;

        public DockerRegisteryServices(IAdminLogger adminLogger)
        {
            _hostLogger = adminLogger;
        }


        public async Task<InvokeResult<string>> GetTokenAsync(string uid, string pwd)
        {
            /* Note we really should be using OAuth here, but that can wait */
            var loginCredentials = new
            {
                username = uid,
                password = pwd
            };

            var strContent = new StringContent(JsonConvert.SerializeObject(loginCredentials), System.Text.Encoding.ASCII, "application/json");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(DOCKER_REPO_URI);
                var loginResponse = await client.PostAsync("v2/users/login/", strContent);

                if (!loginResponse.IsSuccessStatusCode)
                {
                    return InvokeResult<string>.FromError(loginResponse.ReasonPhrase);
                }

                try
                {
                    var responseContent = await loginResponse.Content.ReadAsStringAsync();
                    var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                    return InvokeResult<string>.Create(tokenResult.Token);
                }
                catch (Exception ex)
                {
                    return InvokeResult<string>.FromError(ex.Message);
                }
            }
        }

        public async Task<InvokeResult<DockerRepoResponse>> GetReposAsync(string nameSpace, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(DOCKER_REPO_URI);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("JWT", token);
                var repoResponse = await client.GetAsync($"v2/repositories/{nameSpace}/");

                if (!repoResponse.IsSuccessStatusCode)
                {
                    return InvokeResult<DockerRepoResponse>.FromError(repoResponse.ReasonPhrase);
                }

                try
                {
                    var responseContent = await repoResponse.Content.ReadAsStringAsync();
                    var repos = JsonConvert.DeserializeObject<DockerRepoResponse>(responseContent);
                    return InvokeResult<DockerRepoResponse>.Create(repos);
                }
                catch (Exception ex)
                {
                    return InvokeResult<DockerRepoResponse>.FromError(ex.Message);
                }
            }
        }

        public async Task<InvokeResult<DockerTagsResponse>> GetTagsAsync(string nameSpace, string repo, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(DOCKER_REPO_URI);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("JWT", token);
                var repoResponse = await client.GetAsync($"v2/repositories/{nameSpace}/{repo}/tags");

                if (!repoResponse.IsSuccessStatusCode)
                {
                    return InvokeResult<DockerTagsResponse>.FromError(repoResponse.ReasonPhrase);
                }

                try
                {
                    var responseContent = await repoResponse.Content.ReadAsStringAsync();
                    var repos = JsonConvert.DeserializeObject<DockerTagsResponse>(responseContent);
                    return InvokeResult<DockerTagsResponse>.Create(repos);
                }
                catch (Exception ex)
                {
                    return InvokeResult<DockerTagsResponse>.FromError(ex.Message);
                }
            }
        }
    }
}
