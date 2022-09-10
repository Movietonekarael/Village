using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace GameCore.Inventory
{
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerInventoryPanelUI : MonoBehaviour
    {
        private PlayerInventory _playerInventory;

        public GameObject _playerInventoryCanvas;
        [SerializeField] private GameObject _inventoryButtonPrefab;
        private InventoryButton[,] inventoryButton;


        [SerializeField] private Vector2Int _itemIndent = new(80, -80);
        [SerializeField] private Vector2Int _itemDelta = new(100, -100);


        private GameObject _dragAndDropPullInventoryItem;
        private DragObject _dragObject;
        [SerializeField] private GameObject _dragAndDropUIPrefab;

        private RectTransform _playerInventoryRectTransform;


        [SerializeField] private GameObject _uiEventSystem;
        private EventSystem _eventSystem;


        private void Awake()
        {
            _playerInventory = GetComponent<PlayerInventory>();

            SetUpInventoryGrid();
            SetUpDragAndDropInventoryItem();
            SetUpEventSystem();
        }

        private void SetUpInventoryGrid()
        {
            inventoryButton = new InventoryButton[_playerInventory._inventorySize.x, _playerInventory._inventorySize.y];


            for (var i = 0; i < _playerInventory._inventorySize.x; i++)
            {
                for (var j = 0; j < _playerInventory._inventorySize.y; j++)
                {
                    var item = Instantiate(_inventoryButtonPrefab, _playerInventoryCanvas.transform);
                    var itemRect = item.GetComponent<RectTransform>();
                    itemRect.anchoredPosition = new Vector2(_itemIndent.x + i * _itemDelta.x, _itemIndent.y + j * _itemDelta.y);

                    inventoryButton[i, j] = item.GetComponent<InventoryButton>();
                    _playerInventory._fullInventory.inventory[i, j].image = inventoryButton[i, j].itemImage;

                    _playerInventory._fullInventory.inventory[i, j].OnNumberChanged += inventoryButton[i, j].SetNumber;

                    var coordinates = new Vector2Int(i, j);
                    item.GetComponent<Button>().onClick.AddListener(() => InventoryItemClick(coordinates.x, coordinates.y));

                    inventoryButton[i, j].SetUpButton(this, i, j);
                }
            }
        }

        private void SetUpEventSystem()
        {
            _eventSystem = _uiEventSystem.GetComponent<EventSystem>();
        }

        private void SetUpDragAndDropInventoryItem()
        {
            _dragAndDropPullInventoryItem = Instantiate(_dragAndDropUIPrefab, _playerInventoryCanvas.transform);
            _dragAndDropPullInventoryItem.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
            _dragAndDropPullInventoryItem.GetComponent<Image>().raycastTarget = false;
            _dragAndDropPullInventoryItem.SetActive(false);

            _dragObject = _dragAndDropPullInventoryItem.GetComponent<DragObject>();
            _playerInventoryRectTransform = _playerInventoryCanvas.GetComponent<RectTransform>();
        }

        private void InventoryItemClick(int x, int y)
        {
            //Paste here logic of used item
        }

        private Vector2Int? _currentSelectedInventoryItem = null;
        private Vector2Int? _currentDraggingInventoryItem = null;

        private bool _isDragging = false;
        private bool _isStartDragging = false;

        public void ButtonPointerDown()
        {
            if (_currentSelectedInventoryItem is not null && _playerInventory._fullInventory.inventory[_currentSelectedInventoryItem.Value.x, _currentSelectedInventoryItem.Value.y].inventoryItem is not null)
            {
                _isDragging = true;
                _isStartDragging = true;
            }
            _eventSystem.SetSelectedGameObject(null);
        }

        public void ButtonPointerUp()
        {
            if (_isDragging && _currentDraggingInventoryItem is not null)
            {
                _dragAndDropPullInventoryItem.SetActive(false);
                _isDragging = false;
                _isStartDragging = false;
                if (_currentSelectedInventoryItem is not null)
                {
                    _playerInventory.MoveItems(_currentDraggingInventoryItem.Value.x,
                                               _currentDraggingInventoryItem.Value.y,
                                               _currentSelectedInventoryItem.Value.x,
                                               _currentSelectedInventoryItem.Value.y);
                }
                else
                {
                    for (var count = _playerInventory._fullInventory.inventory[_currentDraggingInventoryItem.Value.x, _currentDraggingInventoryItem.Value.y].number; count > 0; count--)
                    {
                        var item = _playerInventory.Pull(_currentDraggingInventoryItem.Value.x, _currentDraggingInventoryItem.Value.y);
                        Instantiate(item.prefab, transform.forward * 0.7f + Vector3.up + transform.position, new Quaternion());
                    }

                }
                _currentDraggingInventoryItem = null;
            }
            else
            {
                _isDragging = false;
                _isStartDragging = false;
            }
        }

        public void SetCurrentSelectedInventoryItem(Vector2Int position)
        {
            if (_isStartDragging && _currentSelectedInventoryItem is not null)
            {
                _dragAndDropPullInventoryItem.SetActive(true);
                _dragObject.Activate(_playerInventory._fullInventory.inventory[_currentSelectedInventoryItem.Value.x, _currentSelectedInventoryItem.Value.y].imageSprite,
                                     _playerInventoryRectTransform);
                _currentDraggingInventoryItem = _currentSelectedInventoryItem;
                _isStartDragging = false;
            }

            if (position.x < 0 || position.y < 0)
                _currentSelectedInventoryItem = null;
            else
                _currentSelectedInventoryItem = position;
        }

        public void SetCurrentSelectedInventoryItem(int x, int y)
        {
            SetCurrentSelectedInventoryItem(new Vector2Int(x, y));
        }
    }
}