using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private sealed class CameraRotationInputHandler : SubInputHandler, 
                                                          IHoldControl, 
                                                          IStickDirection
        {
            private bool _rotationEnabled = false;
            private Vector2 _forceVector = Vector2.zero;

            public CameraRotationInputHandler(InputHandler inputHandler) : base(inputHandler) { }

            protected override void RegisterForInputEvents()
            {
                CheckForInputHandler("CameraRotationInputHandler");
                var inputAction = _InputHandler._inputScheme.CameraControl;
                inputAction.MouseMovement.performed += MoveMouse;
                inputAction.RotationSticks.started += StartRightStickMoving;
                inputAction.RotationSticks.canceled += StopRightStickMoving;
                inputAction.RotationSticks.performed += PerformedRightStickMoving;
            }

            private void MoveMouse(InputAction.CallbackContext context)
            {
                InvokeCameraRotationEvent(context.ReadValue<Vector2>(), false);
            }

            private void StartRightStickMoving(InputAction.CallbackContext context)
            {
                SetEnable();
            }

            private void StopRightStickMoving(InputAction.CallbackContext context)
            {
                SetDisable();
            }

            private void PerformedRightStickMoving(InputAction.CallbackContext context)
            {
                Perform(context.ReadValue<Vector2>());
            }

            public void SetEnable()
            {
                _rotationEnabled = true;
            }

            public void SetDisable()
            {
                _rotationEnabled = false;
            }

            public void Perform(Vector2 forceVector)
            {
                _forceVector = forceVector;
            }

            public void Update()
            {
                if (_rotationEnabled)
                {
                    InvokeCameraRotationEvent(_forceVector
                                              * Time.deltaTime,
                                              true);
                }
            }

            private void InvokeCameraRotationEvent(Vector2 vec, bool isGamepad)
            {
                _InputHandler.OnCameraRotated?.Invoke(vec, isGamepad);
            }
        }
    }
}