using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovietoneMath;
using GameCore.GameControls;



namespace GameCore.GameMovement
{
    public class PlayerMovementStateMachine : NPCMovementStateMachine
    {
        private InputHandler _inputHandler;

        [SerializeField] private Transform _camera;
        [SerializeField] private Transform _cameraTarget;


        private void OnEnable()
        {
            _inputHandler.OnMovementStart += delegate { OnMovementStart?.Invoke(); };
            _inputHandler.OnMovementFinish += delegate { OnMovementFinish?.Invoke(); };
            _inputHandler.OnMovement += (Vector2 val) => OnMovement?.Invoke(val);
            _inputHandler.OnRunningChanged += delegate { OnRunningStateChanged?.Invoke(); };
            _inputHandler.OnDashed += delegate { OnDashed?.Invoke(); };
            _inputHandler.OnJumped += delegate { OnJump?.Invoke(); };
        }

        private void OnDisable()
        {
            _inputHandler.OnMovementStart -= delegate { OnMovementStart?.Invoke(); };
            _inputHandler.OnMovementFinish -= delegate { OnMovementFinish?.Invoke(); };
            _inputHandler.OnMovement -= (Vector2 val) => OnMovement?.Invoke(val);
            _inputHandler.OnRunningChanged -= delegate { OnRunningStateChanged?.Invoke(); };
            _inputHandler.OnDashed -= delegate { OnDashed?.Invoke(); };
            _inputHandler.OnJumped -= delegate { OnJump?.Invoke(); };
        }

        protected override void Awake()
        {
            base.Awake();
            TryAttachInputHandler();
        }

        private void TryAttachInputHandler()
        {
            _inputHandler = InputHandler.Instance;
            LogWarningIfInputHandlerIsNull();
        }

        private void LogWarningIfInputHandlerIsNull()
        {
            if (_inputHandler is null)
                Debug.LogWarning("There is no InputHandler in the scene to attach to PlayerMovement script.");
        }

        protected override void LocalDirectionOfMovingChanged()
        {
            var localMovingDirection = GetAngleOfLocalMovingDirection();
            var cameraDirection = GetCameraDirecton();
            var cameraDirectionAngle = GetAngleOfGlobalcameraDirection(cameraDirection);
            var angleRotation = CalculateGlobalMovingDirectionAngle(localMovingDirection, cameraDirectionAngle);
            _GlobalDirectionOfMoving = CalculateGlobalMovingDirection(angleRotation);
            _NeededRotation.eulerAngles = SetNeededRotationDependingOnGlobalMovingVector(angleRotation);
        }

        private float GetAngleOfLocalMovingDirection()
        {
            return MathM.Vector.GetAngleOfVector2(_localDirectionOfMoving);
        }

        private Vector2 GetCameraDirecton()
        {
            return new(_cameraTarget.position.x - _camera.position.x,
                       _cameraTarget.position.z - _camera.position.z);
        }

        private float GetAngleOfGlobalcameraDirection(Vector2 cameraDirection)
        {
            return MathM.Vector.GetAngleOfVector2(cameraDirection);
        }

        private float CalculateGlobalMovingDirectionAngle(float angle1, float angle2)
        {
            return angle1 + angle2;
        }

        private Vector2 CalculateGlobalMovingDirection(float angleRotation)
        {
            return MathM.Vector.GetVector2OfAngle(angleRotation);
        }

        private Vector3 SetNeededRotationDependingOnGlobalMovingVector(float angleRotation)
        {
            return new Vector3(.0f, angleRotation, .0f);
        }
    }
}