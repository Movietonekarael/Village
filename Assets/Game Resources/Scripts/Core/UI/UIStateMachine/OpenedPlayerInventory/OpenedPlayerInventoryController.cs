using GameCore.Inventory;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class OpenedPlayerInventoryController : UIController<OpenedPlayerInventoryViewParameters,
                                                                           IOpenedPlayerInventoryController,
                                                                           OpenedPlayerInventoryView,
                                                                           IOpenedPlayerInventoryView>,
                                                              IOpenedPlayerInventoryController
        {
            [Inject] private readonly IInventory _inventory;
            [Inject] private readonly IMovableInventory _movableInventory;
            [Inject] private readonly IDropableInventory _dropableInventory;


            protected override void InitializeParameters(OpenedPlayerInventoryViewParameters parameters)
            {
                parameters.ItemsNumber = _inventory.GetInventorySize();
            }

            protected override void SubscribeForPermanentEvents()
            {
                _inventory.OnItemChanged += ChangeItemInformation;
            }

            protected override void UnsubscribeForPermanentEvents()
            {
                _inventory.OnItemChanged -= ChangeItemInformation;
            }

            protected override void SubscribeForTemporaryEvents() { }
            protected override void UnsubscribeForTemporaryEvents() { }

            private void ChangeItemInformation(int position)
            {
                var item = _inventory.GetGameItem(position);
                _View.SetItemInformation(position, item);
            }

            protected override void OnActivate() { }
            protected override void OnDeactivate() { }

            public void ChangeItemsInInventory(int itemNumber1, int itemNumber2)
            {
                _movableInventory.MoveItem(itemNumber1, itemNumber2);
            }

            public void DropInventoryItem(int itemNumber)
            {
                _dropableInventory.DropItem(itemNumber);
            }
        }
    }
}
