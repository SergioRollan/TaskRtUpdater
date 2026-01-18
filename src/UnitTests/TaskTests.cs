using TaskRtUpdater.src.Domain;
using TaskRtUpdater.src.Enums;
using Xunit;

namespace TaskRtUpdater.src.UnitTests
{
    public class TaskTests
    {
        [Fact]
        public void Task_Should_Start_As_Pending()
        {
            var task = new TaskEntity();
            Assert.Equal(Enums.TaskStatus.Pending, task.Status);
        }
    }
}
