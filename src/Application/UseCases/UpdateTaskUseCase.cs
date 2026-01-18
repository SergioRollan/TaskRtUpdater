using System;
using TaskRtUpdater.src.Application.DTOs;
using TaskRtUpdater.src.Domain;
using TaskRtUpdater.src.Domain.Exceptions;
using TaskRtUpdater.src.Infrastructure.Observer;
using TaskRtUpdater.src.Infrastructure.Repositories;

namespace TaskRtUpdater.src.Application.UseCases
{
    public class UpdateTaskUseCase
    {
        private readonly ITaskRepository _repository;
        private readonly INotificationService _notifier;

        public UpdateTaskUseCase(ITaskRepository repository, INotificationService notifier)
        {
            _repository = repository;
            _notifier = notifier;
        }

        public async Task<TaskEntity> ExecuteAsync(int id, UpdateTaskStatusDto dto)
        {
            var task = await _repository.GetByIdAsync(id);

            if (task.Status == Enums.TaskStatus.Done && dto.Status != Enums.TaskStatus.Done)
            {
                throw new InvalidTaskStatusException("Cannot change status of a finished task.");
            }

            if (dto.Status == Enums.TaskStatus.InProgress || dto.Status == Enums.TaskStatus.Done)
            {
                var dependencies = task.Dependencies.Select(d => d.Dependency).ToList();
                var incompleteDependencies = dependencies.Where(d => d.Status != Enums.TaskStatus.Done).ToList();

                if (incompleteDependencies.Any())
                {
                    throw new InvalidTaskStatusException(
                        $"Task status cannot be updated since there"
                        + (incompleteDependencies.Count == 1 ? "is" : "are")
                        + incompleteDependencies.Count
                        + " dependenc"
                        + (incompleteDependencies.Count == 1 ? "y":"ies") 
                        + " yet to be completed."
                    );
                }
            }

            task.Status = dto.Status;
            task.ModifiedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(task);

            // Notificar con el estado correcto
            await _notifier.NotifyTaskUpdatedAsync(task.Id, task.Status.ToString());

            return task;
        }
    }
}