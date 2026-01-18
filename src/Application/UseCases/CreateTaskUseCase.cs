using System;
using TaskRtUpdater.src.Application.DTOs;
using TaskRtUpdater.src.Domain;
using TaskRtUpdater.src.Domain.Exceptions;
using TaskRtUpdater.src.Enums;
using TaskRtUpdater.src.Infrastructure.Observer;
using TaskRtUpdater.src.Infrastructure.Repositories;

namespace TaskRtUpdater.src.Application.UseCases
{
    public class CreateTaskUseCase
    {
        private readonly ITaskRepository _repository;
        private readonly INotificationService _notifier;

        public CreateTaskUseCase(ITaskRepository repository, INotificationService notifier)
        {
            _repository = repository;
            _notifier = notifier;
        }

        public async Task<TaskEntity> ExecuteAsync(CreateTaskDto dto)
        {
            if (dto.Dependencies != null && dto.Dependencies.Any())
            {
                foreach (var depId in dto.Dependencies)
                {
                    if (depId == 0)
                    {
                        throw new InvalidOperationException("Task cannot depend on either unexisting tasks or itself.");
                    }

                    var dependency = await _repository.GetByIdAsync(depId);
                    if (dependency == null)
                    {
                        throw new DependencyNotFoundException(depId);
                    }
                }
            }

            var task = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = Enums.TaskStatus.Pending,
                Priority = dto.Priority,
                Duration = dto.Duration,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(task);
            if (dto.Dependencies != null && dto.Dependencies.Any())
            {
                task.Dependencies = dto.Dependencies.Select(depId => new TaskDependency
                {
                    TaskId = task.Id,
                    DependencyId = depId
                }).ToList();

                await _repository.UpdateAsync(task);
            }

            await _notifier.NotifyTaskCreatedAsync(task.Id, task.Title, task.Description);
            return task;
        }
    }
}