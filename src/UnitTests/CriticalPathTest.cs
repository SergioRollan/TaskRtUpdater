using global::TaskRtUpdater.src.Domain;
using global::TaskRtUpdater.src.Infrastructure.Data;
using global::TaskRtUpdater.src.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TaskRtUpdater.src.UnitTests
{
    namespace TaskRtUpdater.src.UnitTests
    {
        public class CriticalPathTests
        {
            [Fact]
            public async Task GetCriticalPath_Returns_LongestPath()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase("critical_path_db_" + Guid.NewGuid())
                    .Options;

                using var context = new AppDbContext(options);

                var t1 = new TaskEntity { Id = 1, Title = "A", Description = "Task A", Duration = 3, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
                var t2 = new TaskEntity { Id = 2, Title = "B", Description = "Task B", Duration = 2, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
                var t3 = new TaskEntity { Id = 3, Title = "C", Description = "Task C", Duration = 4, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };
                var t4 = new TaskEntity { Id = 4, Title = "D", Description = "Task D", Duration = 5, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow };

                context.Tasks.AddRange(t1, t2, t3, t4);

                context.Dependencies.AddRange(
                    new TaskDependency { TaskId = 2, DependencyId = 1 }, // B depende de A
                    new TaskDependency { TaskId = 3, DependencyId = 2 }  // C depende de B
                );

                context.SaveChanges();

                var repo = new TaskRepository(context);

                var result = await repo.GetCriticalPath();

                Assert.NotNull(result);
                Assert.Equal(new[] { 1, 2, 3 }, result.Select(t => t.Id).ToArray());
            }
        }
    }
}