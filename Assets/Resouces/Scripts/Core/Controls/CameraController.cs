using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;


namespace GameCore.GameControls
{
    public class CameraController : MonoBehaviour
    {
        private PlayerController _playerController;

        /// <summary>
        /// Object that camera follows
        /// </summary>
        [SerializeField] private Transform _cameraTargetObjectTransform;

        /// Parameters
        [SerializeField] private float _mouseCensivity = 0.5f;
        [SerializeField] private float _cameraZoomStep = 0.5f;
        [SerializeField] private float _cameraDistance = 6;
        [SerializeField] private float _maxCameraDistance = 15;
        [SerializeField] private float _minCameraDistance = 1;

        /// <summary>
        /// Needs for creation of multiple virtualCameras
        /// </summary>
        private int _cameraNumber;
        private int _maxCameraNumber;

        /// <summary>
        /// Basic virtual camera object
        /// </summary>
        [SerializeField] private GameObject _virtualCameraObjectSample;
        /// <summary>
        /// Array to store created virtual cameras
        /// </summary>
        private GameObject[] _virtualCamerasObjects;

        private void Awake()
        {
            _playerController = PlayerController.instance;
            if (_playerController is null)
                Debug.LogWarning("There is not PlayerController in the scene to attach to CameraController script.");
        }

        private void Start()
        {
            _playerController.OnCameraMoved += RotateCam;
            _playerController.OnMouseScrolled += ZoomCam;
            CreateVirtualCameras();
        }

        private void OnEnable()
        {
            if (_playerController is not null)
            {
                _playerController.OnCameraMoved += RotateCam;
                _playerController.OnMouseScrolled += ZoomCam;
            }
        }

        private void OnDisable()
        {
            if (_playerController is not null)
            {
                _playerController.OnCameraMoved -= RotateCam;
                _playerController.OnMouseScrolled -= ZoomCam;
            }
        }


        /// <summary>
        /// Creates virtual cameras for zooming
        /// </summary>
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

        /// <summary>
        /// Rotates camera and target object
        /// </summary>
        /// <param name="vec">Vector2 of rotation values</param>
        public void RotateCam(Vector2 vec)
        {
            /// Rotates camera and target objects
            _cameraTargetObjectTransform.transform.rotation *= Quaternion.AngleAxis(vec.x * _mouseCensivity, Vector3.up * _mouseCensivity);
            transform.rotation *= Quaternion.AngleAxis(-vec.y * _mouseCensivity, Vector3.right * _mouseCensivity);//Сделать процентный просчёт

            /// Checks if camera crossed it's borders
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

        /// <summary>
        /// Zooms camera to targer object
        /// </summary>
        /// <param name="f">Value of zoom direction</param>
        public void ZoomCam(float f)
        {
            /// Deactivates old camera and activates new camera
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


