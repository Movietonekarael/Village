using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;


namespace GameCore.GameControls
{
    public class CameraController : MonoBehaviour
    {
        private InputHandler _inputHandler;

        [SerializeField] private Transform _cameraTargetObjectTransform;


        [SerializeField] private float _degreesPerSecond = 60f;
        [SerializeField] private float _mouseSensitivity = 0.05f;
        [SerializeField] private float _cameraZoomStep = 0.5f;
        [SerializeField] private float _cameraDistance = 6;
        [SerializeField] private float _maxCameraDistance = 15;
        [SerializeField] private float _minCameraDistance = 1;


        private int _cameraNumber;
        private int _maxCameraNumber;

        [SerializeField] private GameObject _virtualCameraObjectSample;
        [SerializeField] private GameObject _camera;
        private GameObject[] _virtualCamerasObjects;

        private void Awake()
        {
            _inputHandler = InputHandler.Instance;
            if (_inputHandler is null)
                Debug.LogWarning("There is not InputHandler in the scene to attach to CameraController script.");
        }

        private void Start()
        {
            _inputHandler.OnCameraRotated += RotateCam;
            _inputHandler.OnCameraZoomed += ZoomCam;
            CreateVirtualCameras();
        }

        private void OnEnable()
        {
            if (_inputHandler is not null)
            {
                _inputHandler.OnCameraRotated += RotateCam;
                _inputHandler.OnCameraZoomed += ZoomCam;
            }
        }

        private void OnDisable()
        {
            if (_inputHandler is not null)
            {
                _inputHandler.OnCameraRotated -= RotateCam;
                _inputHandler.OnCameraZoomed -= ZoomCam;
            }
        }


        private void CreateVirtualCameras()
        {
            int camerasCountLeft = (int)((_cameraDistance - _minCameraDistance) / _cameraZoomStep);
            if ((_cameraDistance - _minCameraDistance) % _cameraZoomStep != .0f)
                camerasCountLeft++;

            int camerasCountRight = (int)((_maxCameraDistance - _cameraDistance) / _cameraZoomStep);
            if ((_maxCameraDistance - _cameraDistance) % _cameraZoomStep != .0f)
                camerasCountRight++;


            _virtualCamerasObjects = new GameObject[camerasCountLeft + 1 + camerasCountRight];
            _maxCameraNumber = _virtualCamerasObjects.Length - 1;
            for (int i = 0; i < camerasCountLeft; i++)
            {
                _virtualCamerasObjects[i] = Instantiate(_virtualCameraObjectSample, _virtualCameraObjectSample.transform.parent);
            }
            _virtualCamerasObjects[camerasCountLeft] = _virtualCameraObjectSample;
            _cameraNumber = camerasCountLeft;
            for (int i = camerasCountLeft + 1; i < _virtualCamerasObjects.Length; i++)
            {
                _virtualCamerasObjects[i] = Instantiate(_virtualCameraObjectSample, _virtualCameraObjectSample.transform.parent);
            }

            Cinemachine3rdPersonFollow[] VirtualCameras = new Cinemachine3rdPersonFollow[_virtualCamerasObjects.Length];
            for (int i = 0; i < _virtualCamerasObjects.Length; i++)
            {
                VirtualCameras[i] = _virtualCamerasObjects[i].GetComponent<CinemachineVirtualCamera>()
                    .GetCinemachineComponent(CinemachineCore.Stage.Body) as Cinemachine3rdPersonFollow;
                VirtualCameras[i].CameraDistance = _cameraDistance + (-camerasCountLeft + i) * _cameraZoomStep;
            }

            foreach (GameObject obj in _virtualCamerasObjects)
            {
                obj.SetActive(false);
            }

            _virtualCameraObjectSample.SetActive(true);
        }

        public void RotateCam(Vector2 vec, bool isGamepad)
        {
            var rotationModifier = isGamepad ? _degreesPerSecond : _mouseSensitivity;

            _cameraTargetObjectTransform.transform.rotation *= Quaternion.AngleAxis(vec.x * rotationModifier, Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(-vec.y * rotationModifier, Vector3.right);


            Vector3 rotationValue = transform.rotation.eulerAngles;
            if (rotationValue.x > 85.0f && rotationValue.x < 100.0f)
            {
                transform.rotation = Quaternion.Euler(85.0f, rotationValue.y, rotationValue.z);
            }
            else if (rotationValue.x > 260.0f && rotationValue.x < 275.0f)
            {
                transform.rotation = Quaternion.Euler(275.0f, rotationValue.y, rotationValue.z);
            }
        }

        public void ZoomCam(float f)
        {
            _virtualCamerasObjects[_cameraNumber].SetActive(false);

            if (f < 0 && _cameraNumber < _maxCameraNumber)
            {
                _cameraNumber++;
            }
            else if (f > 0 && _cameraNumber > 0)
            {
                _cameraNumber--;
            }

            _virtualCamerasObjects[_cameraNumber].SetActive(true);
        }


    }

}


