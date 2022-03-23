using System;

namespace The_Project.Exceptions
{
    internal class PasswordHashMismatchException : Exception
    {
        public PasswordHashMismatchException() : base("ACCOUNT CREATION")
        {
        }
    }
}