using TaskRtUpdater.src.Domain;

namespace TaskRtUpdater.src.Infrastructure.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskEntity> GetByIdAsync(int id);
        Task<List<TaskEntity>> GetAllAsync();
        Task<List<TaskEntity>> GetCriticalPath();
        Task AddAsync(TaskEntity task);
        Task UpdateAsync(TaskEntity task);
        Task DeleteAsync(int id);
    }
}
