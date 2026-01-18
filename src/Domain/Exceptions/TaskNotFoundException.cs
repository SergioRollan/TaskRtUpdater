namespace TaskRtUpdater.src.Domain.Exceptions
{
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(int taskId) 
            : base($"No task with id {taskId} could be found.")
        {
        }
    }
}
