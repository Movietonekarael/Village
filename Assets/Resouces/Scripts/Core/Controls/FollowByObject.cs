using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace GameCore.GameControls
{
    public class FollowByObject : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _isOnlyXZ = false;

        private void Awake()
        {
            UpdatePosition();
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_isOnlyXZ)
            {
                var targetPosition = _target.position;
                var objectPosition = transform.position;
                objectPosition.x = targetPosition.x;
                objectPosition.z = targetPosition.z;
                transform.position = objectPosition;
            }
            else
            {
                transform.position = _target.position;
            }
                
        }
    }
}

