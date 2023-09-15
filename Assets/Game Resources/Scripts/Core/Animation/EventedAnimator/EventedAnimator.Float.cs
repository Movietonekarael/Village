using System;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public partial class EventedAnimator
        {
            public event Action<int, float> OnFloatParameterSetted;
            public event Action<int, float, float, float> OnFloatWithDampParameterSetted;

            public void SetFloat(string name, float value)
            {
                _animator.SetFloat(name, value);
                var id = Animator.StringToHash(name);
                OnFloatParameterSetted?.Invoke(id, value);
            }

            public void SetFloat(int id, float value)
            {
                _animator.SetFloat(id, value);
                OnFloatParameterSetted?.Invoke(id, value);
            }

            public void SetFloat(string name, float value, float dampTime, float deltaTime)
            {
                _animator.SetFloat(name, value, dampTime, deltaTime);
                var id = Animator.StringToHash(name);
                OnFloatWithDampParameterSetted?.Invoke(id, value, dampTime, deltaTime);
            }

            public void SetFloat(int id, float value, float dampTime, float deltaTime)
            {
                _animator.SetFloat(id, value, dampTime, deltaTime);
                OnFloatWithDampParameterSetted?.Invoke(id, value, dampTime, deltaTime);
            }
        }
    }
}