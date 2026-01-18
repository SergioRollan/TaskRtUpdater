using TaskRtUpdater.src.Enums;

namespace TaskRtUpdater.src.Infrastructure.Observer
{
    public class TaskEventMessage
    {
        public string Event { get; set; } = null!;
        public int TaskId { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public long Timestamp { get; set; }

        public TaskEventMessage() { }

        public TaskEventMessage(TaskEventType eventType, int taskId, string? status = "Pending", string? description = null)
        {
            Event = eventType.ToString();
            TaskId = taskId;
            Status = status;
            Description = description;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}