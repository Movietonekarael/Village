using System;
using UnityEngine.InputSystem;


namespace GameCore
{
    namespace GameControls
    {
        partial class InputHandler
        {
            private sealed class InventoryInputHandler : SubInputHandler
            {
                public InventoryInputHandler(InputHandler inputHandler) : base(inputHandler) { }

                private Action<InputAction.CallbackContext>[] _keyActions;

                protected override void RegisterForInputEvents()
                {
                    InitializeKeyActions();

                    CheckForInputHandler(this.GetType().Name);
                    var inputAction = _InputHandler._inputScheme.InventoryControl;
                    inputAction.OpenCloseInventory.performed += OpenClosePlayerInventory;
                    inputAction.LeftArrow.performed += LeftKeyPressed;
                    inputAction.RightArrow.performed += RightKeyPressed;
                    inputAction.Key1.performed += _keyActions[0];
                    inputAction.Key2.performed += _keyActions[1];
                    inputAction.Key3.performed += _keyActions[2];
                    inputAction.Key4.performed += _keyActions[3];
                    inputAction.Key5.performed += _keyActions[4];
                    inputAction.Key6.performed += _keyActions[5];
                    inputAction.Key7.performed += _keyActions[6];
                    inputAction.Key8.performed += _keyActions[7];
                }

                protected override void UnregisterForInputEvents()
                {
                    CheckForInputHandler(this.GetType().Name);
                    var inputAction = _InputHandler._inputScheme.InventoryControl;
                    inputAction.OpenCloseInventory.performed -= OpenClosePlayerInventory;
                    inputAction.LeftArrow.performed -= LeftKeyPressed;
                    inputAction.RightArrow.performed -= RightKeyPressed;
                    inputAction.Key1.performed -= _keyActions[0];
                    inputAction.Key2.performed -= _keyActions[1];
                    inputAction.Key3.performed -= _keyActions[2];
                    inputAction.Key4.performed -= _keyActions[3];
                    inputAction.Key5.performed -= _keyActions[4];
                    inputAction.Key6.performed -= _keyActions[5];
                    inputAction.Key7.performed -= _keyActions[6];
                    inputAction.Key8.performed -= _keyActions[7];

                    DeinitializeKeyActions();
                }

                private void InitializeKeyActions()
                {
                    _keyActions = new Action<InputAction.CallbackContext>[8];

                    _keyActions[0] = delegate { InventoryKeyPressed(0); };
                    _keyActions[1] = delegate { InventoryKeyPressed(1); };
                    _keyActions[2] = delegate { InventoryKeyPressed(2); };
                    _keyActions[3] = delegate { InventoryKeyPressed(3); };
                    _keyActions[4] = delegate { InventoryKeyPressed(4); };
                    _keyActions[5] = delegate { InventoryKeyPressed(5); };
                    _keyActions[6] = delegate { InventoryKeyPressed(6); };
                    _keyActions[7] = delegate { InventoryKeyPressed(7); };
                }

                private void DeinitializeKeyActions()
                {
                    _keyActions = null;
                }

                private void OpenClosePlayerInventory(InputAction.CallbackContext context)
                {
                    _InputHandler.OnOpenCloseInventory?.Invoke();
                }

                private void InventoryKeyPressed(int number)
                {
                    _InputHandler.OnInventoryKeyPressed?.Invoke(number);
                }

                private void LeftKeyPressed(InputAction.CallbackContext context)
                {
                    _InputHandler.OnInventoryArrowPressed?.Invoke(-1);
                }

                private void RightKeyPressed(InputAction.CallbackContext context)
                {
                    _InputHandler.OnInventoryArrowPressed?.Invoke(+1);
                }
            }
        }
    }
}