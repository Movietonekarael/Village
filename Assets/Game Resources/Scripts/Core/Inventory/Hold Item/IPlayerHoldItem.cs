namespace GameCore
{
    namespace Inventory
    {
        public interface IPlayerHoldItem
        {
            public void SetItem(GameItem item);
            public void SetItem(int position);
        }
    }
}