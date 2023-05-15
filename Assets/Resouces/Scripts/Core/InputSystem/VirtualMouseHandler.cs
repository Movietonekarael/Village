using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;


namespace GameCore.GameControls
{
    public partial class InputHandler
    {
        public sealed class VirtualMouseHandler
        {
            private InputHandler _inputHandler;
            private UnityEngine.InputSystem.PlayerInput _playerInput => _inputHandler._playerInput;
            private RectTransform _pointerTransform => _inputHandler._pointerTransform;

            private Mouse _virtualMouse;
            public Mouse VirtualMouse { get { return _virtualMouse; } }
            private Mouse _realMouse;

            public VirtualMouseHandler(InputHandler inputHandler)
            {
                _inputHandler = inputHandler;
                InitializeVirtualMouse();
            }

            private void InitializeVirtualMouse()
            {
                _realMouse = Mouse.current;

                InputDevice virtualMouseInputDevice = InputSystem.GetDevice("VirtualMouse");

                if (virtualMouseInputDevice == null)
                {
                    _virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
                }
                else if (!virtualMouseInputDevice.added)
                {
                    _virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
                }
                else
                {
                    _virtualMouse = (Mouse)virtualMouseInputDevice;
                }

                InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);

                if (_pointerTransform != null)
                {
                    var changingPosition = _pointerTransform.anchoredPosition;
                    InputState.Change(_virtualMouse.position, changingPosition);
                }
            }

            public void DeactivateVirtualPointer()
            {
                _pointerTransform.gameObject.SetActive(false);
                Cursor.visible = true;
                _realMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
                InputState.Change(_virtualMouse.position, Vector2.zero);
            }

            public void ActivateVirtualPointer()
            {
                _pointerTransform.gameObject.SetActive(true);
                Cursor.visible = false;
                InputState.Change(_virtualMouse.position, _realMouse.position.ReadValue());
                AnchorCursor(_realMouse.position.ReadValue());
            }

            private void AnchorCursor(Vector2 newPosition)
            {
                _pointerTransform.anchoredPosition = _inputHandler.AnchorPosition(newPosition);
            }

            public void EnableVirtualMouse()
            {
                InputSystem.EnableDevice(_virtualMouse);
            }

            public void DisableVirtualMouse()
            {
                InputSystem.DisableDevice(_virtualMouse);
            }
        }
    }
}