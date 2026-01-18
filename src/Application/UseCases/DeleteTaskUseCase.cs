using TaskRtUpdater.src.Domain;
using TaskRtUpdater.src.Infrastructure.Observer;
using TaskRtUpdater.src.Infrastructure.Repositories;

namespace TaskRtUpdater.src.Application.UseCases
{
    public class DeleteTaskUseCase
    {
        private readonly ITaskRepository _repository;
        private readonly INotificationService _notifier;

        public DeleteTaskUseCase(ITaskRepository repository, INotificationService notifier)
        {
            _repository = repository;
            _notifier = notifier;
        }

        public async Task ExecuteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _notifier.NotifyTaskDeletedAsync(id);
        }
    }
}