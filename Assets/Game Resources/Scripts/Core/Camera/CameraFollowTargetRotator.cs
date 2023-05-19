using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace GameCore.GameControls
{
    public sealed class CameraFollowTargetRotator : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        private Quaternion _lastFrameParentRotation;

        [SerializeField] private Vector3 _startRotation = new(15, 0, 0);
        private Quaternion _quaternion;

        [SerializeField] private float _minVerticalAngle = 80f;
        [SerializeField] private float _maxVerticalAngle = 280f;
        private const float AVERAGE_VERTICAL_ANGLE = 180f;


        private void Awake()
        {
            CheckParentTransformExistance();
        }

        private void CheckParentTransformExistance()
        {
            if (_parentTransform == null)
            {
                _parentTransform = transform.parent;

                if (_parentTransform != null)
                    LogWarningThatParentTransfornIsNotAsssigned();
                else
                    LogErrorThatParentTransformIsNotFound();
            }
        }

        private void LogWarningThatParentTransfornIsNotAsssigned()
        {
            Debug.LogWarning("Transform component is not assigned to CameraFollowTarget script, " +
                             "so the parent object is taken by default.");
        }

        private void LogErrorThatParentTransformIsNotFound()
        {
            Debug.LogError("No parent Transform is assigned to CameraFollowTarget script " +
                           "and no parent Transform is available.");
        }

        private void Start()
        {
            InitializeLastFrameParentRotation();
            SetupStartingRotation();
        }

        private void InitializeLastFrameParentRotation()
        {
            _lastFrameParentRotation = _parentTransform.rotation;
        }

        private void SetupStartingRotation()
        {
            _quaternion = Quaternion.Euler(_startRotation.x, _startRotation.y, _startRotation.z);
            transform.localRotation = _quaternion;
        }

        private void Update()
        {
            CompensateParentRotation();
            Rotate();
        }

        private void CompensateParentRotation()
        {
            var thisFrameParentRotation = _parentTransform.rotation;
            var thisFrameParentRotationAngleY = thisFrameParentRotation.eulerAngles.y;
            var lastFrameParentRotationAngleY = _lastFrameParentRotation.eulerAngles.y;
            var rotationDifference = lastFrameParentRotationAngleY - thisFrameParentRotationAngleY;
            InvokeRotation(new Vector2(0, rotationDifference)); ;
            _lastFrameParentRotation = thisFrameParentRotation;
        }

        public void InvokeRotation(Vector2 vec)
        {
            Vector2 angles = _quaternion.eulerAngles;
            angles += vec;
            CheckVerticalAngleLimits(ref angles);
            _quaternion = Quaternion.Euler(angles.x, angles.y, 0);
        }

        private void CheckVerticalAngleLimits(ref Vector2 angles)
        {
            if (angles.x > _minVerticalAngle && angles.x < AVERAGE_VERTICAL_ANGLE)
            {
                angles.x = _minVerticalAngle;
            }
            else if (angles.x < _maxVerticalAngle && angles.x > AVERAGE_VERTICAL_ANGLE)
            {
                angles.x = _maxVerticalAngle;
            }
        }

        private void Rotate()
        {
            transform.localRotation = _quaternion;
        }

    }
}
