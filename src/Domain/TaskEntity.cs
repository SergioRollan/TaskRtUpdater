namespace TaskRtUpdater.src.Domain
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Enums.TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int Priority { get; set; }
        public int Duration { get; set; }


        public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
    }
}
