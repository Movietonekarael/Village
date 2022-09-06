 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
    public class InventoryItem : ScriptableObject
    {
        new public string name;
        public GameObject prefab;
        public Sprite image;
        public int maxStackNumber = 1;
    }

}

