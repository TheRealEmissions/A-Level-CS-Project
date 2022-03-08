using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace The_Project.Extensions
{
    public static class TaskExtension<T>
    {
        public static async Task<T> FirstSuccess(IEnumerable<Task<T?>> tasks)
        {
            List<Task<T?>> taskList = new(tasks);

            T? result = default;

            while (taskList.Count > 0)
            {
                Task<T?> currentCompleted = await Task.WhenAny(taskList);
                if (currentCompleted.Status == TaskStatus.RanToCompletion && currentCompleted.Result is T)
                {
                    result = currentCompleted.Result;
                    break;
                }
                else
                {
                    _ = taskList.Remove(currentCompleted);
                }
            }

            return result is not null ? result : throw new Exception("result is null");
        }
    }
}