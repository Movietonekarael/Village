using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameCore.Inventory;
using UnityEngine.UI;


namespace GameCore.Inventory
{
    public class InventoryButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private PlayerInventoryPanelUI _playerInventoryPanelUI;
        private Vector2Int _position;

        public Image itemImage;
        public Text itemCountText;

        [SerializeField] private uint _number = 0;

        public uint number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                ChangeItemNumberInTextUI();
            }
        }

        private void Awake()
        {
            ChangeItemNumberInTextUI();
        }

        public void SetUpButton(PlayerInventoryPanelUI playerInventoryPanelUI, Vector2Int position)
        {
            _playerInventoryPanelUI = playerInventoryPanelUI;
            _position = position;
        }

        public void SetUpButton(PlayerInventoryPanelUI playerInventoryPanelUI, int x, int y)
        {
            _playerInventoryPanelUI = playerInventoryPanelUI;
            _position = new Vector2Int(x, y);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_playerInventoryPanelUI == null) return;

            _playerInventoryPanelUI.ButtonPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_playerInventoryPanelUI == null) return;

            _playerInventoryPanelUI.ButtonPointerUp();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_playerInventoryPanelUI == null) return;

            _playerInventoryPanelUI.SetCurrentSelectedInventoryItem(_position.x, _position.y);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_playerInventoryPanelUI == null) return;

            _playerInventoryPanelUI.SetCurrentSelectedInventoryItem(-1, -1);
        }

        private void ChangeItemNumberInTextUI()
        {
            if (itemCountText is null)
            {
                Debug.LogWarning("Text reference is not assigned to InventoryButton");
                return;
            }

            if (_number > 0)
                itemCountText.text = _number.ToString();
            else
                itemCountText.text = "";
        }
    }
}

