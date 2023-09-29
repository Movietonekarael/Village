using GameCore.Inventory;
using System;

namespace GameCore
{
    namespace Inventory
    {
        public sealed class PlayerInventoryWrapper : IInventory, IMovableInventory, IDropableInventory
        {
            private static IInventory _inventory;
            public static IInventory Inventory
            {
                get => _inventory;
                set
                {
                    UnsubscribeForInventoryChange();
                    _inventory = value;
                    SubscribeForInventoryChange();
                }
            }
            public static IMovableInventory MovableInventory;
            public static IDropableInventory DropableInventory;

            public event Action<int> OnItemChanged;


            private static PlayerInventoryWrapper _singleton;
            public PlayerInventoryWrapper()
            {
                _singleton = this;
                SubscribeForInventoryChange();
            }

            private static void SubscribeForInventoryChange()
            {
                if (_inventory is not null && _singleton is not null)
                    _inventory.OnItemChanged += _singleton.InventoryChanged;
            }

            private static void UnsubscribeForInventoryChange()
            {
                if (_inventory is not null && _singleton is not null)
                    _inventory.OnItemChanged -= _singleton.InventoryChanged;
            }

            private void InventoryChanged(int position)
            {
                OnItemChanged?.Invoke(position);
            }

            public void DropItem(int position)
            {
                DropableInventory?.DropItem(position);
            }

            public GameItem GetGameItem(int position)
            {
                return Inventory?.GetGameItem(position);
            }

            public int GetInventorySize()
            {
                var size = Inventory?.GetInventorySize();
                return size ?? 0;
            }

            public void MoveItem(int fromPosition, int toPosition)
            {
                MovableInventory?.MoveItem(fromPosition, toPosition);
            }

            public GameItem Pull(int position)
            {
                return Inventory?.Pull(position);
            }

            public bool Push(ref GameItem item)
            {
                var wasPushed = Inventory?.Push(ref item);
                return wasPushed ?? false;
            }

            public bool Push(ref GameItem item, int position)
            {
                var wasPushed = Inventory?.Push(ref item, position);
                return wasPushed ?? false;
            }
        }
    }
}