using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameCore.GameControls;
using GameCore.Inventory;
using Zenject;

namespace GameCore.GUI
{
    public sealed class MainScreenController : UIController<MainScreenViewParameters, IMainScreenController, IMainScreenView>, IMainScreenController
    {
        [Inject] private readonly IInventory _inventory;
        [Inject] private readonly PlayerHoldItem _playerHoldItem;
        private int _indexOfCurrentHoldItem = 0;

        protected override void SubscribeForEvents()
        {
            _InputHandler.OnInventoryKeyPressed += SwitchSelectedItem;
            _InputHandler.OnInventoryArrowPressed += MoveItemSelection;
            _inventory.OnItemChanged += ChangeItemInformation;
        }

        protected override void UnsubscribeForEvents()
        {
            _InputHandler.OnInventoryKeyPressed -= SwitchSelectedItem;
            _InputHandler.OnInventoryArrowPressed -= MoveItemSelection;
            _inventory.OnItemChanged -= ChangeItemInformation;
        }

        protected override void InitializeParameters(MainScreenViewParameters parameters) { }
        protected override void OnActivate() { }
        protected override void OnDeactivate() { }

        public void SetActiveItem(int index)
        {
            SetHoldingItem(index);
        }

        private void SwitchSelectedItem(int index)
        {
            _SpecificView.SetActiveButton(index);
        }

        private void MoveItemSelection(int direction)
        {
            _SpecificView.MoveActiveButtonSelection(direction);
        }

        private void ChangeItemInformation(int position)
        {
            var item = _inventory.GetGameItem(position);
            _SpecificView.SetItemInformation(position, item);
            CheckHoldinghItemForUpdate(position);
        }

        private void CheckHoldinghItemForUpdate(int index)
        {
            if (index == _indexOfCurrentHoldItem)
                UpdateHoldingItem();
        }

        private void SetHoldingItem(int index)
        {
            _indexOfCurrentHoldItem = index;
            UpdateHoldingItem();
        }

        private void UpdateHoldingItem()
        {
            var item = _inventory.GetGameItem(_indexOfCurrentHoldItem);
            _playerHoldItem.Item = item;
        }
    }
}