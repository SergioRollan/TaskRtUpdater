namespace TaskRtUpdater.src.Domain.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(int taskId, int dependencyId) 
            : base($"Dependency between {taskId} and {dependencyId} cannot be created, it would generate a loop.")
        {
        }
    }
}
