using GameCore.Inventory;
using GameCore.Services;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MainScreenView : UIView<MainScreenViewParameters, IMainScreenController, IMainScreenView>, IMainScreenView
        {
            private const string _CANVAS_NAME = "MainScreenCanvas";
            private GameObject _canvasPrefab => _Parameters.CanvasPrefab;
            private GameObject _canvasObject;
            private GameObject _itemCellPrefab => _Parameters.ItemCellPrefab;

            private int _numberOfItemsToShow => (int)_Parameters.NumberOfItemsToShow;
            private Vector2Int _itemIndent => _Parameters.ItemIndent;
            private Vector2Int _itemDelta => _Parameters.ItemDelta;

            [Inject] private readonly InstantiateService _instantiateService;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;
            [Inject] private readonly UiSelectionService _submitService;

            private ItemCell[] _itemCells;
            private int _currentActiveButton = 0;

            public override void Activate()
            {
                _canvasObject.SetActive(true);
                EnableActiveButton();
            }

            public override void Deactivate()
            {
                _canvasObject.SetActive(false);
            }

            public override void Deinitialize()
            {
                _instantiateService.DestroyObject(_canvasObject);
            }

            protected override void InstantiateViewElements()
            {
                InstantiateCanvas();
                var buttons = InstantiateButtons();
                CacheInventoryButtons(buttons);
                Deactivate();
            }

            private void InstantiateCanvas()
            {
                _canvasObject = _instantiateService.InstantiateObject(_canvasPrefab);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            public void SetActiveButton(int index)
            {
                if (!CheckIfActiveButtonIndexWasChanged(index))
                    return;
                if (CheckIfNewButtonIndexOutOfRange(index))
                    return;

                DisableActiveButton();
                _currentActiveButton = index;
                EnableActiveButton();
            }

            public void MoveActiveButtonSelection(int direction)
            {
                DisableActiveButton();
                _currentActiveButton += direction;
                HandleCurrentActiveButtonForOutOfRange();
                EnableActiveButton();
            }

            public void SetItemInformation(int position, GameItem item)
            {
                if (position >= _numberOfItemsToShow)
                    return;

                _itemCells[position].SetItem(item);
            }

            private GameObject[] InstantiateButtons()
            {
                var buttons = new GameObject[_numberOfItemsToShow];
                for (var i = 0; i < _numberOfItemsToShow; i++)
                {
                    var itemButton = _instantiateService.InstantiateObject(_itemCellPrefab, _canvasObject.transform);
                    SetButtonAnchoredPosition(itemButton, i);
                    buttons[i] = itemButton;
                }
                SetActiveButton(0);
                return buttons;
            }

            private void SetButtonAnchoredPosition(GameObject itemButton, int numberOfButton)
            {
                var itemButtonRect = itemButton.GetComponent<RectTransform>();

                itemButtonRect.anchorMax = new Vector2(.5f, 0f);
                itemButtonRect.anchorMin = new Vector2(.5f, 0f);

                itemButtonRect.anchoredPosition = new Vector2(-_itemDelta.x * (_numberOfItemsToShow / 2 - 1) -
                                                       (_numberOfItemsToShow % 2 != 0 ? 0 : _itemDelta.x / 2) +
                                                       _itemDelta.x * numberOfButton, -_itemIndent.y);
            }

            private void CacheInventoryButtons(GameObject[] buttons)
            {
                _itemCells = new ItemCell[_numberOfItemsToShow];
                for (var i = 0; i < _numberOfItemsToShow; i++)
                {
                    _itemCells[i] = buttons[i].GetComponent<ItemCell>();
                }
            }

            private bool CheckIfActiveButtonIndexWasChanged(int index)
            {
                return index != _currentActiveButton;
            }

            private bool CheckIfNewButtonIndexOutOfRange(int index)
            {
                if (index >= _numberOfItemsToShow || index < 0)
                {
                    return true;
                }
                return false;
            }

            private void HandleCurrentActiveButtonForOutOfRange()
            {
                if (_currentActiveButton >= _numberOfItemsToShow)
                {
                    _currentActiveButton %= _numberOfItemsToShow;
                }
                while (_currentActiveButton < 0)
                {
                    _currentActiveButton += _numberOfItemsToShow;
                }
            }

            private void DisableActiveButton()
            {
                _submitService.CurrentSelected = _itemCells?[_currentActiveButton];
            }

            private void EnableActiveButton()
            {
                var itemCell = _itemCells?[_currentActiveButton];
                if (itemCell != null)
                {
                    _submitService.CurrentSelected = itemCell;
                    _Controller.SetActiveItem(_currentActiveButton);
                }
            }
        }
    }
}