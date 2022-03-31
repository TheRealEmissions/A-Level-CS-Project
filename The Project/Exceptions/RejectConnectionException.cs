using System;

namespace The_Project.Exceptions
{
    [Serializable]
    internal sealed class RejectConnectionException : Exception
    {
        public RejectConnectionException() : base("REJECT CONNECTION")
        {
        }
    }
}