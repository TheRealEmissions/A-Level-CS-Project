using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class CreateConnectionException : Exception
    {
        public CreateConnectionException(string error) : base(error)
        {
        }
    }
}