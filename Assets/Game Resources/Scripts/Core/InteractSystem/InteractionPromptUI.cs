using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Interactions
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private Image _canvasImage;
        [SerializeField] private Text _itemNameText;
        [SerializeField] private Text _promptText;

        [Header("Colors")]
        private const float _textColorAmount = 50f / 255f;

        [SerializeField] private Color _visiblePanelColor = new(1f, 1f, 1f, 100f / 255f);
        [SerializeField] private Color _textColor = new(_textColorAmount, _textColorAmount, _textColorAmount, 1f);
        private Color _invisibleColor = new(0f, 0f, 0f, 0f);


        public void SetPrompt(string message)
        {
            _canvasImage.color = _visiblePanelColor;
            _itemNameText.color = _textColor;
            _promptText.color = _textColor;

            _itemNameText.text = message;
        }

        public void DisablePrompt()
        {
            _canvasImage.color = _invisibleColor;
            _itemNameText.color = _invisibleColor;
            _promptText.color = _invisibleColor;
        }
    }
}

