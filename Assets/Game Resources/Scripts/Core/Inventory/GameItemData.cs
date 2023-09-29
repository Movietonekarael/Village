using UnityEngine;


namespace GameCore
{
    namespace Inventory
    {
        [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
        public class GameItemData : ScriptableObject
        {
            public string Name;
            public GameObject Prefab;
            public Sprite Image;
            public int MaxStackNumber = 1;
            [HideInInspector] public uint ItemID;
        }
    }
}