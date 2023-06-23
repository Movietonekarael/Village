using GameCore.Inventory;

namespace GameCore.GUI
{
    public interface IOpenedPlayerInventoryView : ISpecificView
    {
        public void SetItemInformation(int position, GameItem item);
    }
}