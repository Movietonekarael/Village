using UnityEditor;
using UnityEngine.InputSystem;


namespace GameCore
{
    namespace GameControls
    {
        partial class InputHandler
        {
            private sealed class ApplicationInputHandler : SubInputHandler
            {
                public ApplicationInputHandler(InputHandler inputHandler) : base(inputHandler) { }

                protected override void RegisterForInputEvents()
                {
                    CheckForInputHandler(this.GetType().Name);
                    _InputHandler._inputScheme.ApplicationControl.Escape.performed += EscapePressed;
#if UNITY_EDITOR
                    _InputHandler._inputScheme.ApplicationControl.EditorGamemodeQuit.performed += QuitGameMode;
#endif
                }

                protected override void UnregisterForInputEvents()
                {
                    CheckForInputHandler(this.GetType().Name);
                    _InputHandler._inputScheme.ApplicationControl.Escape.performed -= EscapePressed;
#if UNITY_EDITOR
                    _InputHandler._inputScheme.ApplicationControl.EditorGamemodeQuit.performed -= QuitGameMode;
#endif
                }

                private void EscapePressed(InputAction.CallbackContext context)
                {
                    _InputHandler.OnEscape?.Invoke();
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
}