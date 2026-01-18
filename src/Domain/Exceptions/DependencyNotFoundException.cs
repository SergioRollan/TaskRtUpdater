namespace TaskRtUpdater.src.Domain.Exceptions
{
    public class DependencyNotFoundException : Exception
    {
        public int DependencyId { get; }

        public DependencyNotFoundException(int dependencyId)
            : base($"Task with id {dependencyId} does not exist so it cannot be a dependency.")
        {
            DependencyId = dependencyId;
        }
    }
}