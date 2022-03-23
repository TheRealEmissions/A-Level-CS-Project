using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class PasswordUpdateException : Exception
    {
        public PasswordUpdateException() : base()
        {
        }

        public PasswordUpdateException(string Message) : base(Message)
        {
        }
    }
}