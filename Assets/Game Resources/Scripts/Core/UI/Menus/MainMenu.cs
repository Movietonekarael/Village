using GameCore.Animation;
using System;
using Unity.Collections;
using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            public sealed class MainMenu : Menu<MainMenuButtonType>, IMainMenu
            {
                [SerializeField] private GameObject _header;
                private LabelAnimator _headerAnimator;
                private AnimationEventListener _headerAnimationEvent;

                [HideInInspector] private bool _allowAnimations = false;
                private const string _ANIMATE_ALL_BUTTONS_KEY = "AnimateAllButtons";

                public event Action OnSinglePlayerButtonPressed;
                public event Action OnMultiplayerButtonPressed;
                public event Action OnQuitApplicationPressed;


                protected override void AdditionalOnDestroy() { }


                protected override void OnAwake()
                {
                    CacheAnimationEventListeners();
                    SubscribeForEvents();
                }

                protected override void OnCacheAnimators()
                {
                    CacheHeaderAnimator();
                }
                protected override void ButtonPressed(uint index)
                {
                    var type = (MainMenuButtonType)index;
                    switch (type)
                    {
                        case MainMenuButtonType.Singleplayer:
                            OnSinglePlayerButtonPressed?.Invoke();
                            break;
                        case MainMenuButtonType.Multiplayer:
                            OnMultiplayerButtonPressed?.Invoke();
                            break;
                        case MainMenuButtonType.Quit:
                            OnQuitApplicationPressed?.Invoke();
                            break;
                    }
                }

                private void CacheHeaderAnimator()
                {
                    _headerAnimator = _header.GetComponent<LabelAnimator>();
                }

                private void CacheAnimationEventListeners()
                {
                    CacheHeaderAnimationEventListener();
                }

                private void CacheHeaderAnimationEventListener()
                {
                    _headerAnimationEvent = _header.GetComponent<AnimationEventListener>();
                }

                private void SubscribeForEvents()
                {
                    _headerAnimationEvent.OnAnimationEventAction += AnimateAllButtons;
                }

                private void UnsubscribeForEvents()
                {
                    _headerAnimationEvent.OnAnimationEventAction -= AnimateAllButtons;
                }

                private void AnimateAllButtons(FixedString64Bytes key)
                {
                    if (key != _ANIMATE_ALL_BUTTONS_KEY)
                        return;

                    StartButtonsAnimation();
                }

                public void StartMainMenu()
                {
                    StartHeaderAnimation();
                    StartButtonsAnimationIfAnimated();
                }

                public void SetAnimated(bool isAnimated)
                {
                    _allowAnimations = isAnimated;
                }

                private void StartHeaderAnimation()
                {
                    _headerAnimator.SetAnimated(_allowAnimations);
                }

                private void StartButtonsAnimationIfAnimated()
                {
                    if (_allowAnimations)
                        return;

                    StartButtonsAnimation();
                }

                private void StartButtonsAnimation()
                {
                    foreach (var animator in _ButtonAnimators)
                    {
                        animator.Animate(_allowAnimations);
                    }
                }

                private void OnDestroy()
                {
                    UnsubscribeForEvents();
                }
            }
        }
    }
}