using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameCore.GameControls;
using GameCore.Inventory;

namespace GameCore.GUI
{
    public sealed class MainScreenController : ISubscribable, IDeinitializable, IActivatable
    {
        private MainScreenView _mainScreenView;
        private InputHandler _inputHandler;
        private IInventory _inventory;
        private PlayerHoldItem _playerHoldItem;
        private int _indexOfCurrentHoldItem = 0;

        public MainScreenController(MainScreenArgs args, IInventory inventory, PlayerHoldItem playerHoldItem)
        {
            CacheInputHandler();
            CacheInventory(inventory);
            CachePlayerHoldItem(playerHoldItem);
            InitializeView(args);
            Subscribe();
        }

        private void CacheInputHandler()
        {
            _inputHandler = InputHandler.GetInstance(this.GetType().Name);
        }

        private void CacheInventory(IInventory inventory)
        {
            _inventory = inventory;
        }

        private void CachePlayerHoldItem(PlayerHoldItem playerHoldItem)
        {
            _playerHoldItem = playerHoldItem;
        }

        private void InitializeView(MainScreenArgs args)
        {
            _mainScreenView = new(args, this);
        }

        public void Deinitialize()
        {
            Unsubscribe();
            _mainScreenView.Deinitialize();
            _mainScreenView = null;
        }

        public void Subscribe()
        {
            _inputHandler.OnInventoryKeyPressed += SwitchSelectedItem;
            _inputHandler.OnInventoryArrowPressed += MoveItemSelection;
            _inventory.OnItemChanged += ChangeItemInformation;
        }

        public void Unsubscribe()
        {
            _inputHandler.OnInventoryKeyPressed -= SwitchSelectedItem;
            _inputHandler.OnInventoryArrowPressed -= MoveItemSelection;
            _inventory.OnItemChanged -= ChangeItemInformation;
        }

        private void SwitchSelectedItem(int index)
        {
            _mainScreenView.SetActiveButton(index);
        }

        private void MoveItemSelection(int direction)
        {
            _mainScreenView.MoveActiveButtonSelection(direction);
        }

        public void SetHoldingItem(int index)
        {
            _indexOfCurrentHoldItem = index;
            UpdateHoldingItem();
        }

        private void UpdateHoldingItem()
        {
            var item = _inventory.GetGameItem(_indexOfCurrentHoldItem);
            _playerHoldItem.Item = item;
        }

        private void ChangeItemInformation(int position)
        {
            var item = _inventory.GetGameItem(position);
            _mainScreenView.SetItemInformation(position, item);
            CheckHoldinghItemForUpdate(position);
        }

        private void CheckHoldinghItemForUpdate(int index)
        {
            if (index == _indexOfCurrentHoldItem)
                UpdateHoldingItem();
        }

        public void Activate()
        {
            _mainScreenView.Activate();
        }

        public void Deactivate()
        {
            _mainScreenView.Deactivate();
        }
    }
}