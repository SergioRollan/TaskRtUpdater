namespace TaskRtUpdater.src.Application.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Priority { get; set; }
        public int Duration { get; set; }
        public List<int>? Dependencies { get; set; }
    }
}
