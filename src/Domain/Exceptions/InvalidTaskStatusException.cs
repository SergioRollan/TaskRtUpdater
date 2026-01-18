namespace TaskRtUpdater.src.Domain.Exceptions
{
    public class InvalidTaskStatusException : Exception
    {
        public InvalidTaskStatusException(string message) : base(message)
        {
        }
    }
}
