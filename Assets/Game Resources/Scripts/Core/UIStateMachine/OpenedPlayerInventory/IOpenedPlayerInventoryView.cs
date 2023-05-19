using GameCore.Inventory;

namespace GameCore.GUI
{
    public interface IOpenedPlayerInventoryView
    {
        public void Init(OpenedPlayerInventoryViewParameters parameters, int itemsNumber, OpenedPlayerInventoryController controller);
        public void SetItemInformation(int position, GameItem item);
    }
}