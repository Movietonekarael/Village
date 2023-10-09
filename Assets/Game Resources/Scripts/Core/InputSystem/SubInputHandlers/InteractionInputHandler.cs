using UnityEngine.InputSystem;


namespace GameCore
{
    namespace GameControls
    {
        partial class InputHandler
        {
            private sealed class InteractionInputHandler : SubInputHandler
            {
                public InteractionInputHandler(InputHandler inputHandler) : base(inputHandler) { }

                protected override void RegisterForInputEvents()
                {
                    CheckForInputHandler(this.GetType().Name);
                    _InputHandler._inputScheme.PlayerControl.Intaract.performed += InteractionPressed;
                }

                protected override void UnregisterForInputEvents()
                {
                    CheckForInputHandler(this.GetType().Name);
                    _InputHandler._inputScheme.PlayerControl.Intaract.performed -= InteractionPressed;
                }

                private void InteractionPressed(InputAction.CallbackContext context)
                {
                    _InputHandler.OnInteractionPerformed?.Invoke();
                }
            }
        }
    }
}