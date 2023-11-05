using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeviceErrorCodesManager : ManagerBase, IDeviceErrorCodesManager
    {
        private readonly IDeviceErrorCodesRepo _deviceErrorCodesRepo;

        public DeviceErrorCodesManager(IDeviceErrorCodesRepo deviceErrorCodesRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : 
            base(logger, appConfig, dependencyManager, security)
        {
            _deviceErrorCodesRepo = deviceErrorCodesRepo ?? throw new ArgumentNullException(nameof(deviceErrorCodesRepo));
        }

        public async Task<InvokeResult> AddErrorCodeAsync(DeviceErrorCode errorCode, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(errorCode, Actions.Create);
            await AuthorizeAsync(errorCode, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _deviceErrorCodesRepo.AddErrorCodeAsync(errorCode);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteErrorCodeAsync(string id, EntityHeader org, EntityHeader user)
        {
            var errorCode = await _deviceErrorCodesRepo.GetErrorCodeAsync(id);
            await ConfirmNoDepenenciesAsync(errorCode);

            await AuthorizeAsync(errorCode, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _deviceErrorCodesRepo.DeleteErrorCodeAsync(id);

            return InvokeResult.Success;
        }

        public async Task<DeviceErrorCode> GetErrorCodeAsync(string id, EntityHeader org, EntityHeader user)
        {
            var errorCode = await _deviceErrorCodesRepo.GetErrorCodeAsync(id);
            await AuthorizeAsync(errorCode, AuthorizeResult.AuthorizeActions.Read, user, org);

            return errorCode;
        }

        public async Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceErrorCode));
            return await _deviceErrorCodesRepo.GetErrorCodesForOrgAsync(orgId, listRequest);
        }

        public async Task<InvokeResult> UpdateErrorCodeAsync(DeviceErrorCode errorCode, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(errorCode, Actions.Create);
            await AuthorizeAsync(errorCode, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _deviceErrorCodesRepo.AddErrorCodeAsync(errorCode);

            return InvokeResult.Success;
        }
    }
}
