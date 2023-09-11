using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace GameCore
{
    namespace Network
    {
        public class NetworkConnectionService : NetworkBehaviour
        {
            public static ConnectionType ConnectionType = ConnectionType.None;
            private const string _PREFAB_NAME = "NetworkConnectionService";

            private static NetworkConnectionService _instance = null;
            public static NetworkConnectionService Instance => _instance;


            public static async void CreateInstance()
            {
                if (_instance == null)
                {
                    var loadHandle = Addressables.LoadAssetAsync<GameObject>(_PREFAB_NAME);
                    await loadHandle.Task;

                    var prefabInstance = Instantiate(loadHandle.Result);
                    var networkObject = prefabInstance.GetComponent<NetworkObject>();
                    networkObject.Spawn(false);
                    _instance = prefabInstance.GetComponent<NetworkConnectionService>();
                }
                else
                {
                    Debug.LogWarning("Instance already created. Quiting creation.");
                    return;
                }
            }

            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            public override void OnNetworkSpawn()
            {
                SubscribeForClientConnection();

                base.OnNetworkSpawn();
            }

            public override void OnNetworkDespawn()
            {
                UnsubscribeForClientConnection();

                base.OnNetworkDespawn();
            }

            private void SubscribeForClientConnection()
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SendSyncronization;
            }

            private void UnsubscribeForClientConnection()
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= SendSyncronization;
            }

            private void SendSyncronization(ulong clientId)
            {
                if (!IsServer) return;

                AddressablesSceneManager.Singleton.SynchronizeScenesClientRpc();
            }

            public static void StartHost(RelayHostData hostData)
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    hostData.IPv4Address,
                    hostData.Port,
                    hostData.AllocationIDBytes,
                    hostData.Key,
                    hostData.ConnectionData);
                NetworkManager.Singleton.StartHost();
            }

            public static void StartClient(RelayJoinData joinData)
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinData.IPv4Address,
                    joinData.Port,
                    joinData.AllocationIDBytes,
                    joinData.Key,
                    joinData.ConnectionData,
                    joinData.HostConnectionData);
                NetworkManager.Singleton.StartClient();
            }

            public static async Task<RelayJoinData> ConnectToRelayServer(string joinCode)
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayJoinData data = new RelayJoinData
                {
                    IPv4Address = allocation.RelayServer.IpV4,
                    Port = (ushort)allocation.RelayServer.Port,
                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    ConnectionData = allocation.ConnectionData,
                    HostConnectionData = allocation.HostConnectionData,
                    Key = allocation.Key,
                };
                return data;
            }

            public static async Task<RelayHostData> StartRelayServer()
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(5);

                RelayHostData data = new()
                {
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