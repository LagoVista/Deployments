using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Threading.Tasks;
using LagoVista.Core.Managers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    internal class SystemTestManager : ManagerBase, ISystemTestManager
    {
        private readonly ISystemTestRepo _systemTestsRepo;

    public SystemTestManager(ISystemTestRepo systemTestsRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
        base(logger, appConfig, dependencyManager, security)
    {
        _systemTestsRepo = systemTestsRepo ?? throw new ArgumentNullException(nameof(systemTestsRepo));
    }

    public async Task<InvokeResult> AddSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user)
    {
        ValidationCheck(systemTest, Actions.Create);
        await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Create, user, org);

        await _systemTestsRepo.AddSystemTestAsync(systemTest);

        return InvokeResult.Success;
    }

    public async Task<InvokeResult> DeleteSystemTestAsync(string id, EntityHeader org, EntityHeader user)
    {
        var systemTest = await _systemTestsRepo.GetSystemTestAsync(id);
        await ConfirmNoDepenenciesAsync(systemTest);

        await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Delete, user, org);

        await _systemTestsRepo.DeleteSystemTestAsync(id);

        return InvokeResult.Success;
    }

    public async Task<SystemTest> GetSystemTestAsync(string id, EntityHeader org, EntityHeader user)
    {
        var systemTest = await _systemTestsRepo.GetSystemTestAsync(id);
        await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Read, user, org);

        return systemTest;
    }

    public async Task<SystemTest> GetSystemTestByKeyAsync(string key, EntityHeader org, EntityHeader user)
    {
        var systemTest = await _systemTestsRepo.GetSystemTestByKeyAsync(key, org.Id);
        await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Read, user, org);
        return systemTest;
    }

    public async Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
    {
        await AuthorizeOrgAccessAsync(user, orgId, typeof(SystemTest));
        return await _systemTestsRepo.GetSystemTestsForOrgAsync(orgId, listRequest);
    }

    public async Task<InvokeResult> UpdateSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user)
    {
        ValidationCheck(systemTest, Actions.Create);
        await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Update, user, org);

        await _systemTestsRepo.UpdateSystemTestAsync(systemTest);

        return InvokeResult.Success;
    }
}
}
