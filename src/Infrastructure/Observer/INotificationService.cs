
namespace TaskRtUpdater.src.Infrastructure.Observer
{
    public interface INotificationService
    {
        Task NotifyTaskCreatedAsync(int taskId, string title, string description);
        Task NotifyTaskUpdatedAsync(int taskId, string status);
        Task NotifyTaskDeletedAsync(int taskId);
    }
}