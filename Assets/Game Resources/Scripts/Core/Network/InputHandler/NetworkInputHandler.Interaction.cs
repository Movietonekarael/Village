using GameCore.GameControls;
using System;
using Unity.Netcode;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        namespace Input
        {
            public partial class NetworkInputHandler : IInteractionPerformer
            {
                [Inject] private readonly IInteractionPerformer _interactionPerformer;
                public event Action OnInteractionPerformed;


                private void SubscribeForInteractionEvent()
                {
                    _interactionPerformer.OnInteractionPerformed += PerformInteractionServerRpc;
                }

                private void UnsubscribeForInteractionEvent()
                {
                    _interactionPerformer.OnInteractionPerformed -= PerformInteractionServerRpc;
                }

                [ServerRpc]
                private void PerformInteractionServerRpc()
                {
                    OnInteractionPerformed?.Invoke();
                }
            }
        }
    }
}