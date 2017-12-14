using LagoVista.IoT.Deployment.Models.Resources;
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
        public static ErrorCode InstanceAlreadyRunning => new ErrorCode() { Code = "DEP1005", Message = DeploymentAdminResources.Err_InstanceAlreadyRunning };
        public static ErrorCode InstanceNotRunning => new ErrorCode() { Code = "DEP1006", Message = DeploymentAdminResources.Err_InstanceNotRunning };
        public static ErrorCode ErrorCommunicatingWithhost => new ErrorCode() { Code = "DEP1007", Message = DeploymentAdminResources.Err_ErrorCommunicatingWithHost };
        public static ErrorCode InstanceWithoutHost => new ErrorCode() { Code = "DEP1008", Message = DeploymentAdminResources.Err_InstanceWithoutHost };
        public static ErrorCode InstanceWithoutSolution => new ErrorCode() { Code = "DEP1009", Message = DeploymentAdminResources.Err_InstanceWithoutSolution };
        public static ErrorCode CouldNotLoadInstance => new ErrorCode() { Code = "DEP1010", Message = DeploymentAdminResources.Err_CouldNotLoadInstance };

        public static ErrorCode MCPExists => new ErrorCode() { Code = "DEP1011", Message = DeploymentAdminResources.Err_MCPServerExists };
        public static ErrorCode NotificationsServerExists => new ErrorCode() { Code = "DEP1012", Message = DeploymentAdminResources.Err_NotificationServerExists };

        public static ErrorCode NoMCPExits => new ErrorCode() { Code = "DEP1013", Message = DeploymentAdminResources.Err_NoMCPServerExists };
        public static ErrorCode NoNotificationsServerExits => new ErrorCode() { Code = "DEP1014", Message = DeploymentAdminResources.Err_NoNotificationsServerExists };

        public static ErrorCode MultipleMCPServersFound => new ErrorCode() { Code = "DEP1015", Message = DeploymentAdminResources.Err_MultipleMCPServersFound };
        public static ErrorCode MultipleNotificationServersFound => new ErrorCode() { Code = "DEP1016", Message = DeploymentAdminResources.Err_MultipleNotificationServersFound };


        public static ErrorCode CantStartNotStopped => new ErrorCode() { Code = "DEP1017", Message = DeploymentAdminResources.Err_CouldntStart_NotOffline };
        public static ErrorCode CantStopNotRunning => new ErrorCode() { Code = "DEP1018", Message = DeploymentAdminResources.Err_CouldntStop_NotRunning };

        public static ErrorCode CannotDeployContainerToNonRunningHost => new ErrorCode() { Code = "DEP1019", Message = DeploymentAdminResources.Err_CantPublishNotRunning };


        public static ErrorCode CouldNotLoadSolution => new ErrorCode() { Code = "INS1000", Message = DeploymentAdminResources.Err_CouldNotLoadSolution };

        public static ErrorCode CouldNotLoadPlanner => new ErrorCode() { Code = "INS1001", Message = DeploymentAdminResources.Err_CouldNotLoadPlanner };

        public static ErrorCode NoPlannerSpecified => new ErrorCode() { Code = "INS1002", Message = DeploymentAdminResources.Err_NoPlannerHasBeenSpecified };


        public static ErrorCode CanNotDeleteMCPHost => new ErrorCode() { Code = "HST1001", Message = "The MCP Server can not be deleted" };
        public static ErrorCode CanNotDeleteNotificationServerHost => new ErrorCode() { Code = "HST1002", Message = "The Notification Server can not be deleted" };


        public static ErrorCode NoMessageOnRoute => new ErrorCode() { Code = "RTE1001", Message = DeploymentAdminResources.Err_NoMessageDefinitionOnRoute };
        public static ErrorCode EmptyRoute => new ErrorCode() { Code = "RTE1002", Message = DeploymentAdminResources.Err_EmptyRoute };

        public static ErrorCode RouteModuleNameNotDefined => new ErrorCode() { Code = "RTE1003", Message = DeploymentAdminResources.Err_RouteModule_NameNotDefined };
        public static ErrorCode RouteModuleEmptyType => new ErrorCode() { Code = "RTE1004", Message = DeploymentAdminResources.Err_RouteModule_ModuleTypeNotDefined };
        public static ErrorCode RouteModuleEmptyModule => new ErrorCode() { Code = "RTE1005", Message = DeploymentAdminResources.Err_RouteModule_ModuleIsRequired };
        public static ErrorCode RouteCouldNotFindLinkedModule => new ErrorCode() { Code = "RTE1006", Message = DeploymentAdminResources.Err_CouldNotFindDestinationModule };
        public static ErrorCode RouteSourceModuleNull => new ErrorCode() { Code = "RTE1007", Message = "Souce Module on Route is NULL" };
        public static ErrorCode RouteDestinationModuleNull => new ErrorCode() { Code = "RTE1008", Message = "Destination Modoule on Route is NULL" };
        public static ErrorCode RouteMessageDefinitionNull => new ErrorCode() { Code = "RTE1009", Message = "Message Definition on Route is Null" };


        /* error messages when doing full validation on a solution */
        public static ErrorCode NoListeners => new ErrorCode() { Code = "SLN1001", Message = DeploymentAdminResources.Warning_NoListeners };
        public static ErrorCode NoDeviceConfigs => new ErrorCode() { Code = "SLN1002", Message = DeploymentAdminResources.Warning_NoDeviceConfigs };
        public static ErrorCode CouldNotLoadListener => new ErrorCode() { Code = "SLN1003", Message = DeploymentAdminResources.Err_CouldNotLoadListener };
        public static ErrorCode CouldNotLoadDeviceConfiguration => new ErrorCode() { Code = "SLN1004", Message = DeploymentAdminResources.Err_CouldNotLoadDeviceConfiguration };


        public static ErrorCode InvalidOuputCommandMapping => new ErrorCode() { Code = "SLN1005", Message = "Invalid Output Command Mapping" };
        public static ErrorCode InvalidInputCommandMapping => new ErrorCode() { Code = "SLN1006", Message = "Invalid Input Command Mapping" };


    }
}
