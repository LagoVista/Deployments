using LagoVista.IoT.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Resources
{
    public class DeploymentErrorCodes
    {
        public static ErrorCode NotDeployed => new ErrorCode() { Code = "DEP1000", Message = DeploymentAdminResources.Errs_NotDeployed };
        public static ErrorCode AlreadyDeployed => new ErrorCode() { Code = "DEP1001", Message = DeploymentAdminResources.Errs_AlreadyDeployed };
        public static ErrorCode MustBeStoppedBeforeRemoving => new ErrorCode() { Code = "DEP1003", Message = DeploymentAdminResources.Errs_MustBeStoppedBeforeRemoving };
        public static ErrorCode InstanceBusy => new ErrorCode() { Code = "DEP1004", Message = DeploymentAdminResources.Errs_InstanceBusy };
        public static ErrorCode InstanceAlreadyRunning  => new ErrorCode() { Code = "DEP1005", Message = DeploymentAdminResources.Err_InstanceAlreadyRunning };
        public static ErrorCode InstanceNotRunning => new ErrorCode() { Code = "DEP1006", Message = DeploymentAdminResources.Err_InstanceNotRunning };
        public static ErrorCode ErrorCommunicatingWithhost => new ErrorCode() { Code = "DEP1007", Message = DeploymentAdminResources.Err_ErrorCommunicatingWithHost };
    }
}
