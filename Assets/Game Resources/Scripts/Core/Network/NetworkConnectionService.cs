using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public struct RelayHostData
        {
            public string JoinCode;
            public string IPv4Address;
            public ushort Port;
            public System.Guid AllocationID;
            public byte[] AllocationIDBytes;
            public byte[] ConnectionData;
            public byte[] Key;
        }

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

            private async Task<RelayHostData> StartRelayServer()
            {
                Debug.Log("Starting authentication.");

                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    //If not already logged, log the user in
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Debug.Log("Starting relay server.");

                Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(5);

                RelayHostData data = new RelayHostData
                {
                    // WARNING allocation.RelayServer is deprecated
                    IPv4Address = allocation.RelayServer.IpV4,
                    Port = (ushort)allocation.RelayServer.Port,

                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    ConnectionData = allocation.ConnectionData,
                    Key = allocation.Key,
                };

                data.JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(data.AllocationID);
                Debug.Log($"Data: {data.JoinCode}.");

                return data;
            }
        }
    }
}