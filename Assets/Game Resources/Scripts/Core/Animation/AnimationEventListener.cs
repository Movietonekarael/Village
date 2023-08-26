using System;
using Unity.Collections;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    { 
        public sealed class AnimationEventListener : MonoBehaviour
        {
            public event Action<FixedString64Bytes> OnAnimationEventAction;

            public void EventTriggered(string key)
            {
                OnAnimationEventAction?.Invoke(key);
            }
        }
    }
}