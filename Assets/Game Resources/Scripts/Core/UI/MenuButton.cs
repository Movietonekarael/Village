using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        [RequireComponent(typeof(UiSelecter))]
        [RequireComponent(typeof(Button))]
        public class MenuButton : MonoBehaviour, IMenuButton
        {
            [Inject] private readonly UiSelectionService _uiSelectionService;

            public event Action OnButtonActivated;
            public event Action<uint> OnButtonPressed;

            private Button _button;

            private uint _index;
            public bool Interactable = true; 
            private ISelectable _selecter;

            private void Awake()
            {
                CacheSelecter();
                CacheButton();
            }

            private void CacheSelecter()
            {
                _selecter = GetComponent<UiSelecter>();
            }

            private void CacheButton()
            {
                _button = GetComponent<Button>();
            }

            public void SubscribeForClickEvent()
            {
                if (_button != null)
                    _button.onClick.AddListener(ButtonPressed);
                else
                    GetComponent<Button>().onClick.AddListener(ButtonPressed);
            }

            private void ButtonPressed()
            {
                OnButtonPressed?.Invoke(_index);
            }

            public void SetActive()
            {
                OnButtonActivated?.Invoke();
                SetInteractable();
            }

            private void SetInteractable()
            {
                _button.interactable = Interactable;
            }

            public void SetSelected()
            {
                _uiSelectionService.CurrentSelected = _selecter;
            }

            public void SetIndex(uint index)
            {
                _index = index;
            }
        }
    }
}