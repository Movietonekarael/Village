using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Codice.Client.Common.GameUI;
using System.Threading.Tasks;
using System.Threading;

namespace GameCore.GameControls
{
    [RequireComponent(typeof(InputHandler))]
    public sealed class CameraZoomer : MonoBehaviour
    {
        private InputHandler _inputHandler;
        [SerializeField] private CinemachineMixingCamera _mixingCamera;

        [Header("Parameters:")]
        [SerializeField] private float _minCameraDistance = 1;
        [SerializeField] private float _maxCameraDistance = 15;
        [SerializeField] private float _startingCameraDistance = 6;
        [SerializeField] private float _cameraZoomStep = 0.5f;

        private float _currentDistance;
        private float _currentMixing;


        private float _distanceFrom;
        private float _distanceTo;
        private float _part;

        [SerializeField] private AnimationCurve _curve;


        private void Awake()
        {
            _inputHandler = InputHandler.GetInstance("CameraZoomer");
            CheckMixingCameraChildrenCount();

            InitializeCameraDistance();
        }

        private void CheckMixingCameraChildrenCount()
        {
            if (_mixingCamera.ChildCameras.Length != 2)
            {
                throw new Exception("CameraZoomer need MixingCamera with 2 child cameras.");
            }
        }

        private void InitializeCameraDistance()
        {
            _currentDistance = _startingCameraDistance;
            ApplyCameraDistance();
        }

        private void ApplyCameraDistance()
        {
            CheckDistanceLimits();
            _currentMixing = CalculateMixing();
            ApplyCameraMixing();
        }

        private void CheckDistanceLimits() 
        {
            if (_currentDistance < _minCameraDistance)
            {
                _currentDistance = _minCameraDistance;
            }
            else if (_currentDistance > _maxCameraDistance)
            {
                _currentDistance = _maxCameraDistance;
            }
        }

        private float CalculateMixing()
        {
            return (_currentDistance - _minCameraDistance) / (_maxCameraDistance - _minCameraDistance);
        }

        private void ApplyCameraMixing()
        {
            _mixingCamera.SetWeight(0, _currentMixing);
            _mixingCamera.SetWeight(1, 1f - _currentMixing);
        }

        private void OnEnable()
        {
            _inputHandler.OnCameraZoomed += ZoomCamera;
        }

        private void OnDisable()
        {
            _inputHandler.OnCameraZoomed -= ZoomCamera;
        }

        private void ZoomCamera(float zoomDirection)
        {
            if (zoomDirection < 0)
            {
                _currentDistance -= _cameraZoomStep;
            }
            else if (zoomDirection > 0)
            {
                _currentDistance += _cameraZoomStep;
            }

            HandleDistance();
        }

        private Task _mixingHandler;
        private CancellationTokenSource tokenSource;
        private async void HandleDistance()
        {
            CheckDistanceLimits();
            var targetMixing = CalculateMixing();

            if (_mixingHandler != null && _mixingHandler.IsCompleted != true)
            {
                tokenSource.Cancel();
            }

            tokenSource = new();

            _mixingHandler = HandleMixing(_currentMixing, targetMixing, tokenSource.Token);
            await Task.WhenAll(_mixingHandler);
        }

        private async Task HandleMixing(float fromMixing, float toMixing, CancellationToken token)
        {
            float time = Time.deltaTime;

            while (time <= 1)
            {
                if (token.IsCancellationRequested) { return; }
                time += Time.deltaTime;
                _currentMixing = Mathf.Lerp(fromMixing, toMixing, time);
                ApplyCameraMixing();
                await Task.Yield();
            }
        }
    }
}