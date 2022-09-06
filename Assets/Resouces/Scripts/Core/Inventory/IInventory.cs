using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Inventory
{
    public interface IInventory
    {
        public bool Pull(InventoryItem item);
        public bool Pull(InventoryItem item, int i, int j);
        public InventoryItem Push(int i, int j);
    }

}
