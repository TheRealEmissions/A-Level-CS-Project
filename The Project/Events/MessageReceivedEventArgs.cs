using System;

namespace The_Project.Events
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Ciphertext { get; init; }
    }
}
