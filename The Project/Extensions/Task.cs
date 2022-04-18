using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Extensions
{
    internal static class TaskExtension
    {
        internal static async Task<T> FirstSuccessAsync<T>(this IList<Task<T?>> tasks)
        {
            // converts IList to List
            List<Task<T?>> taskList = new(tasks);
            // using SplitArr extension method, creates a list of lists of tasks which each list instead of the list contains 2 tasks
            List<IList<Task<T?>>> splitTaskList = Array<Task<T?>>.SplitArr(taskList);

            // assigns result to its default value
            T? result = default;
            foreach (IList<Task<T?>> subTasks in splitTaskList)
            {
                while (subTasks.Count > 0)
                {
                    // returns the task that completes first out of the 2 tasks in the subTasks
                    Task<T?> currentCompleted = await Task.WhenAny(subTasks);
                    // gets the result from the task
                    T? subResult = await currentCompleted;
                    // checks if the task has completed and if the task is not null
                    if (currentCompleted.Status == TaskStatus.RanToCompletion && subResult is not null)
                    {
                        // as this application is primarily being used for sending messages, it was easier to implement TcpClient checking inside of this generic method
                        // to avoid bloat in the rest of the application
                        if (subResult is TcpClient)
                        {

                            TcpClient? castedResult = subResult is TcpClient client ? client : null;

                            if (castedResult?.Client.Connected ?? false)
                            {
                                // found the tcp client that connected and breaks out of the loop
                                result = subResult;
                                break;
                            }
                        }
                        else
                        {
                            // generic method still completes even if subResult is not TcpClient
                            T? castedResult = subResult is T res ? res : default;
                            if (castedResult is not null)
                            {
                                result = subResult;
                                break;
                            }
                        }
                    }

                    // removes the task from the list so the while loop can continue (discards Remove return value)
                    _ = subTasks.Remove(currentCompleted);
                }

                if (result is not null)
                {
                    break;
                }
            }

            Debug.WriteLine("debug");
            return result is not null ? result : throw new TaskFirstSuccessNoSuccessException("result is null");
        }
    }
}