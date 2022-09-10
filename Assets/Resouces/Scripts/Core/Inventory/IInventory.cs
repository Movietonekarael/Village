using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Inventory
{
    public interface IInventory
    {
        public bool Push(InventoryItem item);
        public bool Push(InventoryItem item, int i, int j);
        public InventoryItem Pull(int i, int j);
    }

}
