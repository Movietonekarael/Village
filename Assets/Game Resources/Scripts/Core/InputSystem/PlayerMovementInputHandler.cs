using PlayerInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class PlayerMovementInputHandler : SubInputHandler
        {
            public PlayerMovementInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler(this.GetType().Name);
                var inputActions = _InputHandler._inputScheme.PlayerControl;
                inputActions.Movement.started += StartMoving;
                inputActions.Movement.canceled += StopMoving;
                inputActions.Movement.performed += Move;
                inputActions.isRunning.started += SwitchRunState;
                inputActions.Dash.started += Dash;
                inputActions.Jump.performed += Jump;
            }

            protected override void UnregisterForInputEvents()
            {
                CheckForInputHandler(this.GetType().Name);
                var inputActions = _InputHandler._inputScheme.PlayerControl;
                inputActions.Movement.started -= StartMoving;
                inputActions.Movement.canceled -= StopMoving;
                inputActions.Movement.performed -= Move;
                inputActions.isRunning.started -= SwitchRunState;
                inputActions.Dash.started -= Dash;
                inputActions.Jump.performed -= Jump;
            }

            private void StartMoving(InputAction.CallbackContext context)
            {
                _InputHandler.OnMovementStart?.Invoke();
            }

            private void StopMoving(InputAction.CallbackContext context)
            {
                _InputHandler.OnMovementFinish?.Invoke();
            }

            private void Move(InputAction.CallbackContext context)
            {
                _InputHandler.OnMovement?.Invoke(context.ReadValue<Vector2>());
            }

            private void SwitchRunState(InputAction.CallbackContext context)
            {
                _InputHandler.OnRunningChanged?.Invoke();
            }

            private void Dash(InputAction.CallbackContext context)
            {
                _InputHandler.OnDashed?.Invoke();
            }

            private void Jump(InputAction.CallbackContext context)
            {
                _InputHandler.OnJumped?.Invoke();
            }
        }
    }
}

