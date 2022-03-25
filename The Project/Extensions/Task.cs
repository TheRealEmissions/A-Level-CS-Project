using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Extensions
{
    public static class TaskExtension
    {
        public static async Task<T> FirstSuccessAsync<T>(this IList<Task<T?>> tasks)
        {
            Debug.WriteLine($"task list count -> {tasks.Count}");
            List<Task<T?>> taskList = new(tasks);
            List<IList<Task<T?>>> splitTaskList = Array<Task<T?>>.SplitArr(taskList);
            Debug.WriteLine($"splitTaskList count -> {splitTaskList.Count}");

            T? result = default;
            foreach (IList<Task<T?>> subTasks in splitTaskList)
            {
                while (subTasks.Count > 0)
                {
                    Task<T?> currentCompleted = await Task.WhenAny(subTasks);
                    T? subResult = await currentCompleted;
                    if (currentCompleted.Status == TaskStatus.RanToCompletion && subResult is not null)
                    {
                        Debug.WriteLine("got subresult -> subresult is not null, ran to completion");
                        if (subResult is TcpClient)
                        {
                            Debug.WriteLine("subresult is tcp client");
                            TcpClient? castedResult = subResult is TcpClient client ? client : null;
                            Debug.WriteLine($"castedresult -> {castedResult}");
                            if (castedResult?.Client.Connected ?? false)
                            {
                                Debug.WriteLine("casted result is not null & is connected");
                                result = subResult;
                                break;
                            }
                        }
                        else
                        {
                            T? castedResult = subResult is T res ? res : default;
                            if (castedResult is not null)
                            {
                                result = subResult;
                                break;
                            }
                        }
                    }
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