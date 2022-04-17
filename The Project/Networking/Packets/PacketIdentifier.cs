namespace The_Project.Networking.Packets
{
    internal static class PacketIdentifier
    {
        internal enum Packet
        {
            AccountIdVerification,
            PublicKey,
            Message,
            Exception,
            ConnectionVerified,
            AccountId
        }

        public enum Exceptions
        {
            FailedVerification
        }
    }
}
