using GameCore.GameControls;
using GameCore.GameMovement;
using GameCore.Interactions;
using GameCore.Inventory;
using GameCore.Services;
using Lightbug.CharacterControllerPro.Core;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        public sealed class Player : DefaultNetworkBehaviour
        {
            [Inject] private IMovement _movement;

            public NetworkInputHandler NetworkInputHandler;
            public PlayerMovementStateMachine PlayerMovement;
            public Interactor Interactor;

            private PlayerDoll _playerDoll;
            private CharacterActor _characterActor;

            private bool _playerDollIsReady = false;
            private bool _characterActorIsReady = false;

            public PlayerDoll PlayerDoll
            {
                get => _playerDoll;
                set
                {
                    if (value == null) return;

                    _playerDollIsReady = true;
                    _playerDoll = value;
                    if (IsClient && IsOwner)
                    {
                        StartPlayerMovement();
                    } 
                    else
                    {
                        DestroyPlayerMovement();
                    }
                    
                    if (IsServer)
                    {
                        SetDropPoint();
                    }
                }
            }

            public CharacterActor Actor
            {
                get => _characterActor;
                set
                {
                    if (value == null) return;

                    _characterActorIsReady = true;
                    _characterActor = value;
                    StartPlayerMovement();
                }
            }


            protected override void AllOnNetworkSpawn()
            {
                DontDestroyOnLoad(this.gameObject);
                ChangeName();


                void ChangeName()
                {
                    var newName = gameObject.name[..gameObject.name.IndexOf("(Clone)")];
                    gameObject.name = newName;
                }
            }

            protected override void OnClientNetworkSpawn()
            {
                InjectItself();
                InjectComponents();
                DestroyNonClientComponents();


                void InjectItself()
                {
                    InstantiateService.Singleton.DiContainer.Inject(this);
                }

                void InjectComponents()
                {
                    if (NetworkInputHandler == null)
                    {
                        Debug.LogError("NetworkInputHandler is not attached to player.");
                        return;
                    }
                    InstantiateService.Singleton.DiContainer.Inject(NetworkInputHandler);
                }

                void DestroyNonClientComponents()
                {
                    if (IsServer) return;
                    Destroy(GetComponent<PlayerInventory>());
                    Destroy(GetComponent<Interactor>());
                    if (IsOwner) return;
                    Destroy(GetComponent<NetworkPlayerInventory>());
                }
            }

            private void StartPlayerMovement()
            {
                if (!_playerDollIsReady || !_characterActorIsReady) return;

                PlayerMovement.CharacterActor = Actor;
                PlayerMovement.AnimatorController = PlayerDoll.DollAnimator;
                PlayerMovement.Movement = _movement;
                PlayerMovement.ÑameraAngle = PlayerDoll.CameraRotator;
                PlayerMovement.StartMovement();
            }

            private void SetDropPoint()
            {
                var inventory = GetComponent<PlayerInventory>();
                inventory.DropPoint = _playerDoll.DropPoint;
            }

            private void DestroyPlayerMovement()
            {
                Destroy(PlayerMovement);
            }
        }
    }
}