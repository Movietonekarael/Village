using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
                CheckForInputHandler(this.GetType().Name);
                _InputHandler._inputScheme.ApplicationControl.Quit.performed += QuitApplication;
#if UNITY_EDITOR
                _InputHandler._inputScheme.ApplicationControl.EditorGamemodeQuit.performed += QuitGameMode;
#endif
            }

            protected override void UnregisterForInputEvents() 
            {
                CheckForInputHandler(this.GetType().Name);
                _InputHandler._inputScheme.ApplicationControl.Quit.performed -= QuitApplication;
#if UNITY_EDITOR
                _InputHandler._inputScheme.ApplicationControl.EditorGamemodeQuit.performed -= QuitGameMode;
#endif
            }

            private void QuitApplication(InputAction.CallbackContext context)
            {
                Application.Quit();
            }

#if UNITY_EDITOR
            private void QuitGameMode(InputAction.CallbackContext context)
            {
                EditorApplication.isPlaying = false;
            }
#endif
        }
    }
}
