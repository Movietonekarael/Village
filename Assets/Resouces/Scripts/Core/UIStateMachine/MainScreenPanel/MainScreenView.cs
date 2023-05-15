using GameCore.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.GUI
{
    public sealed class MainScreenView : IActivatable, IDeinitializable
    {
        private InstantiateManager _instantiateManager;
        private MainScreenViewParameters _parameters;
        private MainScreenController _controller;

        private const string _CANVAS_NAME = "MainScreenCanvas";
        private GameObject _canvasPrefab => _parameters.CanvasPrefab;
        private GameObject _canvasObject;
        private GameObject _itemCellPrefab => _parameters.ItemCellPrefab;

        private int _numberOfItemsToShow => (int)_parameters.NumberOfItemsToShow;
        private Vector2Int _itemIndent => _parameters.ItemIndent;
        private Vector2Int _itemDelta => _parameters.ItemDelta;

        private Camera _uiCamera;

        private ItemCell[] _itemCells;
        private int _currentActiveButton = 0;


        public MainScreenView(MainScreenArgs args, MainScreenController controller)
        {
            InitializeArguments(args);
            CacheInstantiateManager();
            InstantiateViewElements();
            _controller = controller;
        }

        private void InitializeArguments(MainScreenArgs args)
        {
            _parameters = args.MainScreenViewParameters;
            _uiCamera = args.UiCamera;
        }

        private void CacheInstantiateManager()
        {
            _instantiateManager = InstantiateManager.GetInstance(this.GetType().Name);
        }

        private void InstantiateViewElements()
        {
            InstantiateCanvas();
            var buttons = InstantiateButtons();
            CacheInventoryButtons(buttons);
            Deactivate();
        }

        private void InstantiateCanvas()
        {
            _canvasObject = _instantiateManager.InstantiateObject(_canvasPrefab);
            _canvasObject.name = _CANVAS_NAME;
            var canvas = _canvasObject.GetComponent<Canvas>();
            canvas.worldCamera = _uiCamera;
        }

        private GameObject[] InstantiateButtons()
        {
            var buttons = new GameObject[_numberOfItemsToShow];
            for (var i = 0; i < _numberOfItemsToShow; i++) 
            {
                var itemButton = _instantiateManager.InstantiateObject(_itemCellPrefab, _canvasObject.transform);
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

        public void Activate()
        {
            _canvasObject.SetActive(true);
            EnableActiveButton();
        }

        public void Deactivate()
        {
            _canvasObject.SetActive(false);
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

        public void MoveActiveButtonSelection(int direction)
        {
            DisableActiveButton();
            _currentActiveButton += direction;
            HandleCurrentActiveButtonForOutOfRange();
            EnableActiveButton();
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
            _itemCells?[_currentActiveButton].SetInactive();
        }

        private void EnableActiveButton()
        {
            var itemCell = _itemCells?[_currentActiveButton];
            if (itemCell != null) 
            {
                itemCell.SetActive();
                _controller.SetHoldingItem(_currentActiveButton);
            }
        }

        public void Deinitialize()
        {
            _instantiateManager.DestroyObject(_canvasObject);
        }

        public void SetItemInformation(int position, GameItem item)
        {
            if (position >= _numberOfItemsToShow)
                return;

            _itemCells[position].SetItem(item);
        }
    }
}