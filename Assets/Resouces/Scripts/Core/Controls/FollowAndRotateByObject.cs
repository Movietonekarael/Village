using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace GameCore.GameControls
{
    public class FollowAndRotateByObject : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private void Update()
        {
            transform.position = _target.position;

            float verticalAngle = transform.rotation.eulerAngles.x;
            transform.rotation = Quaternion.Euler(verticalAngle, _target.rotation.eulerAngles.y, .0f);
        }
    }
}

