using GameCore.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class OpenedPlayerInventoryView : UIView<OpenedPlayerInventoryViewParameters,
                                                               IOpenedPlayerInventoryController,
                                                               IOpenedPlayerInventoryView>,
                                                        IOpenedPlayerInventoryView
        {
            private const string _CANVAS_NAME = "PlayerInventoryCanvas";
            private GameObject _canvasPrefab => _Parameters.CanvasPrefab;
            private GameObject _canvasObject;
            private GameObject _buttonPrefab => _Parameters.ButtonPrefab;
            private GameObject _dragAndDropPrefab => _Parameters.DragAndDropPrefab;
            private DragObject _dragAndDropObject;

            private int _numberOfItemsInRow => (int)_Parameters.NumberOfItemsInRow;
            private int _itemsNumber => _Parameters.ItemsNumber;
            private Vector2Int _itemIndent => _Parameters.ItemIndent;
            private Vector2Int _itemDelta => _Parameters.ItemDelta;

            [Inject] private readonly EventSystem _eventSystem;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;

            private InventoryButton[][] _inventoryButtons;

            private Vector2Int? _currentButtonHoveredPosition = null;
            private Vector2Int? _holdingButtonImagePosition = null;


            public override void Activate()
            {
                _canvasObject.SetActive(true);
            }

            public override void Deactivate()
            {
                DeactivateDragAndDropObject();
                _canvasObject.SetActive(false);
            }

            public override void Deinitialize()
            {
                UnsubscribeButtonsEvents();
                InstantiateService.DestroyObject(_canvasObject);
            }

            protected override void InstantiateViewElements()
            {
                var buttons = CreateViewElements();
                SetupInventoryButtons(buttons);
            }

            public void SetItemInformation(int position, GameItem item)
            {
                _inventoryButtons[position / _numberOfItemsInRow][position % _numberOfItemsInRow].SetItem(item);
            }

            private GameObject[][] CreateViewElements()
            {
                InstantiateCanvas();
                var buttons = CreateButtons();
                InstantiateDragAndDropObject();
                Deactivate();
                return buttons;
            }

            private void InstantiateCanvas()
            {
                _canvasObject = InstantiateService.InstantiateObject(_canvasPrefab);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            private GameObject[][] CreateButtons()
            {
                var buttons = InitializeButtonsObjectArray();
                InstantiateButtons(buttons);
                return buttons;
            }

            private GameObject[][] InitializeButtonsObjectArray()
            {
                int rowsCount = _itemsNumber / _numberOfItemsInRow;
                int lastRow = _itemsNumber % _numberOfItemsInRow;
                rowsCount += lastRow > 0 ? 1 : 0;
                var buttons = new GameObject[rowsCount][];
                for (var i = 0; i < rowsCount; i++)
                {
                    buttons[i] = new GameObject[lastRow > 0 && i == rowsCount - 1 ? lastRow : _numberOfItemsInRow];
                }
                return buttons;
            }

            private void InstantiateButtons(GameObject[][] buttons)
            {
                for (var i = 0; i < buttons.Length; i++)
                {
                    for (var j = 0; j < buttons[i].Length; j++)
                    {
                        var itemButton = InstantiateService.InstantiateObjectWithInjections(_buttonPrefab, _canvasObject.transform);
                        SetButtonAnchoredPosition(itemButton, i, j);
                        buttons[i][j] = itemButton;
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

            private void SetupInventoryButtons(GameObject[][] buttons)
            {
                CacheIventoryButtons(buttons);
                InitializeButtonsPositions();
                SubscribeButtonsEvents();
            }

            private void CacheIventoryButtons(GameObject[][] buttons)
            {
                _inventoryButtons = new InventoryButton[buttons.Length][];
                for (var i = 0; i < buttons.Length; i++)
                {
                    _inventoryButtons[i] = new InventoryButton[buttons[i].Length];
                    for (var j = 0; j < buttons[i].Length; j++)
                    {
                        _inventoryButtons[i][j] = buttons[i][j].GetComponent<InventoryButton>();
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
                _dragAndDropObject = InstantiateService.InstantiateObjectWithInjections(_dragAndDropPrefab, _canvasObject.transform).GetComponent<DragObject>();
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
                    var sprite = _inventoryButtons[_currentButtonHoveredPosition.Value.y][_currentButtonHoveredPosition.Value.x].ItemImage.sprite;
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

                _Controller.ChangeItemsInInventory(itemNumber1, itemNumber2);
            }

            private void DropInventoryItem()
            {
                var itemNumber = GetNumberFromPosition(_holdingButtonImagePosition.Value);

                _Controller.DropInventoryItem(itemNumber);
            }

            private int GetNumberFromPosition(Vector2Int position)
            {
                var number = position.y * _numberOfItemsInRow + position.x;
                return number;
            }
        }
    }
}