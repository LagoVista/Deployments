namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum PipelineModuleStatus
    {
        Idle,
        StartingUp,
        Running,
        Listening,
        Paused,
        ShuttingDown,
        FatalError,
    }
}
