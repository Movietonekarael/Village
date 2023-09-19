using GameCore.Animation;
using GameCore.GameMovement;
using GameCore.Inventory;
using GameCore.Network;
using GameCore.Services;
using Lightbug.CharacterControllerPro.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using NetworkPlayer = GameCore.Network.NetworkPlayer;

namespace GameCore
{
    public sealed class PlayerSpawn : MonoBehaviour
    {
        [Inject] private InstantiateService _instantiateService;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _characterActorPrefab;
        private GameObject _characterActorInstance;

        private IEventedAnimatorEvents _playerAnimatorEvents;

        private void Awake()
        {
            InstantiateHostPlayer();
        }

        private void Start()
        {
            ConnectPlayers();
            SubscribeForClientConnectionEvents();
        }

        private void InstantiateHostPlayer()
        {
            var networkManager = NetworkManager.Singleton;

            if (!(networkManager.IsClient || networkManager.IsServer) || networkManager.IsHost)
            {
                var playerInstance = _instantiateService.InstantiateObjectWithInjections(_playerPrefab, transform.parent);
                _characterActorInstance = _instantiateService.InstantiateObject(_characterActorPrefab, transform.parent);

                playerInstance.GetComponent<PlayerMovementStateMachine>().CharacterActor = _characterActorInstance.GetComponent<CharacterActor>();
                _characterActorInstance.GetComponent<TransformCopyer>().CopyToTransform = playerInstance.transform;

                var playerInventory = playerInstance.GetComponent<PlayerInventory>();
                _instantiateService.DiContainer.Bind<GameObject>().WithId("Main Player").FromInstance(playerInstance).AsCached().NonLazy();
                _instantiateService.DiContainer.Bind<PlayerInventory>().WithId("Main Player").FromInstance(playerInventory).AsCached().NonLazy();

                var eventedAnimator = playerInstance.GetComponentInChildren<EventedAnimator>();
                _playerAnimatorEvents = eventedAnimator;
                _instantiateService.DiContainer.Bind<EventedAnimator>().WithId("Main Player").FromInstance(eventedAnimator).AsCached().NonLazy();
                _instantiateService.DiContainer.Bind<IEventedAnimator>().WithId("Main Player").FromInstance(eventedAnimator).AsCached().NonLazy();
                _instantiateService.DiContainer.Bind<IEventedAnimatorEvents>().WithId("Main Player").FromInstance(_playerAnimatorEvents).AsCached().NonLazy();
            }
                
        }

        private void ConnectPlayers()
        {
            var networkManager = NetworkManager.Singleton;

            if (networkManager.IsServer)
            {
                foreach (var client in networkManager.ConnectedClientsList)
                {
                    var playerObject = client.PlayerObject;
                    var networkPlayerSpawner = playerObject.GetComponent<NetworkPlayerSpawner>();
                    _instantiateService.DiContainer.Inject(networkPlayerSpawner);

                    var networkState = playerObject.GetComponent<NetworkPlayerTransform>();
                    networkState.SetPlayerTransform(_characterActorInstance.transform);

                    var animatorSynchronizer = playerObject.GetComponent<AnimatorSynchronizer>();
                    animatorSynchronizer.InputEventedAnimator = _playerAnimatorEvents;
                }
            }
            else
            {
                var playerObject = networkManager.LocalClient.PlayerObject;
                var networkPlayerSpawner = playerObject.GetComponent<NetworkPlayerSpawner>();
                _instantiateService.DiContainer.Inject(networkPlayerSpawner);

                var networkInput = playerObject.GetComponent<NetworkPlayerInput>();
                _instantiateService.DiContainer.Inject(networkInput);
                networkInput.SubscribeForMovementEvents();
            }
        }

        private void SubscribeForClientConnectionEvents()
        {
            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsServer)
                return;

            networkManager.OnClientConnectedCallback += SpawnPlayer;
            networkManager.OnClientDisconnectCallback += DisconnectPlayer;
        }

        private void UnsubscribeForClientConnectionEvents()
        {
            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsServer)
                return;

            networkManager.OnClientConnectedCallback -= SpawnPlayer;
            networkManager.OnClientDisconnectCallback -= DisconnectPlayer;
        }

        private void SpawnPlayer(ulong clientId)
        {
            var networkManager = NetworkManager.Singleton;
            var client = networkManager.ConnectedClients[clientId];
            var playerObject = client.PlayerObject;
            var networkPlayerSpawner = playerObject.GetComponent<NetworkPlayerSpawner>();
            _instantiateService.DiContainer.Inject(networkPlayerSpawner);


            networkPlayerSpawner.CreateClientPlayer(transform);
            SpawnAllPlayerDolls(clientId);

            var networkInput = playerObject.GetComponent<NetworkPlayerInput>();
            _instantiateService.DiContainer.Inject(networkInput);
            networkInput.SubscribeForMovementEvents();
        }

        private void SpawnAllPlayerDolls(ulong clientId)
        {
            var networkManager = NetworkManager.Singleton;
            var clients = networkManager.ConnectedClientsList;

            var clientRpcParams = NetworkConnectionService.GetClientParameters(clientId);

            foreach (var client in clients) 
            { 
                var playerObject = client.PlayerObject;
                var spawner = playerObject.GetComponent<NetworkPlayerSpawner>();
                spawner.SpawnPlayerDollClientRpc(new NetworkTransformPR(transform), clientRpcParams);
            }
        }

        private void DisconnectPlayer(ulong clientId) 
        {

        }
    }
}