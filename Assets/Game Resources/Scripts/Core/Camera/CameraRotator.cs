using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Zenject;

namespace GameCore.GameControls
{
    public sealed class CameraRotator : MonoBehaviour
    {
        [Inject] private readonly ICameraRotator _cameraRotator;

        [SerializeField] private CameraFollowTargetRotator _cameraFollowTarget;

        [SerializeField] private float _gamepadDegreesPerSecondLimit = 60f;

        [Header("Mouse sensitivity")]
        [Tooltip("Default sensitivity is 4")]
        [Range(0f, 10f)]
        [SerializeField] private float _mouseCameraSensitivity = 4f;
        private float _totalMouseSensitivity;
        private const float _MOUSE_REDUCTION = 0.05f;


        private void Awake()
        {
            CalculateMouseSensitivity();
        }

        private void CalculateMouseSensitivity()
        {
            _totalMouseSensitivity = _MOUSE_REDUCTION * _mouseCameraSensitivity / 4;
        }

        private void OnEnable()
        {
            _cameraRotator.OnCameraRotated += RotateCamera;
        }

        private void OnDisable()
        {
            _cameraRotator.OnCameraRotated -= RotateCamera;
        }

        public void RotateCamera(Vector2 vec, bool isGamepad)
        {
            var rotationModifier = isGamepad ? _gamepadDegreesPerSecondLimit : _totalMouseSensitivity;
            _cameraFollowTarget.InvokeRotation(new Vector2(-vec.y, vec.x) * rotationModifier);
        }
    }
}
