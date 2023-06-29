using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovietoneMath;
using GameCore.GameControls;
using UnityEngine.U2D;
using System;
using UnityEngine.InputSystem;
using Lightbug.CharacterControllerPro.Demo;
using Zenject;

namespace GameCore.GameMovement
{
    public sealed class PlayerMovementStateMachine : NPCMovementStateMachine
    {
        [Inject] private IMovement _movement
        {
            set
            {
                _Movement = value;
            }
        }
        [SerializeField] private Transform _cameraTarget;


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
            return MathM.Vector.GetAngleOfVector2(_LocalDirectionOfMoving);
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