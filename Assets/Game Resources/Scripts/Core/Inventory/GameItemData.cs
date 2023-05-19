using GameCore.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
    public class GameItemData : ScriptableObject
    {
        public string Name;
        public GameObject Prefab;
        public Sprite Image;
        public int MaxStackNumber = 1;
    }
}

