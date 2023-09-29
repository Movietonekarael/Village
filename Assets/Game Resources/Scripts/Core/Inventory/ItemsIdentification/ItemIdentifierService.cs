using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    namespace Inventory
    {
        public static partial class ItemIdentifierService
        {
            private static void IdentifyAllItems()
            {
                var identificationList = IdentificationList.Singleton;

                if (identificationList != null ) 
                    identificationList.IdentifyAllItems();
                else
                    Debug.LogWarning("There is no identification list in the project. Can't identify items.");
            }

            public static GameItemData GetItemData(uint id)
            {
                var identificationList = IdentificationList.Singleton;

                if (identificationList != null)
                    return IdentificationList.Singleton.ItemsData[id];
                else
                    return null; 
            }
        }
    }
}