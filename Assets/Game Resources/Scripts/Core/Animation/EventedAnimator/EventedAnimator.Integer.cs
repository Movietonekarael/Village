using System;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public partial class EventedAnimator
        {
            public event Action<int, int> OnIntegerParameterSetted;

            public void SetInteger(string name, int value)
            {
                _animator.SetInteger(name, value);
                var id = Animator.StringToHash(name);
                OnIntegerParameterSetted?.Invoke(id, value);
            }

            public void SetInteger(int id, int value)
            {
                _animator.SetInteger(id, value);
                OnIntegerParameterSetted?.Invoke(id, value);
            }
        }
    }
}