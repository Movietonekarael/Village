using UnityEngine;
using MovietoneMath;
using GameCore.GameControls;
using Zenject;


namespace GameCore
{
    namespace GameMovement
    {
        public sealed class PlayerMovementStateMachine : NPCMovementStateMachine
        {
            public IMovement Movement
            {
                set
                {
                    _Movement = value;
                }
            }

            public ICameraAngle ÑameraAngle;
            private float _cameraDirectionAngle;


            public override void StartMovement()
            {
                base.StartMovement();
                SubscribeForCameraAngleChange();
            }

            private void SubscribeForCameraAngleChange()
            {
                if (ÑameraAngle == null)
                    return;

                ÑameraAngle.OnCameraAngleChanged += ChangeCameraDirectionAngle;
            }

            private void UnsubscribeForCameraAngleChange()
            {
                if (ÑameraAngle == null)
                    return;

                ÑameraAngle.OnCameraAngleChanged -= ChangeCameraDirectionAngle;
            }

            private void ChangeCameraDirectionAngle(float value)
            {
                _cameraDirectionAngle = value;
            }

            protected override void LocalDirectionOfMovingChanged()
            {
                var localMovingDirection = GetAngleOfLocalMovingDirection();
                var angleRotation = CalculateGlobalMovingDirectionAngle(localMovingDirection, _cameraDirectionAngle);
                _GlobalDirectionOfMoving = CalculateGlobalMovingDirection(angleRotation);
                _NeededRotation.eulerAngles = SetNeededRotationDependingOnGlobalMovingVector(angleRotation);


                float GetAngleOfLocalMovingDirection()
                {
                    return MathM.Vector.GetAngleOfVector2(_LocalDirectionOfMoving);
                }

                float CalculateGlobalMovingDirectionAngle(float angle1, float angle2)
                {
                    return angle1 + angle2;
                }

                Vector2 CalculateGlobalMovingDirection(float angleRotation)
                {
                    return MathM.Vector.GetVector2OfAngle(angleRotation);
                }

                Vector3 SetNeededRotationDependingOnGlobalMovingVector(float angleRotation)
                {
                    return new Vector3(.0f, angleRotation, .0f);
                }
            }

            private void OnDestroy()
            {
                UnsubscribeForCameraAngleChange();
            }
        }
    }
}