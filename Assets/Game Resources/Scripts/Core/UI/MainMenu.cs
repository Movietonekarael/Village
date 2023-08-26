using GameCore.Animation;
using System;
using Unity.Collections;
using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenu : MonoBehaviour, IReleasable
        {
            [System.Serializable]
            private sealed class MenuButtonWrapper
            {
                public enum ButtonType : uint
                {
                    Singleplayer,
                    Multiplayer,
                    Quit
                }

                public GameObject GameObject;
                public ButtonType Type;
                public bool IsInteractable = true;
            }

            [SerializeField] private int _defaultSelectedButtonIndex = 0;
            private MenuButton _defaultSelectedButton;

            [SerializeField] private GameObject _header;
            private LabelAnimator _headerAnimator;
            private AnimationEventListener _headerAnimationEvent;
            [SerializeField] private MenuButtonWrapper[] _buttons;
            private MenuButton[] _menuButtons;
            

            [HideInInspector] public bool AllowAnimations = false;
            private ButtonAnimator[] _buttonAnimators;

            private const string _ANIMATE_ALL_BUTTONS_KEY = "AnimateAllButtons";

            public event Action OnSinglePlayerButtonPressed;
            public event Action OnMultiplayerButtonPressed;
            public event Action OnQuitApplicationPressed;
            private uint _buttonIndexes;


            private void Awake()
            {
                CacheAnimators();
                CacheAnimationEventListeners();
                CacheButtons();
                CacheSelectedButton();
                SubscribeForEvents();

                ReleaseUselessReferences();
            }

            private void CacheAnimators()
            {
                CacheHeaderAnimator();
                CacheButtonAnimators();
            }

            private void CacheHeaderAnimator()
            {
                _headerAnimator = _header.GetComponent<LabelAnimator>();
            }

            private void CacheButtonAnimators()
            {
                _buttonAnimators = new ButtonAnimator[_buttons.Length];
                for (var i = 0; i < _buttons.Length; i++)
                    _buttonAnimators[i] = _buttons[i].GameObject.GetComponent<ButtonAnimator>();
            }

            private void CacheAnimationEventListeners()
            {
                CacheHeaderAnimationEventListener();
            }

            private void CacheHeaderAnimationEventListener()
            {
                _headerAnimationEvent = _header.GetComponent<AnimationEventListener>();
            }

            private void CacheButtons()
            {
                _menuButtons = new MenuButton[_buttons.Length];

                for (var i = 0; i < _menuButtons.Length; i++)
                    _menuButtons[i] = _buttons[i].GameObject.GetComponent<MenuButton>();
            }

            private void CacheSelectedButton()
            {
                if (_defaultSelectedButtonIndex >= 0 && _defaultSelectedButtonIndex < _buttons.Length)
                    _defaultSelectedButton = _menuButtons[_defaultSelectedButtonIndex];
            }

            private void SubscribeForEvents()
            {
                _headerAnimationEvent.OnAnimationEventAction += AnimateAllButtons;

                if (_defaultSelectedButton != null)
                    _defaultSelectedButton.OnButtonActivated += SelectButton;

                for (var i = 0u; i < _menuButtons.Length; i++)
                {
                    _menuButtons[i].Index = i;
                    _menuButtons[i].SubscribeForClickEvent();
                    _menuButtons[i].OnButtonPressed += ButtonPressed;
                }
            }

            private void UnsubscribeForEvents()
            {
                _headerAnimationEvent.OnAnimationEventAction -= AnimateAllButtons;

                if (_defaultSelectedButton != null)
                    _defaultSelectedButton.OnButtonActivated -= SelectButton;

                foreach (var menuButton in _menuButtons)
                    menuButton.OnButtonPressed -= ButtonPressed;
            }

            private void AnimateAllButtons(FixedString64Bytes key)
            {
                if (key != _ANIMATE_ALL_BUTTONS_KEY)
                    return;

                StartButtonsAnimation();
            }

            private void SelectButton()
            {
                _defaultSelectedButton.SetSelected();
            }

            private void ButtonPressed(uint index)
            {

            }

            private void EstablishButtonIndexes()
            {
                var len = _menuButtons.Length;
                var indexes = new uint[len];
                //Enum.getv
            }

            public void ReleaseUselessReferences()
            {
                _header = null;
                _buttons = null;
            }

            public void StartMainMenu()
            {
                StartHeaderAnimation();
                StartButtonsAnimationIfAnimated();
            }

            private void StartHeaderAnimation()
            {
                _headerAnimator.SetAnimated(AllowAnimations);
            }

            private void StartButtonsAnimationIfAnimated()
            {
                if (AllowAnimations)
                    return;

                StartButtonsAnimation();
            }

            private void StartButtonsAnimation()
            {
                foreach (var animator in _buttonAnimators)
                {
                    animator.Animate(AllowAnimations);
                }
            }

            private void OnDestroy()
            {
                UnsubscribeForEvents();
            }
        }
    }
}