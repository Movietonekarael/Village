namespace GameCore
{
    namespace Inventory
    {
        public sealed class PlayerHoldItemWrapper : IPlayerHoldItem
        {
            public static IPlayerHoldItem PlayerHoldItem;
            
            public void SetItem(GameItem item)
            {
                PlayerHoldItem?.SetItem(item);
            }
        }
    }
}