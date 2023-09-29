namespace GameCore
{
    namespace Inventory
    {
        public interface IDropableInventory : IInventory
        {
            public void DropItem(int position);
        }
    }
}