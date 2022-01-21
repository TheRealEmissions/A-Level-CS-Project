using System;

namespace The_Project.Exceptions
{
    public class AccountCreationException : Exception
    {
        public string Type { get; }

        public AccountCreationException() : base()
        {
        }

        public AccountCreationException(string Type) : base(Type)
        {
            this.Type = Type;
        }
    }
}