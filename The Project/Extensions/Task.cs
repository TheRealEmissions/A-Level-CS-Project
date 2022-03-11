using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine(tasks.Count());
            List<Task<T?>> taskList = new(tasks);
            taskList.Reverse();
            List<IEnumerable<Task<T?>>> splitTaskList = Array<Task<T?>>.SplitArr(taskList);

            T? result = default;
            foreach (List<Task<T?>> subTasks in splitTaskList)
            {
                while (subTasks.Count > 0)
                {
                    Task<T?> currentCompleted = await Task.WhenAny(subTasks);
                    if (currentCompleted.Status == TaskStatus.RanToCompletion && currentCompleted.Result is T)
                    {
                        result = await currentCompleted;
                        break;
                    }
                    else
                    {
                        _ = subTasks.Remove(currentCompleted);
                    }
                }
                if (result is not null)
                {
                    break;
                }
            }

            return result is not null ? result : throw new Exception("result is null");
        }
    }
}