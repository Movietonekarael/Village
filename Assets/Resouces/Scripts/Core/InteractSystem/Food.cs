using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Inventory;


namespace GameCore.Interactions
{
    public class Food : MonoBehaviour, IInteractable, IStorable
    {
        [SerializeField] private string _name;
        public string interactionMessage => _name;

        [SerializeField] private InventoryItem _inventoryItem;
        public InventoryItem inventoryItem => _inventoryItem;

        public bool Interact(Interactor interactor)
        {
            var inventory = interactor.inventory;

            if (inventory is not null)
                if (inventory.Push(inventoryItem))
                    Destroy(this.gameObject);
                else
                    return false;

            return true;
        }
    }
}

