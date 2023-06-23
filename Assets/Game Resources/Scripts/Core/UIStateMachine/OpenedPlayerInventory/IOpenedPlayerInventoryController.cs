namespace GameCore.GUI
{
    public interface IOpenedPlayerInventoryController : ISpecificController
    {
        public void ChangeItemsInInventory(int itemNumber1, int itemNumber2);
        public void DropInventoryItem(int itemNumber);
    }
}