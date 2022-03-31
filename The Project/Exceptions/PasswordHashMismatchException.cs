using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal sealed class PasswordHashMismatchException : Exception
    {
        public PasswordHashMismatchException() : base("ACCOUNT CREATION")
        {
        }
    }
}