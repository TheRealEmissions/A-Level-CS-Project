using System;

namespace The_Project.Exceptions
{
    internal class PasswordHashMismatchException : Exception
    {
        public string Type { get; }

        public PasswordHashMismatchException() : base("ACCOUNT CREATION")
        {
        }

        public PasswordHashMismatchException(string Type) : this()
        {
            this.Type = Type;
        }
    }
}