using System;

namespace The_Project.Exceptions
{
    internal sealed class ConnectionRefusedException : Exception
    {
        public ConnectionRefusedException(string response) : base(response)
        {
        }
    }
}