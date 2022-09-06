using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace GameCore.GameControls
{
    //[DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder + 101)]
#if MONOCACHE
    public class FollowAndRotateByObject : MonoCache
#else
    public class FollowAndRotateByObject : MonoBehaviour
#endif
    {
        [SerializeField] private Transform _target;
#if MONOCACHE
        protected override void LateRun()
#else
        private void Update()
#endif
        {
            transform.position = _target.position;

            float verticalAngle = transform.rotation.eulerAngles.x;
            transform.rotation = Quaternion.Euler(verticalAngle, _target.rotation.eulerAngles.y, .0f);
        }
    }
}

