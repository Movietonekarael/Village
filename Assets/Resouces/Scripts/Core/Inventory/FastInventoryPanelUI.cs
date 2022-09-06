using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.XR;
using Codice.CM.Common;

namespace GameCore.Inventory
{
    [RequireComponent(typeof(PlayerInventory))]
    public class FastInventoryPanelUI : MonoBehaviour
    {
        private PlayerInventory _playerInventory;

        [SerializeField] private GameObject _fastInventoryCanvas;
        [SerializeField] private GameObject _fastInventoryButtonPrefab;

        [SerializeField] private Vector2Int _itemIndent = new(80, -80);
        [SerializeField] private Vector2Int _itemDelta = new(100, -100);

        [SerializeField] private GameObject _uiEventSystem;
        private EventSystem _eventSystem;

        private void Awake()
        {
            _playerInventory = GetComponent<PlayerInventory>();

            SetUpFastInventory();
            SetUpEventSystem();
        }

        private void SetUpFastInventory()
        {
            for (var i = 0; i < _playerInventory._inventorySize.x; i++)
            {
                var item = Instantiate(_fastInventoryButtonPrefab, _fastInventoryCanvas.transform);
                var itemRect = item.GetComponent<RectTransform>();

                itemRect.anchorMax = new Vector2(.5f, 0f);
                itemRect.anchorMin = new Vector2(.5f, 0f);

                itemRect.anchoredPosition = new Vector2(-_itemDelta.x * (_playerInventory._inventorySize.x / 2 - 1) - 
                                                       (_playerInventory._inventorySize.x % 2 != 0 ? 0 : _itemDelta.x / 2) + 
                                                       _itemDelta.x * i, -_itemIndent.y);

                _playerInventory._fullInventory.fastInventoryButtons[i] = item.GetComponent<InventoryButton>();
                _playerInventory._fullInventory.fastUIImages[i] = _playerInventory._fullInventory.fastInventoryButtons[i].itemImage;
                _playerInventory._fullInventory.fastUIGameObjects[i] = item;

            }
        }

        private void SetUpEventSystem()
        {
            _eventSystem = _uiEventSystem.GetComponent<EventSystem>();
            _eventSystem.firstSelectedGameObject = _playerInventory._fullInventory.fastUIGameObjects[0];
        }

        public void SetCanvasActive(bool isActive)
        {
            _fastInventoryCanvas.SetActive(isActive);
        }

        public void SwitchSelectedItem(int index)
        {
            _eventSystem.SetSelectedGameObject(_playerInventory._fullInventory.fastUIGameObjects[index - 1]);
            _playerInventory._fullInventory.holdingItem = index - 1;
            _playerInventory.ChangeHoldedItem();
        }

    }
}
