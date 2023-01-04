using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class ApplicationInputHandler : SubInputHandler
        {
            public ApplicationInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler("ApplicationInputHandler");
                _InputHandler._inputScheme.ApplicationControl.Quit.performed += QuitApplication;
            }

            private void QuitApplication(InputAction.CallbackContext context)
            {
                Application.Quit();
            }
        }
    }
}
