using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class TaskFirstSuccessNoSuccessException : Exception
    {
        public TaskFirstSuccessNoSuccessException(string error) : base(error)
        {
        }
    }
}