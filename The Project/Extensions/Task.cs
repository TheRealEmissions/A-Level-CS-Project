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
        public static async Task<T> FirstSuccessNullAsReject(IEnumerable<Task<T?>> tasks)
        {
            List<Task<T?>> taskList = new(tasks);
            Task<T>? firstCompleted = default;

            while (taskList.Count > 0)
            {
                Task<T?> currentCompleted = await Task.WhenAny(taskList);
                if (currentCompleted.Status == TaskStatus.RanToCompletion && currentCompleted.Result is T)
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    firstCompleted = currentCompleted;
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                    break;
                }
                else
                {
                    _ = taskList.Remove(currentCompleted);
                }
            }

            return firstCompleted is not null && firstCompleted.Result is T ? firstCompleted.Result : throw new Exception("No tasks completed successfully");
        }
    }
}