using GameCore.GameControls;
using GameCore.GameMovement;
using GameCore.Inventory;
using GameCore.Memory;
using GameCore.SceneManagement;
using GameCore.Services;
using Lightbug.CharacterControllerPro.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static GameCore.Memory.AssetLoader;

namespace GameCore
{
    namespace Network
    {
        public sealed class PlayerDoll : DefaultNetworkBehaviour, IAssetLoaderBehaviour
        {
            [HideInInspector] public Player Player;
            [HideInInspector] public NetworkInputHandler NetworkInputHandler = null;
            [SerializeField] private AssetReferenceGameObject _characterActorAssetReference;
            public Animator DollAnimator;

            [Header("Client side objects:")]
            [SerializeField] private GameObject _vitrualCamera;
            [SerializeField] private GameObject _cameraTarget;
            public CameraRotator CameraRotator;
            [SerializeField] private CameraZoomer _cameraZoomer;

            [Header("Server side objects:")]

            [Header("Other:")]
            [SerializeField] private PlayerHoldItem _holdItem;
            public Transform DropPoint;
            public AudioSource GrabAudioSource;


            public Transform InteractionPoint;

            public event Action<IAssetLoaderBehaviour> OnDestaction;

            protected override async void OnClientNetworkSpawn()
            {
                DontDestroyOnLoad(this.gameObject);
                PlayerHoldItemWrapper.PlayerHoldItem = _holdItem;
                if (!IsOwner)
                {
                    DestroyClientObjects();
                    return;
                }

                DestroyCollider();
                DestroyServerObjects();
                await SetupReferences();
                StartCameraRotator();
                StartCameraZoomer();
                SubscribeNetworkInputHandlerForCamera();
                await TrySpawnCharacterActor();


                void DestroyCollider()
                {
                    Destroy(this.gameObject.GetComponent<CapsuleCollider>());
                }

                void DestroyServerObjects()
                {
                    if (IsServer) return;
                        
                    Destroy(DropPoint.gameObject);
                }

                async Task SetupReferences()
                {
                    NetworkObject playerObject = null;
                    var localClient = NetworkManager.Singleton.LocalClient;
                    while (playerObject == null)
                    {
                        playerObject = localClient.PlayerObject;
                        await Task.Yield();
                    }
                    Player = playerObject.GetComponent<Player>();
                    Player.PlayerDoll = this;
                    NetworkInputHandler = Player.NetworkInputHandler;
                    Player.Interactor.InteractionPoint = InteractionPoint;
                    Player.Interactor.InteractionAudio = GrabAudioSource;
                }

                void StartCameraRotator()
                {
                    InstantiateService.Singleton.DiContainer.Inject(CameraRotator);
                    CameraRotator.SubscribeForCameraRotateInput();
                }

                void StartCameraZoomer()
                {
                    InstantiateService.Singleton.DiContainer.Inject(_cameraZoomer);
                    _cameraZoomer.SubscribeForCameraZoomInput();
                }

                void SubscribeNetworkInputHandlerForCamera()
                {
                    NetworkInputHandler.CameraAngle = CameraRotator;
                    NetworkInputHandler.SubscribeForCameraControlEvents();
                }

                async Task TrySpawnCharacterActor()
                {
                    string[] scenesNames = null;
                    while (true)
                    {
                        if (AddressablesSceneManager.Singleton != null)
                        {
                            scenesNames = AddressablesSceneManager.Singleton.LoadedScenesNames.ToArray();
                            break;
                        }
                        else
                        {
                            await Task.Yield();
                        }
                    }
                    
                    var sceneIsLoaded = false;
                    foreach(var name in scenesNames)
                    {
                        if (name == "TestScene")
                        {
                            sceneIsLoaded = true;
                        }
                    }
                    if (sceneIsLoaded == false)
                    {
                        AddressablesSceneManager.OnSceneLoadedAndActivated += SpawnCharacterActorNow;
                    }
                    else
                    {
                        await SpawnCharacterActor();
                    }
                }
            }

            private async void SpawnCharacterActorNow()
            {
                AddressablesSceneManager.OnSceneLoadedAndActivated -= SpawnCharacterActorNow;
                await SpawnCharacterActor();
            }

            private async Task SpawnCharacterActor()
            {
                var characterActorInstance = await CreateCharacterActorInstance();
                ReferenceCharacterACtorToPlayer(characterActorInstance);
                ReferencePlayerDollToCharacterActorTransformCopyer(characterActorInstance);


                async Task<GameObject> CreateCharacterActorInstance()
                {
                    var characterActorInstance = await AssetLoader.InstantiateAssetCached<GameObject>(this, _characterActorAssetReference);
                    DontDestroyOnLoad(characterActorInstance);
                    characterActorInstance.transform.position = Player.PlayerDoll.transform.position;
                    if (!IsServer)
                    {
                        var physicsLayer = 1 << LayerMask.NameToLayer("Physics");
                        var interactionLayer = 1 << LayerMask.NameToLayer("Interactable");
                        var layerMask = physicsLayer | interactionLayer;
                        var collider = characterActorInstance.GetComponent<CapsuleCollider>();
                        collider.includeLayers = layerMask;
                    }

                    return characterActorInstance;
                }

                void ReferenceCharacterACtorToPlayer(GameObject characterActorInstance)
                {
                    var characterActorComponent = characterActorInstance.GetComponent<CharacterActor>();
                    Player.Actor = characterActorComponent;
                }

                void ReferencePlayerDollToCharacterActorTransformCopyer(GameObject characterActorInstance)
                {
                    var transformCopyer = characterActorInstance.GetComponent<TransformCopyer>();
                    transformCopyer.CopyToTransform = Player.PlayerDoll.transform;
                }
            }

            protected override void OnClientNetworkDespawn()
            {
                if (!IsOwner) return;

                NetworkInputHandler.UnsubscribeForcameraControlEvents();
                OnDestaction?.Invoke(this);
            }

            protected override void OnServerNetworkSpawn()
            {
                if (IsClient && IsOwner) return;

                DestroyClientObjects();
            }

            private void DestroyClientObjects()
            {
                Destroy(_vitrualCamera);
                Destroy(_cameraTarget);
                Destroy(CameraRotator);
                Destroy(_cameraZoomer);
            }
        }
    }
}