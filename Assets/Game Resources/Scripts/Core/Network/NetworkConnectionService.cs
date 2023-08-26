using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {

        public class NetworkConnectionService : MonoBehaviour
        {
            public static ConnectionType ConnectionType = ConnectionType.None;

            private void Start()
            {
                StartNetworking();
            }

            private void StartNetworking()
            {
                var networkManager = NetworkManager.Singleton;
                
                switch (ConnectionType)
                {
                    case ConnectionType.Host:
                        networkManager.StartHost();
                        break;
                    case ConnectionType.Cliet:
                        networkManager.StartClient();
                        break;
                }
            }

            public static void ConnectToRelayServer()
            {

            }
        }
    }
}