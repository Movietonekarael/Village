using System;
using System.Collections.Generic;
using GameCore.Collections;
using UnityEngine;
using Random = System.Random;

namespace GameCore
{
    namespace Inventory
    {
        [CreateAssetMenu(fileName = "IdentificationList", menuName = "Inventory/IdentificationList")]
        public sealed class IdentificationList : ScriptableObject
        {
            [SerializeField] private List<GameItemsList> _itemLists;
            [SerializeField]
            [HideInInspector]
            private SerializableDictionary<uint, GameItemData> _itemsData;
            public Dictionary<uint, GameItemData> ItemsData => _itemsData;


            public void IdentifyAllItems()
            {
                var nextId = 0u;
                _itemsData = new();

                foreach (var itemList in _itemLists)
                {
                    foreach (var itemData in itemList.Items)
                    {
                        itemData.ItemID = nextId;
                        _itemsData.Add(itemData.ItemID, itemData);
                        nextId++;
                    }
                }
            }
        }
    }
}