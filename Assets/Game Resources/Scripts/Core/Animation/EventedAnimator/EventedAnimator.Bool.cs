using System;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public partial class EventedAnimator
        {
            public event Action<int, bool> OnBoolParameterSetted;

            public void SetBool(string name, bool value)
            {
                _animator.SetBool(name, value);
                var id = Animator.StringToHash(name);
                OnBoolParameterSetted?.Invoke(id, value);
            }

            public void SetBool(int id, bool value)
            {
                _animator.SetBool(id, value);
                OnBoolParameterSetted?.Invoke(id, value);
            }
        }
    }
}