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
    public sealed class MainScreenController : IMainScreenController, ISubscribable, IDeinitializable, IActivatable
    {
        [Inject] private readonly IMainScreenView _mainScreenView;
        [Inject(Id = typeof(MainScreenView))] private readonly IDeinitializable _mainScreenViewDeinitializator;
        [Inject(Id = typeof(MainScreenView))] private readonly IActivatable _mainScreenViewActivator;

        [Inject] private readonly InputHandler _inputHandler;
        [Inject] private readonly IInventory _inventory;
        [Inject] private readonly PlayerHoldItem _playerHoldItem;
        private int _indexOfCurrentHoldItem = 0;

        public void Init(MainScreenViewParameters parameters)
        {
            InitializeView(parameters);
            Subscribe();
        }

        private void InitializeView(MainScreenViewParameters parameters)
        {
            _mainScreenView.Init(parameters, this);
        }

        public void Deinitialize()
        {
            Unsubscribe();
            _mainScreenViewDeinitializator.Deinitialize();
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
            _mainScreenViewActivator.Activate();
        }

        public void Deactivate()
        {
            _mainScreenViewActivator.Deactivate();
        }
    }
}