using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            public abstract class Menu<ButtonTypeT> : MonoBehaviour where ButtonTypeT : Enum
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
                protected ButtonAnimator[] _ButtonAnimators;


                protected abstract void OnCacheAnimators();
                protected abstract void ButtonPressed(uint index);
                protected abstract void OnAwake();
                protected abstract void AdditionalOnDestroy();


                private void Awake()
                {
                    CacheAnimators();
                    CacheButtons();
                    CacheSelectedButton();
                    var _buttonIndexes = CreateComperator();
                    SetButtonIndexes(_buttonIndexes);
                    SubscribeForEvents();
                    OnAwake();
                }

                private void CacheAnimators()
                {
                    CacheButtonAnimators();
                    OnCacheAnimators();
                }

                private void CacheButtonAnimators()
                {
                    _ButtonAnimators = new ButtonAnimator[_buttons.Length];
                    for (var i = 0; i < _buttons.Length; i++)
                        _ButtonAnimators[i] = _buttons[i].GameObject.GetComponent<ButtonAnimator>();
                }

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
                        var buttonType = (ButtonTypeT)(object)enumValues[i];
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
                        _MenuButtons[oneComperator.Index].SetIndex((uint)(object)oneComperator.Type);
                    }
                }

                private void CacheSelectedButton()
                {
                    if (_defaultSelectedButtonIndex >= 0 && _defaultSelectedButtonIndex < _buttons.Length)
                        _defaultSelectedButton = _MenuButtons[_defaultSelectedButtonIndex];
                }

                private void SubscribeForEvents()
                {
                    if (_defaultSelectedButton != null)
                        _defaultSelectedButton.OnButtonActivated += SelectButton;

                    for (var i = 0u; i < _MenuButtons.Length; i++)
                    {
                        _MenuButtons[i].SubscribeForClickEvent();
                        _MenuButtons[i].OnButtonPressed += ButtonPressed;
                    }
                }

                private void UnsubscribeForEvents()
                {
                    if (_defaultSelectedButton != null)
                        _defaultSelectedButton.OnButtonActivated -= SelectButton;

                    foreach (var menuButton in _MenuButtons)
                        menuButton.OnButtonPressed -= ButtonPressed;
                }

                private void SelectButton()
                {
                    _defaultSelectedButton.SetSelected();
                }

                private void OnDestroy()
                {
                    UnsubscribeForEvents();
                    AdditionalOnDestroy();
                }
            }
        }
    }
}