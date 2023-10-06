using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public sealed partial class NetworkInputHandler : DefaultNetworkBehaviour
        {
            [SerializeField] private bool _allowMovement = false;
            [SerializeField] private bool _allowCameraRotation = false;
            [SerializeField] private bool _allowInteraction = true;


            protected override void OnServerNetworkSpawn()
            {
                if (_allowMovement) SubscribeForMovingNetworkVariables();
                if (_allowCameraRotation) SubscribeForCameraNetworkVariable();
            }

            protected override void OnServerNetworkDespawn()
            {
                if (_allowMovement) UnsubscribeForMovingNetworkVariables();
                if (_allowCameraRotation) UnsubscribeForCameraNetworkVariable();
            }

            protected override void OnClientNetworkSpawn()
            {
                if (!IsOwner) return;
                if (_allowMovement) SubscribeForMovementEvents();
                if (_allowInteraction) SubscribeForInteractionEvent();
            }

            protected override void OnClientNetworkDespawn()
            {
                if (!IsOwner) return;
                if (_allowMovement) UnsubscribeForMovementEvents();
                if (_allowInteraction) UnsubscribeForInteractionEvent();
            }
        }
    }
}