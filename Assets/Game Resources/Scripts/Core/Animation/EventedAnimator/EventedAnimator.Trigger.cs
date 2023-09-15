using System;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public partial class EventedAnimator
        {
            public event Action<int> OnTriggerParameterSetted;

            public void SetTrigger(string name)
            {
                _animator.SetTrigger(name);
                var id = Animator.StringToHash(name);
                OnTriggerParameterSetted?.Invoke(id);
            }

            public void SetTrigger(int id)
            {
                _animator.SetTrigger(id);
                OnTriggerParameterSetted?.Invoke(id);
            }
        }
    }
}