using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GameControls
    {
        public class CheckForInventoryOpen : MonoBehaviour, ISubscribable
        {
            [Inject] private readonly IOpenCloseInventory _openCloseInventory;
            private bool _isClosed = true;

            private void Start()
            {
                Subscribe();
                SetObjectState();
            }

            public void Subscribe()
            {
                _openCloseInventory.OnOpenCloseInventory += InventoryWasOpenedClosed;
            }

            public void Unsubscribe()
            {
                _openCloseInventory.OnOpenCloseInventory -= InventoryWasOpenedClosed;
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
}