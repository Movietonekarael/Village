using GameCore.GameMovement;
using GameCore.Interactions;
using GameCore.Inventory;
using GameCore.Services;
using Lightbug.CharacterControllerPro.Core;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public sealed class Player : DefaultNetworkBehaviour
        {
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
                    if (IsServer)
                        StartPlayerMovement();
                    else
                        DestroyPlayerMovement();
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
                InjectComponents();
                DestroyNonClientComponents();


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
                PlayerMovement.Movement = NetworkInputHandler;
                PlayerMovement.ÑameraAngle = NetworkInputHandler;
                PlayerMovement.StartMovement();
            }

            private void DestroyPlayerMovement()
            {
                Destroy(PlayerMovement);
            }
        }
    }
}