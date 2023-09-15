using GameCore.GameControls;
using GameCore.Services;
using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Network
    {
        public sealed class NetworkPlayerInput : NetworkBehaviour, IMovement, ICameraAngle
        {
            [Inject] private IMovement _movement;
            public ICameraAngle CameraAngle;

            public event Action OnMovementStart;
            public event Action OnMovementFinish;
            public event Action<Vector2> OnMovement;
            public event Action OnRunningChanged;
            public event Action OnDashed;
            public event Action OnJumped;
            public event Action<float> OnCameraAngleChanged;

            private NetworkVariable<bool> _isMoving = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            private NetworkVariable<Vector2> _movingDirection = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            private NetworkVariable<float> _cameraAngle = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


            public override void OnNetworkSpawn()
            {
                SubscribeForNetworkVariables();
            }

            private void SubscribeForNetworkVariables()
            {
                if (!IsServer) return;

                _isMoving.OnValueChanged += IsMovingChanged;
                _movingDirection.OnValueChanged += MovementDirectionChanged;
                _cameraAngle.OnValueChanged += CameraAngleChangedOnServer;
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

            private void CameraAngleChangedOnServer(float previousValue, float newValue)
            {
                OnCameraAngleChanged?.Invoke(newValue);
            }

            public void SubscribeForMovementEvents()
            {
                if (IsServer || _movement == null) return;

                _movement.OnMovementStart += StartMoving;
                _movement.OnMovementFinish += StopMoving;
                _movement.OnMovement += HandleMoving;
                _movement.OnRunningChanged += ChangeRunState;
                _movement.OnDashed += HandleDash;
                _movement.OnJumped += HandleJump;
            }

            public void UnsubscribeForMovementEvents()
            {
                if (IsServer || _movement == null) return;

                _movement.OnMovementStart -= StartMoving;
                _movement.OnMovementFinish -= StopMoving;
                _movement.OnMovement -= HandleMoving;
                _movement.OnRunningChanged -= ChangeRunState;
                _movement.OnDashed -= HandleDash;
                _movement.OnJumped -= HandleJump;
            }

            public void SubscribeForCameraControlEvents()
            {
                if (IsServer || CameraAngle == null) return;

                CameraAngle.OnCameraAngleChanged += CameraAngleChangedOnClient;
            }

            public void UnsubscribeForcameraControlEvents()
            {
                if (IsServer || CameraAngle == null) return;

                CameraAngle.OnCameraAngleChanged -= CameraAngleChangedOnClient;
            }

            private void CameraAngleChangedOnClient(float angle)
            {
                _cameraAngle.Value = angle;
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

            private void ChangeRunState()
            {
                ChangeRunStateServerRpc();
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void ChangeRunStateServerRpc()
            {
                OnRunningChanged?.Invoke();
            }

            private void HandleDash()
            {
                HandleDashServerRpc();
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void HandleDashServerRpc()
            {
                OnDashed?.Invoke();
            }

            private void HandleJump()
            {
                HandleJumpServerRpc();
            }

            [ServerRpc(Delivery = RpcDelivery.Unreliable)]
            private void HandleJumpServerRpc()
            {
                OnJumped?.Invoke();
            }

            public override void OnNetworkDespawn()
            {
                UnsubscribeForMovementEvents();
            }
        }
    }
}