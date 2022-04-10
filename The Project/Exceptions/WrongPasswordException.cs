using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class WrongPasswordException : DatabaseException
    {
        public WrongPasswordException() : base("WRONG PASSWORD")
        {
        }
    }
}