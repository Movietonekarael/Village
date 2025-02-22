using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using GameCore.SceneManagement;
using Unity.Burst;

namespace GameCore
{
    namespace Network
    {
        public class NetworkConnectionService : DefaultNetworkBehaviour
        {
            public static GameType GameType = GameType.None;
            public static string ConnectionCode = string.Empty;

            private const string _PREFAB_NAME = "NetworkConnectionService";
            private const string _MAIN_MENU_ASSET_NAME = "Menu";

            private static NetworkConnectionService _singleton = null;
            public static NetworkConnectionService Singleton => _singleton;


            public static async Task CreateInstance()
            {
                if (_singleton == null)
                {
                    var loadHandle = Addressables.LoadAssetAsync<GameObject>(_PREFAB_NAME);
                    await loadHandle.Task;

                    var prefab = loadHandle.Result;
                    var prefabInstance = Instantiate(prefab);
                    prefabInstance.name = prefab.name;
                    var networkObject = prefabInstance.GetComponent<NetworkObject>();
                    networkObject.Spawn(false);
                    _singleton = prefabInstance.GetComponent<NetworkConnectionService>();
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

            protected override void OnServerNetworkSpawn()
            {
                SubscribeForClientConnection();
            }

            protected override void OnServerNetworkDespawn()
            {
                UnsubscribeForClientConnection();
            }

            protected override void OnClientNetworkSpawn()
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += ShutdownConnectionAndLoadMainMenu;
            }

            private void SubscribeForClientConnection()
            {
                AddressablesSceneManager.OnClientSceneManagerSpawned += SendSyncronization;
            }

            private void UnsubscribeForClientConnection()
            {
                AddressablesSceneManager.OnClientSceneManagerSpawned -= SendSyncronization;
            }

            private void SendSyncronization(ulong clientId)
            {
                AddressablesSceneManager.Singleton.SynchronizeScenes(clientId);
            }

            public static async Task StartHost()
            {
                NetworkManagerPrefabs.Singleton.CreateDefaultNetworkManager();
                NetworkManager.Singleton.StartHost();
                await AddressablesSceneManager.CreateInstance();
                await NetworkConnectionService.CreateInstance();
                GameType = GameType.SinglePlayer;
            }

            public static async Task StartHost(RelayHostData hostData)
            {
                NetworkManagerPrefabs.Singleton.CreateRelayNetworkManager();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    hostData.IPv4Address,
                    hostData.Port,
                    hostData.AllocationIDBytes,
                    hostData.Key,
                    hostData.ConnectionData);

                NetworkManager.Singleton.StartHost();
                await AddressablesSceneManager.CreateInstance();
                await NetworkConnectionService.CreateInstance();
                GameType = GameType.Multiplayer;
            }

            public static void StartClient(RelayJoinData joinData)
            {
                NetworkManagerPrefabs.Singleton.CreateRelayNetworkManager();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinData.IPv4Address,
                    joinData.Port,
                    joinData.AllocationIDBytes,
                    joinData.Key,
                    joinData.ConnectionData,
                    joinData.HostConnectionData);
                NetworkManager.Singleton.StartClient();
                GameType = GameType.Multiplayer;
            }

            public static void ShutdownConnection()
            {
                NetworkManager.Singleton.Shutdown();
                NetworkManagerPrefabs.Singleton.DestroyNetworkManager();
                if (AddressablesSceneManager.Singleton != null) 
                    AddressablesSceneManager.Singleton.UnloadAll();
                GameType = GameType.None;
            }

            public static async void ShutdownConnectionAndLoadMainMenu(ulong clientId)
            {
                if (clientId == 0)
                {
                    var loadSceneHandle = Addressables.LoadSceneAsync(_MAIN_MENU_ASSET_NAME, LoadSceneMode.Single);
                    await loadSceneHandle.Task;
                    ShutdownConnection();
                }
            }

            public static async Task<RelayJoinData> ConnectToRelayServer(string joinCode)
            {
                await UnityServices.InitializeAsync();

                await AuthenticateConnection();

                JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayJoinData data = new()
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

                await AuthenticateConnection();

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
                ConnectionCode = data.JoinCode;
                Debug.Log($"Code: {data.JoinCode}.");

                return data;
            }
            
            private static async Task AuthenticateConnection()
            {
#if UNITY_EDITOR && PARREL_SYNC
                if (ParrelSync.ClonesManager.IsClone())
                {
                    string customArgument = ParrelSync.ClonesManager.GetArgument();
                    AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
                }
#endif
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }

            [BurstAuthorizedExternalMethod]
            public static ClientRpcParams GetClientParameters(ulong clientId)
            {
                ClientRpcParams clientRpcParams = new()
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                };

                return clientRpcParams;
            }
        }
    }
}