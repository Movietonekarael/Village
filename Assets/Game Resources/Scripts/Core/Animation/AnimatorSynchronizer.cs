using GameCore.GUI;
using UnityEngine;

namespace GameCore
{
    namespace Animation
    {
        public sealed class AnimatorSynchronizer : MonoBehaviour, IIdEventedAnimator, ISerializedInterfaceBehaviour
        {
            [SerializeField]
            [RequireInterface(typeof(IEventedAnimator))]
            private UnityEngine.Object _outputEventedAnimatorBase;

            private IEventedAnimator _outputEventedAnimator;
            private IEventedAnimatorEvents _inputEventedAnimator;

            public IEventedAnimatorEvents InputEventedAnimator
            {
                get
                {
                    return _inputEventedAnimator;
                }
                set
                {
                    UnsubscribeForAnimatorEvents();
                    _inputEventedAnimator = value;
                    SubscribeForAnimatorEvents();
                }
            }


            private void Awake()
            {
                SetupSerializedInterfaces();
                CheckForOutputAnimator();
            }

            public void SetupSerializedInterfaces()
            {
                _outputEventedAnimator = _outputEventedAnimatorBase as IEventedAnimator;
            }

            private void CheckForOutputAnimator()
            {
                if (_outputEventedAnimator == null)
                {
                    Debug.LogWarning("[Warning]: EventedAnimator wasn't attached to evented animator. Trying to find this component on the gameObject.");
                    _outputEventedAnimator = GetComponent<IEventedAnimator>();

                    CheckIfAnimatorNowWasAttached();
                }
            }

            private void CheckIfAnimatorNowWasAttached()
            {
                if (_outputEventedAnimator != null)
                {
                    Debug.LogWarning("[Warning]: EventedAnimator was found. Keep going.");
                }
                else
                {
                    Debug.LogError("[Error]: EventedAnimator wasn't found. Please attach manually in editor.");
                }
            }

            private void SubscribeForAnimatorEvents()
            {
                if (_inputEventedAnimator is null)
                    return;

                Debug.Log("Subscribing");
                _inputEventedAnimator.OnBoolParameterSetted += SetBool;
                _inputEventedAnimator.OnFloatParameterSetted += SetFloat;
                _inputEventedAnimator.OnFloatWithDampParameterSetted += SetFloat;
                _inputEventedAnimator.OnIntegerParameterSetted += SetInteger;
                _inputEventedAnimator.OnTriggerParameterSetted += SetTrigger;
            }

            private void UnsubscribeForAnimatorEvents()
            {
                if (_inputEventedAnimator is null)
                    return;

                Debug.Log("Unsubscribing");
                _inputEventedAnimator.OnBoolParameterSetted -= SetBool;
                _inputEventedAnimator.OnFloatParameterSetted -= SetFloat;
                _inputEventedAnimator.OnFloatWithDampParameterSetted -= SetFloat;
                _inputEventedAnimator.OnIntegerParameterSetted -= SetInteger;
                _inputEventedAnimator.OnTriggerParameterSetted -= SetTrigger;
            }

            public void SetBool(int id, bool value)
            {
                Debug.Log("Calling bool");
                _outputEventedAnimator.SetBool(id, value);
            }

            public void SetFloat(int id, float value)
            {
                _outputEventedAnimator.SetFloat(id, value);
            }

            public void SetFloat(int id, float value, float dampTime, float deltaTime)
            {
                _outputEventedAnimator.SetFloat(id, value, dampTime, deltaTime);
            }

            public void SetInteger(int id, int value)
            {
                _outputEventedAnimator.SetInteger(id, value);
            }

            public void SetTrigger(int id)
            {
                _outputEventedAnimator.SetTrigger(id);
            }
        }
    }
}