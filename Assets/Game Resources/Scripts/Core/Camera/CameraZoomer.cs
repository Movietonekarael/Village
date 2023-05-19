using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Threading.Tasks;
using System.Threading;
using Zenject;

namespace GameCore.GameControls
{
    public sealed class CameraZoomer : MonoBehaviour
    {
        [Inject] private readonly InputHandler _inputHandler;

        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        private Cinemachine3rdPersonFollow _followComponent;

        [Header("Parameters:")]
        [SerializeField] private float _minCameraDistance = 1;
        [SerializeField] private float _maxCameraDistance = 15;
        [SerializeField] private float _startingCameraDistance = 6;
        [SerializeField] private float _cameraZoomStep = 0.5f;
        private float _mixingZoomStep;
        [SerializeField] private float _cameraZoomStepTime = 1f;

        private float _currentMixing;

        [SerializeField] private AnimationCurve _stepCurve;
        [SerializeField] private AnimationCurve _fullCurve;


        private void Awake()
        {
            Initialize3rdPersonCameraFollow();
            InitializeMixingZoomStep();
            InitializeCameraDistance();
        }

        private void Initialize3rdPersonCameraFollow()
        {
            _followComponent = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        private void InitializeMixingZoomStep()
        {
            _mixingZoomStep = CalculateMixing(_cameraZoomStep);
        }

        private void InitializeCameraDistance()
        {
            CheckDistanceLimits();
            _currentMixing = CalculateMixing(_startingCameraDistance);
            ApplyCameraMixing();
        }

        private void CheckDistanceLimits() 
        {
            if (_startingCameraDistance < _minCameraDistance)
            {
                _startingCameraDistance = _minCameraDistance;
            }
            else if (_startingCameraDistance > _maxCameraDistance)
            {
                _startingCameraDistance = _maxCameraDistance;
            }
        }

        private float CalculateMixing(float distance)
        {
            return (distance - _minCameraDistance) / (_maxCameraDistance - _minCameraDistance);
        }

        private void ApplyCameraMixing()
        {
            _followComponent.CameraDistance = _currentMixing 
                                              * (_maxCameraDistance - _minCameraDistance) 
                                              + _minCameraDistance;
        }

        private void OnEnable()
        {
            _inputHandler.OnCameraZoomed += ZoomCamera;
        }

        private void OnDisable()
        {
            _inputHandler.OnCameraZoomed -= ZoomCamera;
        }

        private async void ZoomCamera(float zoomDirection)
        {
            var deltaMixing = _mixingZoomStep * (zoomDirection > 0 ? 1f : -1f);
            var newTime = 0f;

            while (newTime <= _cameraZoomStepTime)
            {
                var lastTime = newTime;
                newTime += Time.deltaTime;
                newTime = newTime > 1f ? 1f : newTime;
                _currentMixing += deltaMixing
                                  * (_stepCurve.Evaluate(newTime) - _stepCurve.Evaluate(lastTime))
                                  * _fullCurve.Evaluate(_currentMixing);
                CheckMixing();
                ApplyCameraMixing();
                await Task.Yield();
            }
        }

        private void CheckMixing()
        {
            if (_currentMixing < 0)
                _currentMixing = 0;
            else if (_currentMixing > 1)
                _currentMixing = 1;
        }
    }
}