using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;

namespace GameCore.Inventory
{
    public sealed class DragObject : MonoBehaviour
    {
        private InputHandler _inputHandler;
        private Sprite _sprite;
        public RectTransform CanvasRectTransform;

        private Image _image;
        private RectTransform _rectTransform;
        private bool _isDragging = false;

        private void Awake()
        {
            _inputHandler = InputHandler.GetInstance(this.GetType().Name);

            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Activate(Sprite sprite)
        {
            _sprite = sprite;
            _isDragging = true;

            _image.sprite = _sprite;
            _image.color = new(1f, 1f, 1f, 1f);

            UpdatePosition();
        }

        public void Deactivate()
        {
            _isDragging = false;

            _image.color = new(1f, 1f, 1f, 0f);
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_isDragging && _rectTransform is not null)
            {
                _rectTransform.anchoredPosition = _inputHandler.AnchorPosition(_inputHandler.MousePosition);
            }
        }

    }
}

