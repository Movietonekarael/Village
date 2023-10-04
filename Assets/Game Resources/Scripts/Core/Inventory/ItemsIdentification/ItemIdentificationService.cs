#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Inventory
    {
        public partial class ItemIdentificationService
        {
            [Inject] private readonly IdentificationList _identificationList;
            private static ItemIdentificationService _instance;


            public ItemIdentificationService()
            {
                if (_instance == null)
                    _instance = this;
                else
                    Debug.LogWarning("There is already one ItemIdentifierService. " +
                                   "This one will be ignored.");
            }

#if UNITY_EDITOR
            private static void IdentifyAllItems()
            {
                var allObjectGuids = AssetDatabase.FindAssets("t:IdentificationList");
                foreach (var guid in allObjectGuids)
                {
                    var currentIdentificationList = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as IdentificationList;
                    currentIdentificationList.IdentifyAllItems();
                }
            }
#endif

            public GameItemData GetItemData(uint id)
            {
                return _identificationList.ItemsData[id];
            }
        }
    }
}