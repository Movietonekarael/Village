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
        public class MenuButton : MonoBehaviour
        {
            [Inject] private readonly UiSelectionService _uiSelectionService;

            public event Action OnButtonActivated;
            public event Action<uint> OnButtonPressed;

            private Button _button;

            [HideInInspector] public uint Index;
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
                    _button.onClick.AddListener(() => { OnButtonPressed?.Invoke(Index); });
                else
                    GetComponent<Button>().onClick.AddListener(() => { OnButtonPressed?.Invoke(Index); });
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
        }
    }
}