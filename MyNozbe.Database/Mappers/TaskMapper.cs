using MyNozbe.Database.Models;
using MyNozbe.Domain.Models;

namespace MyNozbe.Database.Mappers
{
    public class TaskMapper
    {
        public static TaskModel MapTaskToTaskModel(Task task)
        {
            if (task is null)
            {
                return null;
            }

            return new TaskModel(task.Id, task.Name, task.IsCompleted);
        }
    }
}