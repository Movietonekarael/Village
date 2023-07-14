using GameCore.Inventory;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class OpenedPlayerInventoryController : UIController<OpenedPlayerInventoryViewParameters,
                                                                           IOpenedPlayerInventoryController,
                                                                           IOpenedPlayerInventoryView>,
                                                              IOpenedPlayerInventoryController
        {
            [Inject] private readonly PlayerInventory _inventory;

            protected override void InitializeParameters(OpenedPlayerInventoryViewParameters parameters)
            {
                parameters.ItemsNumber = _inventory.GetInventorySize();
            }

            protected override void SubscribeForEvents()
            {
                _inventory.OnItemChanged += ChangeItemInformation;
            }

            protected override void UnsubscribeForEvents()
            {
                _inventory.OnItemChanged -= ChangeItemInformation;
            }

            private void ChangeItemInformation(int position)
            {
                var item = _inventory.GetGameItem(position);
                _SpecificView.SetItemInformation(position, item);
            }

            protected override void OnActivate()
            {

            }

            protected override void OnDeactivate()
            {

            }

            public void ChangeItemsInInventory(int itemNumber1, int itemNumber2)
            {
                _inventory.MoveItem(itemNumber1, itemNumber2);
            }

            public void DropInventoryItem(int itemNumber)
            {
                _inventory.DropItem(itemNumber);
            }
        }
    }
}
