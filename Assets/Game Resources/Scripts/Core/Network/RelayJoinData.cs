namespace GameCore
{
    namespace Network
    {
        public struct RelayJoinData
        {
            public string IPv4Address;
            public ushort Port;
            public System.Guid AllocationID;
            public byte[] AllocationIDBytes;
            public byte[] ConnectionData;
            public byte[] HostConnectionData;
            public byte[] Key;
        }
    }
}