using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        [RequireComponent(typeof(NetworkAnimator))]
        public sealed class NetworkPlayerAnimator : MonoBehaviour
        {
            private Animator _targetAnimator;
            private Animator _animator;

            private struct AnimatorParameterWrapper
            {
                public int id;
                public AnimatorControllerParameterType Type;
            }


            private AnimatorParameterWrapper[] _animatorParameters;


            private void Awake()
            {
                _animator = GetComponent<Animator>();
                _animator.fireEvents = false;


                var parameters = _animator.parameters;
                _animatorParameters = new AnimatorParameterWrapper[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    _animatorParameters[i].id = parameters[i].nameHash;
                    _animatorParameters[i].Type = parameters[i].type;
                }
            }

            private bool _canUpdate = false;

            public void CopyRuntimeAnimatorControllers(Animator targetAnimator)
            {
                _targetAnimator = targetAnimator;
                _canUpdate = true;
            }

            private void Update()
            {
                if (_canUpdate)
                {
                    UpdateAnimatorParameters();
                }
            }

            private void UpdateAnimatorParameters()
            {
                for (var i = 0; i < _animatorParameters.Length; i++)
                {
                    switch (_animatorParameters[i].Type)
                    {
                        case AnimatorControllerParameterType.Float:
                            CheckFloat(ref _animatorParameters[i]);
                            break;
                        case AnimatorControllerParameterType.Int:
                            CheckInt(ref _animatorParameters[i]);
                            break;
                        case AnimatorControllerParameterType.Bool:
                            CheckBool(ref _animatorParameters[i]);
                            break;
                    }
                }
            }

            private void CheckFloat(ref AnimatorParameterWrapper parameter)
            {
                var currentValue = _animator.GetFloat(parameter.id);
                var value = _targetAnimator.GetFloat(parameter.id);
                if (currentValue != value)
                {
                    _targetAnimator.SetFloat(parameter.id, currentValue);
                }
            }

            private void CheckInt(ref AnimatorParameterWrapper parameter)
            {
                var currentValue = _animator.GetInteger(parameter.id);
                var value = _targetAnimator.GetInteger(parameter.id);
                if (currentValue != value)
                {
                    _targetAnimator.SetInteger(parameter.id, currentValue);
                }
            }

            private void CheckBool(ref AnimatorParameterWrapper parameter)
            {
                var currentValue = _animator.GetBool(parameter.id);
                var value = _targetAnimator.GetBool(parameter.id);
                if (currentValue != value)
                {
                    _targetAnimator.SetBool(parameter.id, currentValue);
                }
            }
        }
    }
}