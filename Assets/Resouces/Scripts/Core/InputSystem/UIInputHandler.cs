using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class UIInputHandler : SubInputHandler
        {
            public UIInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler("UIInputHandler");
                var inputAction = _InputHandler._inputScheme.UI;
                inputAction.Point.performed += ChangeMousePosition;
                inputAction.Click.started += StartLeftClick;
                inputAction.Click.canceled += CancelLeftClick;
            }

            private void ChangeMousePosition(InputAction.CallbackContext context)
            {
                _InputHandler._mousePosition = context.ReadValue<Vector2>();
            }

            private void StartLeftClick(InputAction.CallbackContext context)
            {
                _InputHandler.OnLeftMouseButtonPressed?.Invoke();
            }

            private void CancelLeftClick(InputAction.CallbackContext context)
            {
                _InputHandler.OnLeftMouseButtonReleased?.Invoke();
            }
        }
    }
}