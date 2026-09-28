using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Rest.Services;
using LagoVista.IoT.Deployment.Models.Runtime;
using LagoVista.Web.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    /// <summary>
    /// Issues short-lived capability leases for the Runtime Data Service.
    /// This remains a control-plane endpoint; runtime data traffic does not flow through this controller.
    /// </summary>
    public class RuntimeDataLeaseController : Controller
    {
        private readonly IDeploymentInstanceRepo _instanceRepo;
        private readonly ISecureStorage _secureStorage;
        private readonly ISignedRequestHttpValidator _signedRequestValidator;
        private readonly RuntimeAccessLeaseTokenIssuer _leaseIssuer;

        public RuntimeDataLeaseController(
            IDeploymentInstanceRepo instanceRepo,
            ISecureStorage secureStorage,
            ISignedRequestHttpValidator signedRequestValidator,
            IConfiguration configuration)
        {
            _instanceRepo = instanceRepo ?? throw new ArgumentNullException(nameof(instanceRepo));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _signedRequestValidator = signedRequestValidator ?? throw new ArgumentNullException(nameof(signedRequestValidator));
            _leaseIssuer = new RuntimeAccessLeaseTokenIssuer(configuration ?? throw new ArgumentNullException(nameof(configuration)));
        }

        [HttpGet("/api/deployment/instance/runtime-data/lease")]
        public async Task<InvokeResult<RuntimeAccessLease>> GetRuntimeDataLeaseAsync()
        {
            var request = HttpContext.Request;

            var orgId = GetHeader(request, InstanceRuntimeController.ORG_ID);
            var orgName = GetHeader(request, InstanceRuntimeController.ORG);
            var userId = GetHeader(request, InstanceRuntimeController.USER_ID);
            var userName = GetHeader(request, InstanceRuntimeController.USER);
            var instanceId = GetHeader(request, InstanceRuntimeController.INSTANCE_ID);

            var org = EntityHeader.Create(orgId, orgName);
            var user = EntityHeader.Create(userId, userName);

            var instance = await _instanceRepo.GetReadOnlyInstanceAsync(instanceId);
            if (instance == null)
                return InvokeResult<RuntimeAccessLease>.FromError("Could not find deployment instance.");

            var key1 = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKeySecureId1, user);
            if (!key1.Successful)
                return InvokeResult<RuntimeAccessLease>.FromErrors(key1.Errors.ToArray());

            var key2 = await _secureStorage.GetSecretAsync(org, instance.SharedAccessKeySecureId2, user);
            if (!key2.Successful)
                return InvokeResult<RuntimeAccessLease>.FromErrors(key2.Errors.ToArray());

            _signedRequestValidator.ValidateRuntimeInstanceHttpV1(request, key1.Result, key2.Result);

            var hostId = instance.PrimaryHost?.Id;
            if (String.IsNullOrWhiteSpace(hostId))
                return InvokeResult<RuntimeAccessLease>.FromError("Deployment instance does not have a primary host.");

            var lease = _leaseIssuer.Create(orgId, instanceId, hostId, RuntimeDataCapabilities.All);
            return InvokeResult<RuntimeAccessLease>.Create(lease);
        }

        private static string GetHeader(HttpRequest request, string header)
        {
            if (request.Headers.TryGetValue(header, out var value))
                return value;

            if (request.Headers.TryGetValue(header.ToLowerInvariant(), out value))
                return value;

            throw new NotAuthorizedException($"Missing request header: {header}");
        }
    }
}
