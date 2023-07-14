using GameCore.Inventory;

namespace GameCore
{
    namespace GUI
    {
        public interface IOpenedPlayerInventoryView : ISpecificView
        {
            public void SetItemInformation(int position, GameItem item);
        }
    }
}