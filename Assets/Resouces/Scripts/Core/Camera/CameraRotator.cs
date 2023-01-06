using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace GameCore.GameControls
{
    [RequireComponent(typeof(InputHandler))]
    public sealed class CameraRotator : MonoBehaviour
    {
        private InputHandler _inputHandler;

        [SerializeField] private CameraFollowTargetRotator _cameraFollowTarget;

        [SerializeField] private float _degreesPerSecond = 60f;

        [Header("Default sensitivity is 4")]
        [Range(0f, 10f)]
        [SerializeField] private float _mouseSensitivity = 4f;
        private float _totalMouseSensitivity;
        private const float _MOUSE_REDUCTION = 0.05f;


        private void Awake()
        {
            _inputHandler = InputHandler.GetInstance("CameraRotator");
            CalculateMouseSensitivity();
        }

        private void CalculateMouseSensitivity()
        {
            _totalMouseSensitivity = _MOUSE_REDUCTION * _mouseSensitivity / 4;
        }

        private void OnEnable()
        {
            _inputHandler.OnCameraRotated += RotateCamera;
        }

        private void OnDisable()
        {
            _inputHandler.OnCameraRotated -= RotateCamera;
        }

        public void RotateCamera(Vector2 vec, bool isGamepad)
        {
            var rotationModifier = isGamepad ? _degreesPerSecond : _totalMouseSensitivity;
            _cameraFollowTarget.InvokeRotation(new Vector2(-vec.y, vec.x) * rotationModifier);
        }
    }
}
