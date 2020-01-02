namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum PipelineModuleStatus
    {
        Idle,
        StartingUp,
        Running,
        Listening,
        Warning,
        Paused,
        ShuttingDown,
        FatalError,
    }
}
