using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    namespace Inventory
    {
        [CreateAssetMenu(fileName = "ItemsList", menuName = "Inventory/ItemsList")]
        public sealed class GameItemsList : ScriptableObject
        {
            public List<GameItemData> Items;
        }
    }
}