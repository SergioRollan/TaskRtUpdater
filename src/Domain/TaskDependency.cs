namespace TaskRtUpdater.src.Domain
{
    public class TaskDependency
    {
        public int TaskId { get; set; }
        public TaskEntity Task { get; set; } = null!;


        public int DependencyId { get; set; }
        public TaskEntity Dependency { get; set; } = null!;
    }
}
