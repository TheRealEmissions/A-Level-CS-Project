using System;

namespace The_Project.Exceptions
{
    public class AccountCreationException : Exception
    {
        public string Type { get; }

        public AccountCreationException(string type) : base(type)
        {
            Type = type;
        }
    }
}