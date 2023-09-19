using GameCore.Animation;
using GameCore.GameControls;
using GameCore.GameMovement;
using GameCore.Services;
using Lightbug.CharacterControllerPro.Core;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        public interface IDollSpawner
        {
            public event Action<GameObject> OnDollSpawned;
        }

        [RequireComponent(typeof(IMovement))]
        public sealed class NetworkPlayerSpawner : NetworkBehaviour, IDollSpawner
        {
            [Inject] private InstantiateService _instantiateService;

            [SerializeField] private AssetReferenceGameObject _characterActorReference;
            private GameObject _characterActorInstance;
            private AsyncOperationHandle<GameObject> _characterActorLoadHandle;

            [SerializeField] private AssetReferenceGameObject _clientPlayerReference;
            private GameObject _clientPlayerInstance;
            private AsyncOperationHandle<GameObject> _clientPlayerLoadHandle;

            [SerializeField] private AssetReferenceGameObject _activeDollPlayerReference;
            private GameObject _activeDollPlayerInstance;
            private AsyncOperationHandle<GameObject> _activeDollPlayerLoadHandle;

            [SerializeField] private AssetReferenceGameObject _dollPlayerReference;
            private GameObject _dollPlayerInstance;
            private AsyncOperationHandle<GameObject> _dollPlayerLoadHandle;

            public event Action<GameObject> OnDollSpawned;

            public override void OnNetworkDespawn()
            {
                DestroyClientPlayer();
                DestroyActiveDollPlayer();
                DestroyDollPlayer();
            }

            private void DestroyClientPlayer()
            {
                DestroyAddressableAssetAllocation(_clientPlayerInstance, _clientPlayerLoadHandle);
                DestroyAddressableAssetAllocation(_characterActorInstance, _characterActorLoadHandle);
            }

            private void DestroyActiveDollPlayer()
            {
                DestroyAddressableAssetAllocation(_activeDollPlayerInstance, _activeDollPlayerLoadHandle);
            }

            private void DestroyDollPlayer()
            {
                DestroyAddressableAssetAllocation(_dollPlayerInstance, _dollPlayerLoadHandle);
            }

            private void DestroyAddressableAssetAllocation<T>(T assetInstance, AsyncOperationHandle<T> loadHandle) where T : UnityEngine.Object
            {
                if (assetInstance != null) 
                {
                    Destroy(assetInstance);
                    Addressables.Release(loadHandle);
                }
            }

            public async void CreateClientPlayer(Transform spawnPoint)
            {
                var networkSpawnPoint = new NetworkTransformPR(spawnPoint);

                var clientPlayerLoadTask = LoadAddressableAsset(_clientPlayerReference, 
                                                                _clientPlayerInstance,
                                                                networkSpawnPoint);

                var characterActorLoadTask = LoadAddressableAsset(_characterActorReference,
                                                                  _characterActorInstance,
                                                                  networkSpawnPoint);

                (_clientPlayerInstance, _clientPlayerLoadHandle) = await clientPlayerLoadTask;
                (_characterActorInstance, _characterActorLoadHandle) = await characterActorLoadTask;


                var playerMovement = _clientPlayerInstance.GetComponent<PlayerMovementStateMachine>();
                var movement = gameObject.GetComponent<IMovement>();
                playerMovement.Movement = movement;

                var cameraAngle = gameObject.GetComponent<ICameraAngle>();
                playerMovement.CameraAngleBase = cameraAngle as UnityEngine.Object;
                playerMovement.SubscribeForCameraAngleChange();

                playerMovement.CharacterActor = _characterActorInstance.GetComponent<CharacterActor>();
                _characterActorInstance.GetComponent<TransformCopyer>().CopyToTransform = _clientPlayerInstance.transform;

                var networkState = gameObject.GetComponent<NetworkPlayerTransform>();
                networkState.SetPlayerTransform(_characterActorInstance.transform);

                var animatorSynchronizer = gameObject.GetComponent<AnimatorSynchronizer>();
                animatorSynchronizer.InputEventedAnimator = _clientPlayerInstance.GetComponentInChildren<IEventedAnimatorEvents>();

                DontDestroyOnLoad(_clientPlayerInstance);
                DontDestroyOnLoad(_characterActorInstance);
            }

            private async Task<(T, AsyncOperationHandle<T>)> LoadAddressableAsset<T>(AssetReference asset, 
                                                                                     T currentInstance, 
                                                                                     NetworkTransformPR spawnPoint) where T : UnityEngine.Object
            {
                if (currentInstance != null)
                {
                    Debug.Log($"{asset.Asset.name} instance already created. Aborting creation.");
                    return (null, default);
                }

                var loadHandle = Addressables.LoadAssetAsync<T>(asset);
                var spawnPointRotation = new Quaternion();
                QuaternionCompressor.DecompressQuaternion(ref spawnPointRotation, spawnPoint.Rotation);
                await loadHandle.Task;

                var instance = _instantiateService.InstantiateObject(loadHandle.Result,
                                                                     spawnPoint.Position,
                                                                     spawnPointRotation,
                                                                     null);
                return (instance, loadHandle);
            }

            [ClientRpc]
            public void SpawnPlayerDollClientRpc(NetworkTransformPR spawnPoint, ClientRpcParams clientRpcParams = default)
            {
                if (IsServer) return;

                InstantiateService.Singleton.DiContainer.Inject(this);
                Debug.Log("Spawning dolls.");

                if (IsOwner)
                {
                    LoadActiveDollPlayer(spawnPoint);
                }
                else
                {
                    LoadDollPlayer(spawnPoint);
                }
            }

            private async void LoadActiveDollPlayer(NetworkTransformPR spawnPoint)
            {
                (_activeDollPlayerInstance, _activeDollPlayerLoadHandle) = await LoadAddressableAsset(_activeDollPlayerReference,
                                                                                                     _activeDollPlayerInstance,
                                                                                                     spawnPoint);

                DontDestroyOnLoad(_activeDollPlayerInstance);

                var cameraRotator = _activeDollPlayerInstance.GetComponent<CameraRotator>();
                _instantiateService.DiContainer.Inject(cameraRotator);
                var cameraRotationSubscriber = cameraRotator as ICameraRotationSubscriber;
                cameraRotationSubscriber.SubscribeForCameraRotateInput();

                var cameraZoomer = _activeDollPlayerInstance.GetComponent<CameraZoomer>();
                _instantiateService.DiContainer.Inject(cameraZoomer);
                var cameraZoomSubscriber = cameraZoomer as ICameraZoomSubscriber;
                cameraZoomSubscriber.SubscribeForCameraZoomInput();

                var networkInput = GetComponent<NetworkPlayerInput>();
                networkInput.CameraAngle = cameraRotator;
                networkInput.SubscribeForCameraControlEvents();

                var animatorSynchronizer = _activeDollPlayerInstance.GetComponentInChildren<AnimatorSynchronizer>();
                if (animatorSynchronizer == null) 
                {
                    Debug.Log("Synchronizer is null.");
                }
                else
                {
                    Debug.Log($"Synchronizer: {animatorSynchronizer}");
                }
                var events = gameObject.GetComponent<IEventedAnimatorEvents>();
                if (events == null)
                {
                    Debug.Log("Events is null.");
                }
                else
                {
                    Debug.Log($"Events: {events}");
                }
                animatorSynchronizer.InputEventedAnimator = events;

                var networkPlayerAnimator = gameObject.GetComponent<NetworkPlayerAnimator>();
                networkPlayerAnimator.CopyRuntimeAnimatorControllers(_activeDollPlayerInstance.GetComponentInChildren<Animator>());

                OnDollSpawned?.Invoke(_activeDollPlayerInstance);
            }

            private async void LoadDollPlayer(NetworkTransformPR spawnPoint)
            {
                (_dollPlayerInstance, _dollPlayerLoadHandle) = await LoadAddressableAsset(_dollPlayerReference,
                                                                                         _dollPlayerInstance,
                                                                                         spawnPoint);

                var animatorSynchronizer = _dollPlayerInstance.GetComponentInChildren<AnimatorSynchronizer>();
                animatorSynchronizer.InputEventedAnimator = gameObject.GetComponent<IEventedAnimatorEvents>();

                var networkPlayerAnimator = gameObject.GetComponent<NetworkPlayerAnimator>();
                networkPlayerAnimator.CopyRuntimeAnimatorControllers(_dollPlayerInstance.GetComponentInChildren<Animator>());

                DontDestroyOnLoad(_dollPlayerInstance);

                OnDollSpawned?.Invoke(_dollPlayerInstance);
            }
        }
    }
}

