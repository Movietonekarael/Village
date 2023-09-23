namespace GameCore
{
    namespace Network
    {
        public sealed partial class NetworkInputHandler : DefaultNetworkBehaviour
        {
            protected override void OnServerNetworkSpawn()
            {
                SubscribeForMovingNetworkVariables();
                SubscribeForCameraNetworkVariable();
            }

            protected override void OnServerNetworkDespawn()
            {
                UnsubscribeForMovingNetworkVariables();
                UnsubscribeForCameraNetworkVariable();
            }

            protected override void OnClientNetworkSpawn()
            {
                if (!IsOwner) return;
                SubscribeForMovementEvents();
                SubscribeForInteractionEvent();
            }

            protected override void OnClientNetworkDespawn()
            {
                if (!IsOwner) return;
                UnsubscribeForMovementEvents();
                UnsubscribeForInteractionEvent();
            }
        }
    }
}