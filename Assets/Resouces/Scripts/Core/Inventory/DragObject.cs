using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;

namespace GameCore.Inventory
{
    public class DragObject : MonoBehaviour
    {
        private PlayerController _playerController;
        private Sprite _sprite;
        private RectTransform _canvasRectTransform;

        private Image _image;
        private RectTransform _rectTransform;
        private bool _isDragging = false;

        private void Awake()
        {
            _playerController = PlayerController.instance;
            if (_playerController is null)
                Debug.LogWarning("There is not PlayerController in the scene to attach to PlayerInventory script.");

            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Activate(Sprite sprite, RectTransform canvasRectTransform)
        {
            _sprite = sprite;
            _canvasRectTransform = canvasRectTransform;

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
                var canvasSize = _canvasRectTransform.sizeDelta;
                var screenSize = new Vector2(Screen.width, Screen.height);
                var positionCoefficients = canvasSize / screenSize;

                _rectTransform.anchoredPosition = _playerController.mousePosition * positionCoefficients;
            }
        }

    }
}

