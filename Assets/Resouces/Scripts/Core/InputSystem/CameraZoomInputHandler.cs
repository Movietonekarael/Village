using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class CameraZoomInputHandler : SubInputHandler, IHoldControl
        {
            private bool _isZoomEnabled = false;
            private bool _isZoomingIn = false;
            private bool _isZoomingOut = false;

            public CameraZoomInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler(this.GetType().Name);
                var inputAction = _InputHandler._inputScheme.CameraControl;
                inputAction.Zoom.performed += ScrollMouse;
                inputAction.ReadyToZoom.started += SetReadyToZoomCamera;
                inputAction.ReadyToZoom.canceled += SetNotReadyToZoomCamera;
                inputAction.ZoomIn.started += StartZoomingIn;
                inputAction.ZoomIn.canceled += StopZoomingIn;
                inputAction.ZoomOut.started += StartZommingOut;
                inputAction.ZoomOut.canceled += StopZoomingOut;
            }

            protected override void UnregisterForInputEvents() 
            {
                CheckForInputHandler(this.GetType().Name);
                var inputAction = _InputHandler._inputScheme.CameraControl;
                inputAction.Zoom.performed -= ScrollMouse;
                inputAction.ReadyToZoom.started -= SetReadyToZoomCamera;
                inputAction.ReadyToZoom.canceled -= SetNotReadyToZoomCamera;
                inputAction.ZoomIn.started -= StartZoomingIn;
                inputAction.ZoomIn.canceled -= StopZoomingIn;
                inputAction.ZoomOut.started -= StartZommingOut;
                inputAction.ZoomOut.canceled -= StopZoomingOut;
            }

            private void ScrollMouse(InputAction.CallbackContext context)
            {
                SetZoom(context.ReadValue<float>());
            }

            private void SetReadyToZoomCamera(InputAction.CallbackContext context)
            {
                SetEnable();
            }

            private void SetNotReadyToZoomCamera(InputAction.CallbackContext context)
            {
                SetDisable();
            }

            private void StartZoomingIn(InputAction.CallbackContext context)
            {
                ZoomIn(true);
            }

            private void StopZoomingIn(InputAction.CallbackContext context)
            {
                ZoomIn(false);
            }

            private void StartZommingOut(InputAction.CallbackContext context)
            {
                ZoomOut(true);
            }

            private void StopZoomingOut(InputAction.CallbackContext context)
            {
                ZoomOut(false);
            }

            public void SetEnable()
            {
                _isZoomEnabled = true;
            }

            public void SetDisable()
            {
                _isZoomEnabled = false;
            }

            public void ZoomIn(bool val)
            {
                _isZoomingIn = val;
            }

            public void ZoomOut(bool val)
            {
                _isZoomingOut = val;
            }

            public void SetZoom(float val)
            {
                InvokeCameraZoomEvent(val);
            }

            private Task _gamepadZoomDelay;

            public void Update()
            {
                if (_isZoomEnabled && !(_isZoomingIn && _isZoomingOut))
                {
                    if ((_gamepadZoomDelay != null && _gamepadZoomDelay.IsCompleted) || _gamepadZoomDelay == null)
                    {
                        _gamepadZoomDelay = DelayZoom();
                    }
                }
            }

            private async Task DelayZoom()
            {
                if (_isZoomingIn)
                {
                    InvokeCameraZoomEvent(1);
                }
                else if (_isZoomingOut)
                {
                    InvokeCameraZoomEvent(-1);
                }
                await Task.Delay(100);
            }

            private void InvokeCameraZoomEvent(float val)
            {
                _InputHandler.OnCameraZoomed?.Invoke(val);
            }
        }
    }
}