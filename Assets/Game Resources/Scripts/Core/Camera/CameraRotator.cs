using UnityEngine;
using System;
using Zenject;


namespace GameCore
{
    namespace GameControls
    {
        public sealed class CameraRotator : MonoBehaviour, ICameraAngle, ICameraRotationSubscriber
        {
            [Inject] private readonly ICameraRotatorInput _cameraRotatorInput;

            [SerializeField] private Transform _cameraTarget;

            [SerializeField] private CameraFollowTargetRotator _cameraFollowTarget;

            [SerializeField] private float _gamepadDegreesPerSecondLimit = 60f;

            [Header("Mouse sensitivity")]
            [Tooltip("Default sensitivity is 4")]
            [Range(0f, 10f)]
            [SerializeField] private float _mouseCameraSensitivity = _DEFAULT_MOUSE_SENSITIVITY;
            private float _totalMouseSensitivity;
            private const float _MOUSE_REDUCTION = 0.05f;
            private const float _DEFAULT_MOUSE_SENSITIVITY = 4f;

            public event Action<float> OnCameraAngleChanged;


            private void Awake()
            {
                CalculateMouseSensitivity();
            }

            private void CalculateMouseSensitivity()
            {
                _totalMouseSensitivity = _MOUSE_REDUCTION * _mouseCameraSensitivity / _DEFAULT_MOUSE_SENSITIVITY;
            }

            private void OnEnable()
            {
                SubscribeForCameraRotateInput();
            }

            private void OnDisable()
            {
                UnsubscribeForCameraRotateInput();
            }

            public void SubscribeForCameraRotateInput()
            {
                if (_cameraRotatorInput != null)
                {
                    _cameraRotatorInput.OnCameraRotated += RotateCamera;
                }
                    
            }

            public void UnsubscribeForCameraRotateInput()
            {
                if (_cameraRotatorInput != null)
                {
                    _cameraRotatorInput.OnCameraRotated -= RotateCamera;
                } 
            }

            private void RotateCamera(Vector2 vec, bool isGamepad)
            {
                var rotationModifier = isGamepad ? _gamepadDegreesPerSecondLimit : _totalMouseSensitivity;
                _cameraFollowTarget.InvokeRotation(new Vector2(-vec.y, vec.x) * rotationModifier);
                OnCameraAngleChanged?.Invoke(GetAngleOfGlobalCameraDirection());
            }

            private float GetAngleOfGlobalCameraDirection()
            {
                var angle = _cameraTarget.rotation.eulerAngles.y;
                if (angle > 180)
                    angle -= 360;
                return angle;
            }
        }
    }
}