using GameCore.GameMovement;
using GameCore.Memory;
using Lightbug.CharacterControllerPro.Core;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace Network
    {
        public sealed class PlayerSpawner : DefaultNetworkBehaviour, AssetLoader.IAssetLoaderBehaviour
        {
            [SerializeField] private Player _player;
            [SerializeField] private AssetReferenceGameObject _playerDoll;
            [SerializeField] private AssetReferenceGameObject _characterActor;

            public event Action<AssetLoader.IAssetLoaderBehaviour> OnDestaction;

            protected async override void OnServerNetworkSpawn()
            {
                await TrySpawnPlayerDoll();
                await SpawnCharacterActor();
            }

            protected override void AllOnNetworkDespawn()
            {
                OnDestaction?.Invoke(this);
            }

            private async Task TrySpawnPlayerDoll()
            {
                var spawnPoint = await WaitUntilSpawnPointFound();
                var dollInstance = await CreateDollInstance(spawnPoint);
                ReferenceDollToPlayer(dollInstance);
                SpawnDollOnNetwork(dollInstance);


                async Task<Vector3> WaitUntilSpawnPointFound()
                {
                    Vector3 spawnPoint;
                    while (!PlayerSpawnPoint.TryGetSpawnPoint(out spawnPoint))
                    {
                        await Task.Yield();
                    }
                    return spawnPoint;
                }

                async Task<GameObject> CreateDollInstance(Vector3 spawnPoint)
                {
                    var dollInstance = await AssetLoader.InstantiateAssetCached<GameObject>(this, _playerDoll);
                    DontDestroyOnLoad(dollInstance);
                    dollInstance.transform.position = spawnPoint;

                    return dollInstance;
                }

                void ReferenceDollToPlayer(GameObject dollInstance)
                {
                    var playerDollComponent = dollInstance.GetComponent<PlayerDoll>();
                    playerDollComponent.Player = _player;
                    playerDollComponent.NetworkInputHandler = _player.NetworkInputHandler;
                    _player.PlayerDoll = playerDollComponent;
                    _player.Interactor.InteractionPoint = playerDollComponent.InteractionPoint;
                    _player.Interactor.InteractionAudio = playerDollComponent.GrabAudioSource;
                }

                void SpawnDollOnNetwork(GameObject dollInstance)
                {
                    var networkObject = dollInstance.GetComponent<NetworkObject>();
                    networkObject.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
                }
            }

            private async Task SpawnCharacterActor()
            {
                var characterActorInstance = await CreateCharacterActorInstance();
                ReferenceCharacterACtorToPlayer(characterActorInstance);
                ReferencePlayerDollToCharacterActorTransformCopyer(characterActorInstance);


                async Task<GameObject> CreateCharacterActorInstance()
                {
                    var characterActorInstance = await AssetLoader.InstantiateAssetCached<GameObject>(this, _characterActor);
                    DontDestroyOnLoad(characterActorInstance);
                    characterActorInstance.transform.position = _player.PlayerDoll.transform.position;

                    return characterActorInstance;
                }

                void ReferenceCharacterACtorToPlayer(GameObject characterActorInstance)
                {
                    var characterActorComponent = characterActorInstance.GetComponent<CharacterActor>();
                    _player.Actor = characterActorComponent;
                }

                void ReferencePlayerDollToCharacterActorTransformCopyer(GameObject characterActorInstance)
                {
                    var transformCopyer = characterActorInstance.GetComponent<TransformCopyer>();
                    transformCopyer.CopyToTransform = _player.PlayerDoll.transform;
                }
            }
        }
    }
}