using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using Zenject;

namespace GameCore.GameControls
{
    public partial class InputHandler
    {

        public sealed class VirtualMouseHandler : IMouseEnablable
        {
            private const string _DEVICE_NAME = "VirtualMouse";

            private readonly InputHandler _inputHandler;
            private UnityEngine.InputSystem.PlayerInput _playerInput => _inputHandler._playerInput;
            private RectTransform _pointerTransform => _inputHandler._pointerTransform;
            private RealMouseHandler _realMouse => _inputHandler.RealMouse;

            private Mouse _virtualMouse;
            public Mouse VirtualMouse { get { return _virtualMouse; } }

            public bool previousMouseState;

            public VirtualMouseHandler(InputHandler inputHandler)
            {
                _inputHandler = inputHandler;
                InitializeVirtualMouse();
            }

            private void InitializeVirtualMouse()
            {
                InputDevice virtualMouseInputDevice = InputSystem.GetDevice(_DEVICE_NAME);

                if (virtualMouseInputDevice == null || !virtualMouseInputDevice.added)
                {
                    _virtualMouse = (Mouse)InputSystem.AddDevice(_DEVICE_NAME);
                }
                else
                {
                    _virtualMouse = (Mouse)virtualMouseInputDevice;
                }

                InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);   
            }

            public void EnableMouse()
            {
                ActivateVirtualPointer();
                InputSystem.EnableDevice(_virtualMouse);
            }

            public void DisableMouse()
            {
                DeactivateVirtualPointer();
                InputSystem.DisableDevice(_virtualMouse); 
            }

            private void ActivateVirtualPointer()
            {
                _pointerTransform.gameObject.SetActive(true);
                Cursor.visible = false;
                SetPosition(_realMouse.GetPosition());
                AnchorPointer(GetPosition());
            }

            private void DeactivateVirtualPointer()
            {
                _pointerTransform.gameObject.SetActive(false);
                Cursor.visible = true;
                _realMouse.DisableMouse();
                _realMouse.SetPosition(GetPosition());
                _realMouse.EnableMouse();
            }

            private void AnchorPointer(Vector2 newPosition)
            {
                _pointerTransform.anchoredPosition = _inputHandler.AnchorPosition(newPosition);
            }

            public void SetPosition(Vector2 position)
            {
                InputState.Change(_virtualMouse.position, position);
            }

            public void SetDelta(Vector2 delta) 
            {
                InputState.Change(_virtualMouse.delta, delta);
            }

            public Vector2 GetPosition()
            {
                return _virtualMouse.position.ReadValue();
            }

            public void PerformClick(bool isPressed)
            {
                _virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Left, isPressed);
                InputState.Change(_virtualMouse, mouseState);
                previousMouseState = isPressed;
            }
        }
    }
}