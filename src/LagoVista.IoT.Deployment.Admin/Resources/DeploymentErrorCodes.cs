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
        public static ErrorCode InstanceWithoutHost => new ErrorCode() { Code = "DEP1008", Message = DeploymentAdminResources.Err_InstanceWithoutHost };
        public static ErrorCode InstanceWithoutSolution => new ErrorCode() { Code = "DEP1009", Message = DeploymentAdminResources.Err_InstanceWithoutSolution };
        public static ErrorCode CouldNotLoadInstance => new ErrorCode() { Code = "DEP1010", Message = DeploymentAdminResources.Err_CouldNotLoadInstance };



        public static ErrorCode CouldNotLoadSolution => new ErrorCode() { Code = "INS1000", Message = DeploymentAdminResources.Err_CouldNotLoadSolution };

        public static ErrorCode CouldNotLoadPlanner => new ErrorCode() { Code = "INS1001", Message = DeploymentAdminResources.Err_CouldNotLoadPlanner };

        public static ErrorCode NoPlannerSpecified => new ErrorCode() { Code = "INS1002", Message = DeploymentAdminResources.Err_NoPlannerHasBeenSpecified };

        public static ErrorCode CouldNotLoadListener => new ErrorCode() { Code = "INS1003", Message = DeploymentAdminResources.Err_CouldNotLoadListener };

        public static ErrorCode CouldNotLoadDeviceConfiguration => new ErrorCode() { Code = "INS1004", Message = DeploymentAdminResources.Err_CouldNotLoadDeviceConfiguration };

        



    }
}
