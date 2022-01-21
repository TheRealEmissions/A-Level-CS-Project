using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class WrongPasswordException : Exception
    {
        public WrongPasswordException() : base("WRONG PASSWORD")
        {
        }
    }
}