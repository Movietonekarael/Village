using GameCore.GameControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameCore.Inventory
{
    public sealed class InventoryButton : ItemCell, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Vector2Int Position;

        public event Action OnPointerDownEvent;
        public event Action OnPointerUpEvent;
        public event Action<Vector2Int> OnPointerEnterEvent;
        public event Action OnPointerExitEvent;

        protected override void Awake()
        {
            base.Awake();
            InputHandler.GetInstance(this.GetType().Name).OnControlSchemeChanged += OnControlsChanged;
        }

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
    }
}

