using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Inventory
{
    public interface IInventory
    {
        public bool Push(ref GameItem item);
        public bool Push(ref GameItem item, int position);
        public GameItem Pull(int position);
        public GameItem GetGameItem(int position);

        public event Action<int> OnItemChanged;
    }

}
