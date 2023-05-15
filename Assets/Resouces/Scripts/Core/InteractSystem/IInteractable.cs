using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.Interactions
{
    public interface IInteractable
    {
        public string InteractionMessage { get; }

        public bool Interact(Interactor interactor);
    }
}

 