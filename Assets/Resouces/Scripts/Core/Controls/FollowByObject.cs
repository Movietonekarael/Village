using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace GameCore.GameControls
{
    //[DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder + 100)]
#if MONOCACHE
    public class FollowByObject : MonoCache
#else
    public class FollowByObject : MonoBehaviour
#endif
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _isOnlyXZ = false;

#if MONOCACHE
        protected override void LateRun()
#else
        private void Update()
#endif
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
                transform.position = _target.position;
        }
    }
}

