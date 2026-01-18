using Microsoft.EntityFrameworkCore;
using TaskRtUpdater.src.Domain;
using TaskRtUpdater.src.Domain.Exceptions;
using TaskRtUpdater.src.Infrastructure.Data;

namespace TaskRtUpdater.src.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskEntity> GetByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Dependencies)
                    .ThenInclude(d => d.Dependency)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                throw new TaskNotFoundException(id);
            }

            return task;
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Dependencies)
                    .ThenInclude(d => d.Dependency)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TaskEntity>> GetCriticalPath()
        {
            var tasks = await _context.Tasks
                .Include(t => t.Dependencies)
                    .ThenInclude(d => d.Dependency)
                .ToListAsync();

            if (tasks.Count == 0)
            {
                return new List<TaskEntity>();
            }

            // Construir grafo
            var graph = new DirectedAcyclicGraph();
            foreach (var task in tasks)
            {
                graph.AddNode(task.Id);
            }

            var dependencies = await _context.Dependencies.ToListAsync();
            foreach (var dep in dependencies)
            {
                graph.AddEdge(dep.DependencyId, dep.TaskId);
            }

            var topoOrder = graph.TopologicalSort();

            var longestPath = CalculateLongestPath(tasks, dependencies, graph, topoOrder);

            return longestPath;
        }

        private List<TaskEntity> CalculateLongestPath(
            List<TaskEntity> tasks,
            List<TaskDependency> dependencies,
            DirectedAcyclicGraph graph,
            List<int> topoOrder)
        {
            var taskDict = tasks.ToDictionary(t => t.Id);
            var distances = new Dictionary<int, int>();
            var previous = new Dictionary<int, int?>();

            foreach (var taskId in taskDict.Keys)
            {
                distances[taskId] = 0;
                previous[taskId] = null;
            }

            foreach (var taskId in topoOrder)
            {
                var task = taskDict[taskId];
                
                var incomingDeps = dependencies.Where(d => d.TaskId == taskId).ToList();
                
                foreach (var dep in incomingDeps)
                {
                    var depTaskId = dep.DependencyId;
                    var newDistance = distances[depTaskId] + task.Duration;
                    
                    if (newDistance > distances[taskId])
                    {
                        distances[taskId] = newDistance;
                        previous[taskId] = depTaskId;
                    }
                }
            }

            var maxDistance = distances.Values.Max();
            var endNode = distances.FirstOrDefault(d => d.Value == maxDistance).Key;

            var criticalPath = new List<int>();
            var current = endNode;
            
            while (current != 0 && previous.ContainsKey(current) && previous[current] != null)
            {
                criticalPath.Insert(0, current);
                current = previous[current]!.Value;
            }
            
            if (current != 0)
            {
                criticalPath.Insert(0, current);
            }

            if (criticalPath.Count == 0 && tasks.Count > 0)
            {
                var independentTasks = tasks.Where(t => 
                    !dependencies.Any(d => d.TaskId == t.Id)).ToList();
                if (independentTasks.Any())
                {
                    return independentTasks.OrderByDescending(t => t.Duration).Take(1).ToList();
                }
                return new List<TaskEntity> { tasks.First() };
            }

            return criticalPath.Select(id => taskDict[id]).ToList();
        }

        public async Task AddAsync(TaskEntity task)
        {
            var graph = new DirectedAcyclicGraph();
            var existingTasks = await _context.Tasks.Select(t => t.Id).ToListAsync();
            foreach (var taskId in existingTasks)
            {
                graph.AddNode(taskId);
            }
            graph.AddNode(task.Id);
            var existingDeps = await _context.Dependencies.ToListAsync();
            foreach (var dep in existingDeps)
            {
                graph.AddEdge(dep.DependencyId, dep.TaskId);
            }
            foreach (var dep in task.Dependencies)
            {
                graph.AddEdge(dep.DependencyId, dep.TaskId);
            }

            task.CreatedAt = DateTime.UtcNow;
            task.ModifiedAt = DateTime.UtcNow;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskEntity task)
        {
            var existingTask = await _context.Tasks
                .Include(t => t.Dependencies)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            if (existingTask == null)
            {
                throw new TaskNotFoundException(task.Id);
            }

            if (task.Dependencies.Any())
            {
                var graph = new DirectedAcyclicGraph();
                var allTasks = await _context.Tasks.Select(t => t.Id).ToListAsync();
                foreach (var taskId in allTasks)
                {
                    graph.AddNode(taskId);
                }

                var existingDeps = await _context.Dependencies
                    .Where(d => d.TaskId != task.Id)
                    .ToListAsync();
                
                foreach (var dep in existingDeps)
                {
                    graph.AddEdge(dep.DependencyId, dep.TaskId);
                }

                foreach (var dep in task.Dependencies)
                {
                    graph.AddEdge(dep.DependencyId, dep.TaskId);
                }
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.Duration = task.Duration;
            existingTask.ModifiedAt = DateTime.UtcNow;

            _context.Dependencies.RemoveRange(existingTask.Dependencies);
            foreach (var dep in task.Dependencies)
            {
                dep.TaskId = task.Id;
                _context.Dependencies.Add(dep);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Dependencies)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                throw new TaskNotFoundException(id);
            }

            var dependentDependencies = await _context.Dependencies
                .Where(d => d.DependencyId == id)
                .ToListAsync();

            _context.Dependencies.RemoveRange(dependentDependencies);
            _context.Dependencies.RemoveRange(task.Dependencies);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        Task ITaskRepository.DeleteAsync(int id)
        {
            return DeleteAsync(id);
        }
    }
}
