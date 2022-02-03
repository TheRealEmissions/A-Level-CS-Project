using System;

namespace The_Project.Exceptions
{
    public class ConnectionRefusedException : Exception
    {
        public ConnectionRefusedException(string response) : base(response)
        {
        }
    }
}