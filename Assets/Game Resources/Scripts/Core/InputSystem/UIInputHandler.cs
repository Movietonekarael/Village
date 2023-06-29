using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using System;

namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class UIInputHandler : SubInputHandler, IHoldControl, IStickDirection
        {
            private VirtualMouseHandler _virtualMouseHandler => _InputHandler.VirtualMouse;
            private float _virtualMouseSpeed => _InputHandler._virtualMouseSpeed;
            private RectTransform _pointerTransform => _InputHandler._pointerTransform;

            private Vector2 _mouseMoveDirection = Vector2.zero;
            private bool _mouseMoveEnabled = false;

            private InputAction _virtualMouseClick => _InputHandler._inputScheme.UI.GamepadVirtualMouseClick;

            public UIInputHandler(InputHandler inputHandler) : base(inputHandler) { }


            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler(this.GetType().Name);
                var inputAction = _InputHandler._inputScheme.UI;
                inputAction.MousePosition.performed += ChangeMousePosition;
                inputAction.Click.started += StartLeftClick;
                inputAction.Click.canceled += CancelLeftClick;
                inputAction.GamepadVirtualMousePoint.started += StartVirtualPointMoving;
                inputAction.GamepadVirtualMousePoint.canceled += StopVirtualPointMoving;
                inputAction.GamepadVirtualMousePoint.performed += PerformVirtualPointMoving;
            }

            protected override void UnregisterForInputEvents() 
            {
                CheckForInputHandler(this.GetType().Name);
                var inputAction = _InputHandler._inputScheme.UI;
                inputAction.MousePosition.performed -= ChangeMousePosition;
                inputAction.Click.started -= StartLeftClick;
                inputAction.Click.canceled -= CancelLeftClick;
                inputAction.GamepadVirtualMousePoint.started -= StartVirtualPointMoving;
                inputAction.GamepadVirtualMousePoint.canceled -= StopVirtualPointMoving;
                inputAction.GamepadVirtualMousePoint.performed -= PerformVirtualPointMoving;
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

            private void StartVirtualPointMoving(InputAction.CallbackContext context)
            {
                SetEnable();
            }

            private void StopVirtualPointMoving(InputAction.CallbackContext context)
            {
                SetDisable();
            }

            private void PerformVirtualPointMoving(InputAction.CallbackContext context)
            {
                Perform(context.ReadValue<Vector2>());
            }

            public void SetEnable()
            {
                _mouseMoveEnabled = true;
            }

            public void SetDisable()
            {
                _mouseMoveEnabled = false;
            }

            public void Perform(Vector2 vec)
            {
                _mouseMoveDirection = vec;
            }

            public void Update()
            {
                if (_InputHandler._controlsFreezed)
                {
                    TryMoveVirtualMouse();
                    TryClickVirtualMouse();
                }
            }

            private void TryMoveVirtualMouse()
            {
                if (_mouseMoveEnabled)
                {
                    MoveVirtualMouse();
                }
            }

            private void TryClickVirtualMouse()
            {
                var isVirtualButtonPressed = _virtualMouseClick.IsPressed();
                if (isVirtualButtonPressed != _virtualMouseHandler.PreviousMouseState)
                {
                    _virtualMouseHandler.PerformClick(isVirtualButtonPressed);
                }
            }

            private void MoveVirtualMouse()
            {
                if (!_virtualMouseHandler.VirtualMouse.enabled || Gamepad.current == null)
                    return;

                var deltaValue = _mouseMoveDirection;
                deltaValue *= _virtualMouseSpeed * Screen.width / 1920f * Time.deltaTime;

                var currentPosition = _virtualMouseHandler.GetPosition();
                var newPosition = currentPosition + deltaValue;

                newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
                newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

                _virtualMouseHandler.SetPosition(newPosition);
                _virtualMouseHandler.SetDelta(deltaValue);

                _InputHandler._mousePosition = newPosition;
                AnchorPointer(newPosition);
            }

            private void AnchorPointer(Vector2 newPosition)
            {
                var anchoredPosition = _InputHandler.AnchorPosition(newPosition);
                _pointerTransform.anchoredPosition = anchoredPosition;
            }
        }
    }
}