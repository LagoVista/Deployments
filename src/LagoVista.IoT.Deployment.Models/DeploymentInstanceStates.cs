// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2fddf2abb539245e5d717b9e996b26af5dce661109e18b530ba69875b2807242
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentInstanceStates
    {
        [EnumLabel(DeploymentInstance.Status_Offline, DeploymentAdminResources.Names.InstanceStates_Offline, typeof(DeploymentAdminResources))]
        Offline,

        [EnumLabel(DeploymentInstance.Status_HostRestarting, DeploymentAdminResources.Names.InstanceStates_HostRestarting, typeof(DeploymentAdminResources))]
        HostRestarting,

        [EnumLabel(DeploymentInstance.Status_HostFailedHealthCheck, DeploymentAdminResources.Names.InstanceStates_HostFailedHealthCheck, typeof(DeploymentAdminResources))]
        HostFailedHealthCheck,

    
        [EnumLabel(DeploymentInstance.Status_DeployingRuntime, DeploymentAdminResources.Names.InstanceStates_DeployingRuntime, typeof(DeploymentAdminResources))]
        DeployingRuntime,
        [EnumLabel(DeploymentInstance.Status_DeployingRuntime, DeploymentAdminResources.Names.InstanceStates_UpdatingRuntime, typeof(DeploymentAdminResources))]
        UpdatingRuntime,
        [EnumLabel(DeploymentInstance.Status_UpdatingSolution, DeploymentAdminResources.Names.InstanceStates_UpdatingSolution, typeof(DeploymentAdminResources))]
        UpdatingSolution,

        [EnumLabel(DeploymentInstance.Status_CreatingRuntime, DeploymentAdminResources.Names.InstanceStates_CreatingRuntime, typeof(DeploymentAdminResources))]
        CreatingRuntime,

        [EnumLabel(DeploymentInstance.Status_StartingRuntime, DeploymentAdminResources.Names.InstanceStates_StartingRuntime, typeof(DeploymentAdminResources))]
        StartingRuntime,

        [EnumLabel(DeploymentInstance.Status_Initializing, DeploymentAdminResources.Names.InstanceStates_Initializing, typeof(DeploymentAdminResources))]
        Initializing,


        [EnumLabel(DeploymentInstance.Status_Starting, DeploymentAdminResources.Names.InstanceStates_Starting, typeof(DeploymentAdminResources))]
        Starting,
        [EnumLabel(DeploymentInstance.Status_Running, DeploymentAdminResources.Names.InstanceStates_Running, typeof(DeploymentAdminResources))]
        Running,
        [EnumLabel(DeploymentInstance.Status_Pausing, DeploymentAdminResources.Names.InstanceStates_Pausing, typeof(DeploymentAdminResources))]
        Pausing,
        [EnumLabel(DeploymentInstance.Status_Paused, DeploymentAdminResources.Names.InstanceStates_Paused, typeof(DeploymentAdminResources))]
        Paused,
        [EnumLabel(DeploymentInstance.Status_Stopping, DeploymentAdminResources.Names.InstanceStates_Stopping, typeof(DeploymentAdminResources))]
        Stopping,
        [EnumLabel(DeploymentInstance.Status_Stopped, DeploymentAdminResources.Names.InstanceStates_Stopped, typeof(DeploymentAdminResources))]
        Stopped,
        

        [EnumLabel(DeploymentInstance.Status_FatalError, DeploymentAdminResources.Names.InstanceStates_FatalError, typeof(DeploymentAdminResources))]
        FatalError,

        [EnumLabel(DeploymentInstance.Status_FailedToInitialize, DeploymentAdminResources.Names.InstanceStates_FailedToInitialize, typeof(DeploymentAdminResources))]
        FailedToInitialize,

        [EnumLabel(DeploymentInstance.Status_FailedToStart, DeploymentAdminResources.Names.InstanceStates_FailedToStart, typeof(DeploymentAdminResources))]
        FailedToStart,
    }
}
