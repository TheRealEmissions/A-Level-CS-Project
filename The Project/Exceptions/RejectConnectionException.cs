using System;

namespace The_Project.Exceptions
{
    internal class RejectConnectionException : Exception
    {
        public RejectConnectionException() : base("REJECT CONNECTION")
        {
        }
    }
}