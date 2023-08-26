using GameCore.Animation;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        public abstract class ButtonMenu<ButtonTypeT> : MonoBehaviour, IReleasable where ButtonTypeT : Enum, IConvertible
        {
            protected struct IndexTypeComperator
            {
                public ButtonTypeT Type;
                public uint Index;

                public IndexTypeComperator(ButtonTypeT type, uint index)
                {
                    Type = type;
                    Index = index;
                }
            }

            [System.Serializable]
            protected sealed class MenuButtonWrapper
            {
                public GameObject GameObject;
                public ButtonTypeT Type;
                public bool IsInteractable = true;
            }

            [SerializeField] private int _defaultSelectedButtonIndex = 0;
            private IMenuButton _defaultSelectedButton;

            [SerializeField] private MenuButtonWrapper[] _buttons;
            protected IMenuButton[] _MenuButtons;
            private ButtonAnimator[] _buttonAnimators;


            private void Awake()
            {
                CacheAnimators();
                CacheButtons();
                CacheSelectedButton();
                var _buttonIndexes = CreateComperator();
                SetButtonIndexes(_buttonIndexes);
                SubscribeForEvents();
                ReleaseUselessReferences();
                OnAwake();
            }

            protected abstract void OnAwake();

            private void CacheAnimators()
            {
                CacheButtonAnimators();
                OnCacheAnimators();
            }

            private void CacheButtonAnimators()
            {
                _buttonAnimators = new ButtonAnimator[_buttons.Length];
                for (var i = 0; i < _buttons.Length; i++)
                    _buttonAnimators[i] = _buttons[i].GameObject.GetComponent<ButtonAnimator>();
            }

            protected abstract void OnCacheAnimators();

            private void CacheButtons()
            {
                _MenuButtons = new IMenuButton[_buttons.Length];

                for (var i = 0; i < _MenuButtons.Length; i++)
                    _MenuButtons[i] = _buttons[i].GameObject.GetComponent<IMenuButton>();
            }

            private IndexTypeComperator[] CreateComperator()
            {
                var indexComperator = new List<IndexTypeComperator>();
                var createdTypesHashSet = new HashSet<ButtonTypeT>();

                for (var i = 0u; i < _buttons.Length; i++)
                {
                    var buttonType = _buttons[i].Type;
                    if (!createdTypesHashSet.Contains(buttonType))
                    {
                        createdTypesHashSet.Add(buttonType);
                        var oneComperator = new IndexTypeComperator(buttonType, i);
                        indexComperator.Add(oneComperator);
                    }
                    else
                    {
                        Debug.LogWarning("There are more than one button for operating " +
                                         $"with type {buttonType}. Skipping extra buttons.");
                    }
                }

                CheckCreatedHashSet(createdTypesHashSet);

                return indexComperator.ToArray();
            }

            private void CheckCreatedHashSet(HashSet<ButtonTypeT> createdTypesHashSet)
            {
                var enumValues = Enum.GetValues(typeof(ButtonTypeT)) as uint[];

                for (var i = 0; i < enumValues.Length; i++)
                {
                    var buttonType = (ButtonTypeT)enumValues[i];
                    if (!createdTypesHashSet.Contains(buttonType))
                    {
                        Debug.LogWarning($"There are no buttons to operate with {buttonType} event.");
                    }
                }
            }

            private void SetButtonIndexes(IndexTypeComperator[] indexComperator)
            {
                for (var i = 0; i < indexComperator.Length; i++)
                {
                    var oneComperator = indexComperator[i];
                    //var g = Enum.GetUnderlyingType(oneComperator.Type);
                    _MenuButtons[oneComperator.Index].SetIndex(oneComperator.Type);
                }
            }

            private void CacheSelectedButton()
            {
                if (_defaultSelectedButtonIndex >= 0 && _defaultSelectedButtonIndex < _buttons.Length)
                    _defaultSelectedButton = _MenuButtons[_defaultSelectedButtonIndex];
            }

            public void ReleaseUselessReferences()
            {
                _buttons = null;
            }

            protected abstract void OnReleaseUselessReferences();

            private void SubscribeForEvents()
            {
                if (_defaultSelectedButton != null)
                    _defaultSelectedButton.OnButtonActivated += SelectButton;
            }

            private void UnsubscribeForEvents()
            {
                if (_defaultSelectedButton != null)
                    _defaultSelectedButton.OnButtonActivated -= SelectButton;
            }

            private void SelectButton()
            {
                _defaultSelectedButton.SetSelected();
            }
        }

        public enum MultiplayerMenuButtonType : uint
        {
            HostServer,
            ConnectToSeerver,
            Back
        }

        public sealed class MultiplayerMenu : ButtonMenu<MultiplayerMenuButtonType>
        {
            protected override void OnAwake()
            {
                throw new NotImplementedException();
            }

            protected override void OnCacheAnimators()
            {
                throw new NotImplementedException();
            }

            protected override void OnReleaseUselessReferences()
            {
                throw new NotImplementedException();
            }
        }

        public enum MainMenuButtonType : uint
        {
            Singleplayer,
            Multiplayer,
            Quit
        }

        public sealed class MainMenu : ButtonMenu<MainMenuButtonType>, IMainMenu
        {




            [SerializeField] private GameObject _header;
            private LabelAnimator _headerAnimator;
            private AnimationEventListener _headerAnimationEvent;
            
            
            

            [HideInInspector] private bool _allowAnimations = false;

            private const string _ANIMATE_ALL_BUTTONS_KEY = "AnimateAllButtons";

            public event Action OnSinglePlayerButtonPressed;
            public event Action OnMultiplayerButtonPressed;
            public event Action OnQuitApplicationPressed;


            protected override void OnAwake()
            {
                CacheAnimationEventListeners();
                SubscribeForEvents();
            }

            protected override void OnCacheAnimators()
            {
                CacheHeaderAnimator();
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

                for (var i = 0u; i < _MenuButtons.Length; i++)
                {
                    _MenuButtons[i].SubscribeForClickEvent();
                    _MenuButtons[i].OnButtonPressed += ButtonPressed;
                }
            }

            private void UnsubscribeForEvents()
            {
                _headerAnimationEvent.OnAnimationEventAction -= AnimateAllButtons;

                foreach (var menuButton in _MenuButtons)
                    menuButton.OnButtonPressed -= ButtonPressed;
            }

            private void AnimateAllButtons(FixedString64Bytes key)
            {
                if (key != _ANIMATE_ALL_BUTTONS_KEY)
                    return;

                StartButtonsAnimation();
            }

            private void ButtonPressed(uint index)
            {
                var type = (MainMenuButtonType)index;
                switch(type) 
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



            protected override void OnReleaseUselessReferences()
            {
                _header = null;
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
                foreach (var animator in _buttonAnimators)
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