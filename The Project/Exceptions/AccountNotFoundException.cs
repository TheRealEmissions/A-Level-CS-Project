using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class AccountNotFoundException : DatabaseException
    {
        public string Type { get; }

        public AccountNotFoundException() : base("ACCOUNT NOT FOUND")
        {
        }

        public AccountNotFoundException(string type) : this()
        {
            Type = type;
        }
    }
}