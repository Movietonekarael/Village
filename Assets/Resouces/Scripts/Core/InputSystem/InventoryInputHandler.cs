using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class InventoryInputHandler : SubInputHandler
        {
            public InventoryInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler("InventoryInputHandler");
                var inputAction = _InputHandler._inputScheme.InventoryControl;
                inputAction.OpenCloseInventory.performed += OpenClosePlayerInventory;
                inputAction.Key1.performed += delegate { InventoryKeyPressed(1); };
                inputAction.Key2.performed += delegate { InventoryKeyPressed(2); };
                inputAction.Key3.performed += delegate { InventoryKeyPressed(3); };
                inputAction.Key4.performed += delegate { InventoryKeyPressed(4); };
                inputAction.Key5.performed += delegate { InventoryKeyPressed(5); };
                inputAction.Key6.performed += delegate { InventoryKeyPressed(6); };
                inputAction.Key7.performed += delegate { InventoryKeyPressed(7); };
                inputAction.Key8.performed += delegate { InventoryKeyPressed(8); };
            }

            private void OpenClosePlayerInventory(InputAction.CallbackContext context)
            {
                _InputHandler.OnOpenCloseInventory?.Invoke();
            }

            private void InventoryKeyPressed(int number)
            {
                _InputHandler.OnInventoryKeyPressed?.Invoke(number);
            }
        }
    }
}
