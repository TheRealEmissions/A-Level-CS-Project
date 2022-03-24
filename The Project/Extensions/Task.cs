using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Extensions
{
    public static class TaskExtension
    {
        public static async Task<T> FirstSuccessAsync<T>(this IList<Task<T?>> tasks)
        {
            Debug.WriteLine(tasks.Count);
            List<Task<T?>> taskList = new(tasks);
            taskList.Reverse();
            List<IList<Task<T?>>> splitTaskList = Array<Task<T?>>.SplitArr(taskList);

            T? result = default;
            foreach (IList<Task<T?>> subTasks in splitTaskList)
            {
                while (subTasks.Count > 0)
                {
                    Task<T?> currentCompleted = await Task.WhenAny(subTasks);
                    if (currentCompleted.Status == TaskStatus.RanToCompletion && currentCompleted.Result is not null)
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

            return result is not null ? result : throw new TaskFirstSuccessNoSuccessException("result is null");
        }
    }
}