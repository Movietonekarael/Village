using GameCore.GameControls;
using GameCore.Inventory;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MainScreenController : UIController<MainScreenViewParameters, 
                                                                IMainScreenController, 
                                                                MainScreenView,
                                                                IMainScreenView>, 
                                                   IMainScreenController
        {
            [Inject] private readonly IInventoryPress _inventoryPress;
            [Inject] private readonly IInventory _inventory;
            [Inject] private readonly IPlayerHoldItem _playerHoldItem;
            private int _indexOfCurrentHoldItem = 0;

            protected override void SubscribeForPermanentEvents() 
            {
                _inventory.OnItemChanged += ChangeItemInformation;
            }

            protected override void UnsubscribeForPermanentEvents() 
            {
                _inventory.OnItemChanged -= ChangeItemInformation;
            }

            protected override void SubscribeForTemporaryEvents()
            {
                _inventoryPress.OnInventoryKeyPressed += SwitchSelectedItem;
                _inventoryPress.OnInventoryArrowPressed += MoveItemSelection;  
            }

            protected override void UnsubscribeForTemporaryEvents()
            {
                _inventoryPress.OnInventoryKeyPressed -= SwitchSelectedItem;
                _inventoryPress.OnInventoryArrowPressed -= MoveItemSelection;
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
                _View.SetActiveButton(index);
            }

            private void MoveItemSelection(int direction)
            {
                _View.MoveActiveButtonSelection(direction);
            }

            private void ChangeItemInformation(int position)
            {
                var item = _inventory.GetGameItem(position);
                _View.SetItemInformation(position, item);
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
                //var item = _inventory.GetGameItem(_indexOfCurrentHoldItem);
                _playerHoldItem.SetItem(_indexOfCurrentHoldItem);
            }
        }
    }
}