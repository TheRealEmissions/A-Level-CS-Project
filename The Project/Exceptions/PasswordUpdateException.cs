using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class PasswordUpdateException : Exception
    {
        public PasswordUpdateException()
        {
        }

        public PasswordUpdateException(string message) : base(message)
        {
        }
    }
}