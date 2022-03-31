using System;

namespace The_Project.Events
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        internal string Ciphertext { get; init; }
    }
}
