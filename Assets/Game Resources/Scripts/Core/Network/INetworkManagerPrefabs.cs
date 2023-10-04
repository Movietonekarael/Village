namespace GameCore
{
    namespace Network
    {
        public interface INetworkManagerPrefabs
        {
            public void CreateDefaultNetworkManager();
            public void CreateRelayNetworkManager();
            public void DestroyNetworkManager();
        }
    }
}