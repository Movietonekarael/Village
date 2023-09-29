namespace GameCore
{
    namespace Inventory
    {
        public interface IMovableInventory : IInventory
        {
            public void MoveItem(int fromPosition, int toPosition);
        }
    }
}