using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Inventory
{
    public interface IStorable
    {
        public InventoryItem inventoryItem { get; }
    }
}

