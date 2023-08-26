using GameCore.Animation;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    namespace GUI
    {
        [RequireComponent(typeof(MenuButton))]
        public sealed class ButtonAnimator : CustomUIAnimator
        {
            private AnimationEventListener _eventListener;
            private IMenuButton _button;

            private readonly int _isActiveID = Animator.StringToHash("Active");
            private readonly int _isActivateID = Animator.StringToHash("Activate");


            private const string _SET_BUTTON_ACTIVE_KEY = "SetButtonActive";


            protected override void OnAwake() 
            {
                CacheAnimationEventListener();
                SubscribeForAnimationEvents();
                CacheMenuButton();
            }

            private void CacheAnimationEventListener()
            {
                _eventListener = GetComponent<AnimationEventListener>();
            }

            private void SubscribeForAnimationEvents()
            {
                if (_eventListener == null)
                    return;

                _eventListener.OnAnimationEventAction += SetButtonsActive;
            }

            private void UnsubscribeForAnimationEvents()
            {
                if (_eventListener == null)
                    return;

                _eventListener.OnAnimationEventAction -= SetButtonsActive;
            }

            public void SetButtonsActive(FixedString64Bytes key)
            {
                if (key != _SET_BUTTON_ACTIVE_KEY)
                    return;

                SetActive();
            }

            private void CacheMenuButton()
            {
                _button = GetComponent<IMenuButton>();
            }

            public void Animate(bool isAnimated)
            {
                if (isAnimated) 
                {
                    _Animator.SetBool(_isActivateID, true);
                }
                else
                {
                    SetActive();
                }
            }

            public void SetActive()
            {
                _Animator.SetBool(_isActiveID, true);
                _button.SetActive();
            }

            private void OnDestroy()
            {
                UnsubscribeForAnimationEvents();
            }
        }
    }
}