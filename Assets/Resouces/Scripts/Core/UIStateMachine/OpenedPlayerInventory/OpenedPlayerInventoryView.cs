using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryView : IActivatable, IDeinitializable
    {
        private InstantiateManager _instantiateManager;
        private OpenedPlayerInventoryViewParameters _parameters;
        private OpenedPlayerInventoryController _controller;

        private const string _CANVAS_NAME = "PlayerInventoryCanvas";
        private GameObject _canvasPrefab => _parameters.CanvasPrefab;
        private GameObject _canvasObject;
        private GameObject _buttonPrefab => _parameters.ButtonPrefab;
        private GameObject _dragAndDropPrefab => _parameters.DragAndDropPrefab;
        private DragObject _dragAndDropObject;

        private int _numberOfItemsInRow => (int)_parameters.NumberOfItemsInRow;
        private int _itemsNumber;
        private Vector2Int _itemIndent => _parameters.ItemIndent;
        private Vector2Int _itemDelta => _parameters.ItemDelta;

        private EventSystem _eventSystem;
        private Camera _uiCamera;

        private GameObject[][] _buttons;
        private InventoryButton[][] _inventoryButtons;

        private Vector2Int? _currentButtonHoveredPosition = null;
        private Vector2Int? _holdingButtonImagePosition = null;

        public OpenedPlayerInventoryView(OpenedPlayerInventoryArgs args, OpenedPlayerInventoryController controller)
        {
            InitializeArguments(args);
            CacheInstantiateManager();
            InstantiateViewElements();
            SetupInventoryButtons();
            _controller = controller;
        }

        private void InitializeArguments(OpenedPlayerInventoryArgs args)
        {
            _parameters = args.OpenedPlayerInventoryViewParameters;
            _itemsNumber = args.ItemsNumber;
            _eventSystem = args.EventSystem;
            _uiCamera = args.UiCamera;
        }

        private void CacheInstantiateManager()
        {
            _instantiateManager = InstantiateManager.GetInstance(this.GetType().Name);
        }

        private void InstantiateViewElements()
        {
            InstantiateCanvas();
            CreateButtons();
            InstantiateDragAndDropObject();
            Deactivate();
        }

        private void InstantiateCanvas()
        {
            _canvasObject = _instantiateManager.InstantiateObject(_canvasPrefab);
            _canvasObject.name = _CANVAS_NAME;
            var canvas = _canvasObject.GetComponent<Canvas>();
            canvas.worldCamera = _uiCamera;
        }

        private void CreateButtons()
        {
            InitializeButtonsObjectArray();
            InstantiateButtons();
        }

        private void InitializeButtonsObjectArray()
        {
            int rowsCount = _itemsNumber / _numberOfItemsInRow;
            int lastRow = _itemsNumber % _numberOfItemsInRow;
            rowsCount += lastRow > 0 ? 1 : 0;
            _buttons = new GameObject[rowsCount][];
            for (var i = 0; i < rowsCount; i++)
            {
                _buttons[i] = new GameObject[lastRow > 0 && i == rowsCount - 1 ? lastRow : _numberOfItemsInRow];
            }
        }

        private void InstantiateButtons()
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                for (var j = 0; j < _buttons[i].Length; j++)
                {
                    var itemButton = _instantiateManager.InstantiateObject(_buttonPrefab, _canvasObject.transform);
                    SetButtonAnchoredPosition(itemButton, i, j);
                    _buttons[i][j] = itemButton;
                }
            }
        }

        private void SetButtonAnchoredPosition(GameObject itemButton, int rowNumber, int strokeNumber)
        {
            var itemButtonRect = itemButton.GetComponent<RectTransform>();

            itemButtonRect.anchorMax = new Vector2(0f, 1f);
            itemButtonRect.anchorMin = new Vector2(0f, 1f);

            itemButtonRect.anchoredPosition = new Vector2(_itemIndent.x + _itemDelta.x * strokeNumber, _itemIndent.y + _itemDelta.y * rowNumber);
        }

        private void SetupInventoryButtons()
        {
            CacheIventoryButtons();
            InitializeButtonsPositions();
            SubscribeButtonsEvents();
        }

        private void CacheIventoryButtons()
        {
            _inventoryButtons = new InventoryButton[_buttons.Length][];
            for (var i = 0; i < _buttons.Length; i++)
            {
                _inventoryButtons[i] = new InventoryButton[_buttons[i].Length];
                for (var j = 0; j < _buttons[i].Length; j++)
                {
                    _inventoryButtons[i][j] = _buttons[i][j].GetComponent<InventoryButton>();
                }
            }
        }

        private void InitializeButtonsPositions()
        {
            for (var i = 0; i < _inventoryButtons.Length; i++)
            {
                for (var j = 0; j < _inventoryButtons[i].Length; j++)
                {
                    _inventoryButtons[i][j].Position = new Vector2Int(j, i);
                }
            }
        }

        private void SubscribeButtonsEvents()
        {
            for (var i = 0; i < _inventoryButtons.Length; i++)
            {
                for (var j = 0; j < _inventoryButtons[i].Length; j++)
                {
                    _inventoryButtons[i][j].OnPointerDownEvent += ButtonPointerDown;
                    _inventoryButtons[i][j].OnPointerUpEvent += ButtonPointerUp;
                    _inventoryButtons[i][j].OnPointerEnterEvent += ButtonPointerEnter;
                    _inventoryButtons[i][j].OnPointerExitEvent += ButtonPointerExit;
                }
            }
        }

        private void UnsubscribeButtonsEvents()
        {
            for (var i = 0; i < _inventoryButtons.Length; i++)
            {
                for (var j = 0; j < _inventoryButtons[i].Length; j++)
                {
                    _inventoryButtons[i][j].OnPointerDownEvent -= ButtonPointerDown;
                    _inventoryButtons[i][j].OnPointerUpEvent -= ButtonPointerUp;
                    _inventoryButtons[i][j].OnPointerEnterEvent -= ButtonPointerEnter;
                    _inventoryButtons[i][j].OnPointerExitEvent -= ButtonPointerExit;
                }
            }
        }

        private void InstantiateDragAndDropObject()
        {
            _dragAndDropObject = _instantiateManager.InstantiateObject(_dragAndDropPrefab, _canvasObject.transform).GetComponent<DragObject>();
            _dragAndDropObject.CanvasRectTransform = _canvasObject.GetComponent<RectTransform>();
            DeactivateDragAndDropObject();
        }

        private void ActivateDragAndDropObject(Sprite sprite)
        {
            _dragAndDropObject?.Activate(sprite);
        }

        private void DeactivateDragAndDropObject() 
        {
            _dragAndDropObject?.Deactivate();
        }

        private void ButtonPointerDown()
        {
            if (_currentButtonHoveredPosition != null) 
            {
                var sprite = _inventoryButtons[_currentButtonHoveredPosition.Value.y][_currentButtonHoveredPosition.Value.x].itemImage.sprite;
                if (sprite != null) 
                {
                    _holdingButtonImagePosition = _currentButtonHoveredPosition.Value;
                    ActivateDragAndDropObject(sprite);
                }
            }
        }

        private void ButtonPointerUp() 
        {
            _eventSystem.SetSelectedGameObject(null);
            if (_holdingButtonImagePosition != null)
            {
                if (_currentButtonHoveredPosition != null)
                {
                    ChangeItemsInInventory();
                }
                else
                {
                    DropInventoryItem();
                }
                _holdingButtonImagePosition = null;
                DeactivateDragAndDropObject();
            }
        }

        private void ButtonPointerEnter(Vector2Int buttonPosition)
        {
            _currentButtonHoveredPosition = buttonPosition;
        }

        private void ButtonPointerExit()
        {
            _currentButtonHoveredPosition = null;
        }

        private void ChangeItemsInInventory()
        {
            var itemNumber1 = GetNumberFromPosition(_holdingButtonImagePosition.Value);
            var itemNumber2 = GetNumberFromPosition(_currentButtonHoveredPosition.Value);

            _controller.ChangeItemsInInventory(itemNumber1, itemNumber2);
        }

        private void DropInventoryItem()
        {
            var itemNumber = GetNumberFromPosition(_holdingButtonImagePosition.Value);

            _controller.DropInventoryItem(itemNumber);
        }

        private int GetNumberFromPosition(Vector2Int position)
        {
            var number = position.y * _numberOfItemsInRow + position.x;
            return number;
        }

        public void Activate()
        {
            _canvasObject.SetActive(true);
        }

        public void Deactivate()
        {
            DeactivateDragAndDropObject();
            _canvasObject.SetActive(false);
        }

        public void Deinitialize()
        {
            UnsubscribeButtonsEvents();
            _instantiateManager.DestroyObject(_canvasObject);
        }

        public void SetItemInformation(int position, GameItem item)
        {
            _inventoryButtons[position / _numberOfItemsInRow][position % _numberOfItemsInRow].SetItem(item);
        }
    }
}