﻿using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class AccountNotFoundException : Exception
    {
        public string Type { get; }

        public AccountNotFoundException() : base("ACCOUNT NOT FOUND")
        {
        }

        public AccountNotFoundException(string Type) : this()
        {
            this.Type = Type;
        }
    }
}