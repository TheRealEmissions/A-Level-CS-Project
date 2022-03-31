using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal sealed class AccountCreationException : Exception
    {
        private string Type { get; }

        public AccountCreationException(string type) : base(type)
        {
            Type = type;
        }
    }
}