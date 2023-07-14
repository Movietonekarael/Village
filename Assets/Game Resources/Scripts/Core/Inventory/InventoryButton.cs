using GameCore.GameControls;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace GameCore
{
    namespace Inventory
    {
        public sealed class InventoryButton : ItemCell, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISubscribable
        {
            [Inject] private readonly InputHandler _inputHandler;

            public Vector2Int Position;

            public event Action OnPointerDownEvent;
            public event Action OnPointerUpEvent;
            public event Action<Vector2Int> OnPointerEnterEvent;
            public event Action OnPointerExitEvent;


            public void OnPointerDown(PointerEventData eventData)
            {
                OnPointerDownEvent?.Invoke();
            }

            private void OnControlsChanged(ControlScheme controlScheme)
            {
                UpPointer();
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                UpPointer();
            }

            private void UpPointer()
            {
                OnPointerUpEvent?.Invoke();
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                OnPointerEnterEvent?.Invoke(Position);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                OnPointerExitEvent?.Invoke();
            }

            public void Subscribe()
            {
                _inputHandler.OnControlSchemeChanged += OnControlsChanged;
            }

            public void Unsubscribe()
            {
                _inputHandler.OnControlSchemeChanged -= OnControlsChanged;
            }

            private void OnDestroy()
            {
                Unsubscribe();
            }
        }
    }
}