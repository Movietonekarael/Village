using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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


        private GameObject[] _fastUIGameObjects;
        private Image[] _fastUIImages;
        private InventoryButton[] _fastInventoryButtons;


        private void Awake()
        {
            _playerInventory = GetComponent<PlayerInventory>();

            SetUpFastInventory();
            SetUpEventSystem();
        }

        private void SetUpFastInventory()
        {
            var inventorySizeX = _playerInventory._inventorySize.x;

            _fastUIGameObjects = new GameObject[inventorySizeX];
            _fastUIImages = new Image[inventorySizeX];
            _fastInventoryButtons = new InventoryButton[inventorySizeX];

            for (var i = 0; i < inventorySizeX; i++)
            {
                var item = Instantiate(_fastInventoryButtonPrefab, _fastInventoryCanvas.transform);
                var itemRect = item.GetComponent<RectTransform>();

                itemRect.anchorMax = new Vector2(.5f, 0f);
                itemRect.anchorMin = new Vector2(.5f, 0f);

                itemRect.anchoredPosition = new Vector2(-_itemDelta.x * (inventorySizeX / 2 - 1) - 
                                                       (inventorySizeX % 2 != 0 ? 0 : _itemDelta.x / 2) + 
                                                       _itemDelta.x * i, -_itemIndent.y);

                _fastInventoryButtons[i] = item.GetComponent<InventoryButton>();
                _fastUIImages[i] = _fastInventoryButtons[i].itemImage;
                _fastUIGameObjects[i] = item;

                _playerInventory._fullInventory.inventory[i, 0].xCoordinate = i;
                _playerInventory._fullInventory.inventory[i, 0].OnImageChanged += ChangeFastImage;
                _playerInventory._fullInventory.inventory[i, 0].OnItemNumberChanged += ChangeNumber;
            }
        }

        private void ChangeFastImage(int x)
        {
            var sprite = _playerInventory._fullInventory.inventory[x, 0].imageSprite;
            if (_fastUIImages[x] is not null)
            {
                _fastUIImages[x].sprite = sprite;
                if (sprite is null)
                    _fastUIImages[x].gameObject.SetActive(false);
                else
                    _fastUIImages[x].gameObject.SetActive(true);
            }
        }

        private void ChangeNumber(int x)
        {
            if (_fastInventoryButtons[x] is not null)
                _fastInventoryButtons[x].SetNumber(_playerInventory._fullInventory.inventory[x, 0].number);
        }

        private void SetUpEventSystem()
        {
            _eventSystem = _uiEventSystem.GetComponent<EventSystem>();
        }

        public void SetCanvasActive(bool isActive)
        {
            _fastInventoryCanvas.SetActive(isActive);

            if (isActive is true)
                _eventSystem.SetSelectedGameObject(_fastUIGameObjects[_playerInventory._fullInventory.holdingItem]);
        }

        public void SwitchSelectedItem(int index)
        {
            _eventSystem.SetSelectedGameObject(_fastUIGameObjects[index - 1]);
            _playerInventory._fullInventory.holdingItem = index - 1;
            _playerInventory.ChangeHoldedItem();
        }

    }
}
