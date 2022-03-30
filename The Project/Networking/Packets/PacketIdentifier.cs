namespace The_Project.Networking.Packets
{
    public static class PacketIdentifier
    {
        public enum Packet
        {
            AccountIdVerification,
            PublicKey,
            Message,
            Exception,
            ConnectionVerified
        }

        public enum Exceptions
        {
            FailedVerification
        }
    }
}
