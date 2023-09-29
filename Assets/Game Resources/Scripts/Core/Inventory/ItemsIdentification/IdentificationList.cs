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
        public sealed class IdentificationList : ScriptableObject, ISerializationCallbackReceiver
        {
            public static IdentificationList Singleton;
            [HideInInspector]
            [SerializeField] 
            private IdentificationList _singletonHelper;

            [SerializeField] private List<GameItemsList> _itemLists;
            [SerializeField]
            [HideInInspector]
            private SerializableDictionary<uint, GameItemData> _itemsData;
            public Dictionary<uint, GameItemData> ItemsData => _itemsData;
            public void OnAfterDeserialize()
            {
                Singleton = _singletonHelper;
            }

            public void OnBeforeSerialize()
            {
                _singletonHelper = Singleton;
            }

            private void Awake()
            {
                SetSingleton();
                DontDestroyOnLoad(this);
            }

            private void SetSingleton()
            {
                if (Singleton == null)
                {
                    Singleton = this;
                }
                else
                {
                    Debug.LogError("There is already one identification list in the project. " +
                                   "Delete it before creating new or modify it.");
                    DestroyImmediate(this);
                }
            }

            private void OnDestroy()
            {
                Singleton = null;
            }

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