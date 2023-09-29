using GameCore.GameControls;
using GameCore.Inventory;
using GameCore.Services;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public sealed class PlayerDoll : DefaultNetworkBehaviour
        {
            [HideInInspector] public Player Player;
            [HideInInspector] public NetworkInputHandler NetworkInputHandler = null;
            public Animator DollAnimator;

            [Header("Client side objects:")]
            [SerializeField] private GameObject _vitrualCamera;
            [SerializeField] private GameObject _cameraTarget;
            [SerializeField] private CameraRotator _cameraRotator;
            [SerializeField] private CameraZoomer _cameraZoomer;

            [Header("Server side objects:")]

            [Header("Other:")]
            [SerializeField] private PlayerHoldItem _holdItem;
            [SerializeField] private GameObject _dropPoint;
            public AudioSource GrabAudioSource;


            public Transform InteractionPoint;

            protected override async void OnClientNetworkSpawn()
            {
                DontDestroyOnLoad(this.gameObject);
                PlayerHoldItemWrapper.PlayerHoldItem = _holdItem;
                if (!IsOwner)
                {
                    DestroyClientObjects();
                    return;
                }
                

                DestroyServerObjects();
                await SetupReferences();
                StartCameraRotator();
                StartCameraZoomer();
                SubscribeNetworkInputHandlerForCamera();


                void DestroyServerObjects()
                {
                    if (IsServer) return;
                        
                    Destroy(_dropPoint);
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
                    InstantiateService.Singleton.DiContainer.Inject(_cameraRotator);
                    _cameraRotator.SubscribeForCameraRotateInput();
                }

                void StartCameraZoomer()
                {
                    InstantiateService.Singleton.DiContainer.Inject(_cameraZoomer);
                    _cameraZoomer.SubscribeForCameraZoomInput();
                }

                void SubscribeNetworkInputHandlerForCamera()
                {
                    NetworkInputHandler.CameraAngle = _cameraRotator;
                    NetworkInputHandler.SubscribeForCameraControlEvents();
                }
            }

            protected override void OnClientNetworkDespawn()
            {
                if (!IsOwner) return;

                NetworkInputHandler.UnsubscribeForcameraControlEvents();
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
                Destroy(_cameraRotator);
                Destroy(_cameraZoomer);
            }
        }
    }
}