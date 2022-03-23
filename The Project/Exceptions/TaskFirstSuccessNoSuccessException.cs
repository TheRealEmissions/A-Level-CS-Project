using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class TaskFirstSuccessNoSuccessException : Exception
    {
        public TaskFirstSuccessNoSuccessException(string Error) : base(Error)
        {
        }
    }
}