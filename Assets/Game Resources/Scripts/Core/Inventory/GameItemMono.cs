using GameCore.Interactions;
using UnityEngine;


namespace GameCore
{
    namespace Inventory
    {
        public class GameItemMono : MonoBehaviour, IInteractable
        {
            public GameItem Item;
            [SerializeField] private GameItemData _data;
            [SerializeField] private int _number = 1;

            public string InteractionMessage => Item.Name;


            public bool Interact(Interactor interactor)
            {
                return CheckInventoryAndPushItemToIt(interactor);
            }

            private bool CheckInventoryAndPushItemToIt(Interactor interactor)
            {
                var inventory = interactor.Inventory;

                CheckInventory(inventory);
                return PushItemToInventory(inventory);
            }

            private void CheckInventory(IInventory inventory)
            {
                if (inventory == null)
                {
                    throw new System.Exception("No inventory found for interactable item.");
                }
            }

            private bool PushItemToInventory(IInventory inventory)
            {
                if (inventory.Push(ref Item))
                {
                    Destroy(this.gameObject);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void Start()
            {
                InitializeGameItem();
            }

            private void InitializeGameItem()
            {
                Item ??= new(_data, _number);
            }
        }
    }
}