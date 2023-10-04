using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace Inventory
    {
        [CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
        public class GameItemData : ScriptableObject
        {
            public string Name;
            public AssetReferenceGameObject PrefabReference;
            public Sprite Image;
            public int MaxStackNumber = 1;
            [HideInInspector] public uint ItemID;
        }
    }
}