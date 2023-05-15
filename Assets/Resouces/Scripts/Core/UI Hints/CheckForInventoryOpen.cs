using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GameControls
{
    public class CheckForInventoryOpen : MonoBehaviour, ISubscribable
    {
        private InputHandler _inputHandler;
        private bool _isClosed = true;

        private void Start()
        {
            CacheInputHandler();
            Subscribe();
            SetObjectState();
        }

        private void CacheInputHandler()
        {
            _inputHandler = InputHandler.GetInstance(this.GetType().Name);
        }

        public void Subscribe()
        {
            _inputHandler.OnOpenCloseInventory += InventoryWasOpenedClosed;
        }

        public void Unsubscribe()
        {
            _inputHandler.OnOpenCloseInventory -= InventoryWasOpenedClosed;
        }

        public void InventoryWasOpenedClosed()
        {
            _isClosed = !_isClosed;
            SetObjectState();
        }

        private void SetObjectState()
        {
            this.gameObject.SetActive(!_isClosed);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}

