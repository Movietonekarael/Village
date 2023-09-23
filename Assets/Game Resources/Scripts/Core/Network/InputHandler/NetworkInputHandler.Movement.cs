using GameCore.GameControls;
using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Network
    {
        public partial class NetworkInputHandler : IMovement
        {
            [Inject] private readonly IMovement _movement;

            public event Action OnMovementStart;
            public event Action OnMovementFinish;
            public event Action<Vector2> OnMovement;
            public event Action OnRunningChanged;
            public event Action OnDashed;
            public event Action OnJumped;

            private readonly NetworkVariable<bool> _isMoving = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            private readonly NetworkVariable<Vector2> _movingDirection = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


            private void SubscribeForMovingNetworkVariables()
            {
                _isMoving.OnValueChanged += IsMovingChanged;
                _movingDirection.OnValueChanged += MovementDirectionChanged;

            }

            private void UnsubscribeForMovingNetworkVariables()
            {
                _isMoving.OnValueChanged -= IsMovingChanged;
                _movingDirection.OnValueChanged -= MovementDirectionChanged;

            }

            private void IsMovingChanged(bool previousValue, bool newValue)
            {
                if (previousValue == false && newValue == true)
                {
                    OnMovementStart?.Invoke();
                }
                else if (previousValue == true && newValue == false)
                {
                    OnMovementFinish?.Invoke();
                }
            }

            private void MovementDirectionChanged(Vector2 previousValue, Vector2 newValue)
            {
                OnMovement?.Invoke(newValue);
            }

            private void SubscribeForMovementEvents()
            {
                _movement.OnMovementStart += StartMoving;
                _movement.OnMovementFinish += StopMoving;
                _movement.OnMovement += HandleMoving;
                _movement.OnRunningChanged += ChangeRunStateServerRpc;
                _movement.OnDashed += HandleDashServerRpc;
                _movement.OnJumped += HandleJumpServerRpc;
            }

            private void UnsubscribeForMovementEvents()
            {
                _movement.OnMovementStart -= StartMoving;
                _movement.OnMovementFinish -= StopMoving;
                _movement.OnMovement -= HandleMoving;
                _movement.OnRunningChanged -= ChangeRunStateServerRpc;
                _movement.OnDashed -= HandleDashServerRpc;
                _movement.OnJumped -= HandleJumpServerRpc;
            }

            private void StartMoving()
            {
                _isMoving.Value = true;
            }

            private void StopMoving()
            {
                _isMoving.Value = false;
            }

            private void HandleMoving(Vector2 direction)
            {
                if (_isMoving.Value == true)
                {
                    _movingDirection.Value = direction;
                }
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void ChangeRunStateServerRpc()
            {
                OnRunningChanged?.Invoke();
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void HandleDashServerRpc()
            {
                OnDashed?.Invoke();
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void HandleJumpServerRpc()
            {
                OnJumped?.Invoke();
            }
        }
    }
}