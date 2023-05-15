using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovietoneMath;
using GameCore.GameControls;
using UnityEngine.U2D;
using System;
using UnityEngine.InputSystem;
using Lightbug.CharacterControllerPro.Demo;

namespace GameCore.GameMovement
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerMovementStateMachine : NPCMovementStateMachine
    {
        private InputHandler _inputHandler;

        [SerializeField] private Transform _cameraTarget;
        private Action[] _movementActions;
        private Action<Vector2> _movementAction;

        private void OnEnable()
        {
            _inputHandler.OnMovementStart += _movementActions[0];
            _inputHandler.OnMovementFinish += _movementActions[1];
            _inputHandler.OnMovement += _movementAction;
            _inputHandler.OnRunningChanged += _movementActions[2];
            _inputHandler.OnDashed += _movementActions[3];
            _inputHandler.OnJumped += _movementActions[4];
        }

        private void OnDisable()
        {
            _inputHandler.OnMovementStart -= _movementActions[0];
            _inputHandler.OnMovementFinish -= _movementActions[1];
            _inputHandler.OnMovement -= _movementAction;
            _inputHandler.OnRunningChanged -= _movementActions[2];
            _inputHandler.OnDashed -= _movementActions[3];
            _inputHandler.OnJumped -= _movementActions[4];
        }

        protected override void Awake()
        {
            base.Awake();
            _inputHandler = InputHandler.GetInstance("PlayerMovement");
            CacheInputActions();
        }

        private void CacheInputActions()
        {
            _movementActions = new Action[5];

            _movementActions[0] = delegate { OnMovementStart?.Invoke(); };
            _movementActions[1] = delegate { OnMovementFinish?.Invoke(); };
            _movementActions[2] = delegate { OnRunningStateChanged?.Invoke(); };
            _movementActions[3] = delegate { OnDashed?.Invoke(); };
            _movementActions[4] = delegate { OnJump?.Invoke(); };

            _movementAction = (Vector2 val) => OnMovement?.Invoke(val);
        }

        protected override void LocalDirectionOfMovingChanged()
        {
            var localMovingDirection = GetAngleOfLocalMovingDirection();
            var cameraDirectionAngle = GetAngleOfGlobalcameraDirection();
            var angleRotation = CalculateGlobalMovingDirectionAngle(localMovingDirection, cameraDirectionAngle);
            _GlobalDirectionOfMoving = CalculateGlobalMovingDirection(angleRotation);
            _NeededRotation.eulerAngles = SetNeededRotationDependingOnGlobalMovingVector(angleRotation);
        }

        private float GetAngleOfLocalMovingDirection()
        {
            return MathM.Vector.GetAngleOfVector2(_localDirectionOfMoving);
        }

        private float GetAngleOfGlobalcameraDirection()
        {
            var angle = _cameraTarget.rotation.eulerAngles.y;
            if (angle > 180)
                angle -= 360;
            return angle;
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