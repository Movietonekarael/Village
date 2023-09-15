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
            [Inject]
            public IMovement Movement
            {
                set
                {
                    _Movement = value;
                }
            }

            [RequireInterface(typeof(ICameraAngle))]
            public UnityEngine.Object CameraAngleBase;
            private ICameraAngle _cameraAngle { get => CameraAngleBase as ICameraAngle; }
            private float _cameraDirectionAngle;


            protected override void Awake()
            {
                base.Awake();
                SubscribeForCameraAngleChange();
            }

            public void SubscribeForCameraAngleChange()
            {
                if (_cameraAngle == null)
                    return;

                _cameraAngle.OnCameraAngleChanged += ChangeCameraDirectionAngle;
            }

            public void UnsubscribeForCameraAngleChange()
            {
                if (_cameraAngle == null)
                    return;

                _cameraAngle.OnCameraAngleChanged -= ChangeCameraDirectionAngle;
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
            }

            private float GetAngleOfLocalMovingDirection()
            {
                return MathM.Vector.GetAngleOfVector2(_LocalDirectionOfMoving);
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

            private void OnDestroy()
            {
                UnsubscribeForCameraAngleChange();
            }
        }
    }
}