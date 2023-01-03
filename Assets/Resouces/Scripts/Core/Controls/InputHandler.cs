using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using GameCore.GameMovement;



namespace GameCore.GameControls
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance { get; private set; }

        private PlayerInputScheme _playerInput;

        public event Action OnMovementStart;
        public event Action OnMovementFinish;
        public event Action<Vector2> OnMovement;
        public event Action<Vector2> OnCameraMoved;
        public event Action<float> OnMouseScrolled;
        public event Action OnRunningChanged;
        public event Action OnDashed;
        public event Action OnJumpPressed;

        public event Action OnOpenCloseInventory;
        public event Action<int> OnInventoryKeyPressed;

        public event Action OnLeftClickStarted;
        public event Action OnLeftClickCanceled;


        private GamepadCameraRotation _gamepadCameraRotation;
        private GamepadCameraZoom _gamepadCameraZoom;
        [SerializeField] private float _cameraRotationKoefficient = 25.0f;

        private bool _firstFrameUpdateFlag = true;

        private Vector2 _mousePosition = Vector2.zero;
        public Vector2 MousePosition
        {
            get
            {
                return _mousePosition;
            }
        }


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("There are more than one InputHandler in the scene.");
                Destroy(this);
            }
            else
            {
                Instance = this;
                _playerInput = new PlayerInputScheme();

                _gamepadCameraRotation = new(this, _cameraRotationKoefficient);
                _gamepadCameraZoom = new(this);
            }
        }

        private void OnEnable()
        {
            _playerInput.PlayerControl.Movement.started += StartMoving;
            _playerInput.PlayerControl.Movement.canceled += StopMoving;
            _playerInput.PlayerControl.Movement.performed += Moving;
            _playerInput.PlayerControl.isRunning.started += SwitchRunState;
            _playerInput.PlayerControl.Dash.started += Dash;
            _playerInput.PlayerControl.Jump.performed += PerformJump;
            _playerInput.PlayerControl.Enable();

            _playerInput.CameraControl.Rotation.performed += MouseMoved;
            _playerInput.CameraControl.Zoom.performed += MouseScrolled;
            _playerInput.CameraControl.RotationSticks.started += RightStickStartMoving;
            _playerInput.CameraControl.RotationSticks.canceled += RightStickStopMoving;
            _playerInput.CameraControl.RotationSticks.performed += RightStickPerformedMoving;
            _playerInput.CameraControl.ReadyToZoomButton.started += ReadyToZoomCamera;
            _playerInput.CameraControl.ReadyToZoomButton.canceled += NotReadyToZoomCamera;
            _playerInput.CameraControl.ZoomIn.started += StartZoomingIn;
            _playerInput.CameraControl.ZoomIn.canceled += StopZoomingIn;
            _playerInput.CameraControl.ZoomOut.started += StartZommingOut;
            _playerInput.CameraControl.ZoomOut.canceled += StopZoomingOut;
            _playerInput.CameraControl.Enable();

            _playerInput.InventoryControl.OpenCloseInventory.performed += OpenClosePlayerInventory;
            _playerInput.InventoryControl._1Key1.performed += Inventory1KeyPressed;
            _playerInput.InventoryControl._2Key1.performed += Inventory2KeyPressed;
            _playerInput.InventoryControl._3Key.performed += Inventory3KeyPressed;
            _playerInput.InventoryControl._4Key1.performed += Inventory4KeyPressed;
            _playerInput.InventoryControl._5Key.performed += Inventory5KeyPressed;
            _playerInput.InventoryControl._6Key.performed += Inventory6KeyPressed;
            _playerInput.InventoryControl._7Key.performed += Inventory7KeyPressed;
            _playerInput.InventoryControl._8Key.performed += Inventory8KeyPressed;
            _playerInput.InventoryControl.Enable();

            _playerInput.UI.Point.performed += MousePositionChanged;
            _playerInput.UI.Click.started += UILeftClickStared;
            _playerInput.UI.Click.canceled += UILeftClickCanceled;
            _playerInput.UI.Enable();

            _playerInput.ApplicationControl.Quit.performed += QuitApplication;
            _playerInput.ApplicationControl.Enable();
        }


        private void OnDisable()
        {
            _playerInput.PlayerControl.Disable();
            _playerInput.CameraControl.Disable();
            _playerInput.InventoryControl.Disable();
            _playerInput.UI.Disable();
            _playerInput.ApplicationControl.Disable();
        }

        public bool IfInteractWasPerformed()
        {
            return _playerInput.PlayerControl.Intaract.WasPerformedThisFrame();
        }

        //---------------------------------------------- ------Moving-----------------------------------------------------//

        private void StartMoving(InputAction.CallbackContext context)
        {
            OnMovementStart?.Invoke();
        }

        private void StopMoving(InputAction.CallbackContext context)
        {
            OnMovementFinish?.Invoke();
        }

        private void Moving(InputAction.CallbackContext context)
        {
            OnMovement?.Invoke(context.ReadValue<Vector2>());
        }

        //-----------------------------------------------------Running----------------------------------------------------//

        private void SwitchRunState(InputAction.CallbackContext context)
        {
            OnRunningChanged?.Invoke();
        }

        private void Dash(InputAction.CallbackContext context)
        {
            OnDashed?.Invoke();
        }

        //-------------------------------------------------Camera rotation------------------------------------------------//

        private void MouseMoved(InputAction.CallbackContext context)
        {
            if (_firstFrameUpdateFlag) _firstFrameUpdateFlag = false;
            else _gamepadCameraRotation.SetMouseRotationDirection(context.ReadValue<Vector2>());
        }

        private void RightStickStartMoving(InputAction.CallbackContext context)
        {
            _gamepadCameraRotation.SetEnable();
        }

        private void RightStickStopMoving(InputAction.CallbackContext context)
        {
            _gamepadCameraRotation.SetDisable();
        }

        private void RightStickPerformedMoving(InputAction.CallbackContext context)
        {
            _gamepadCameraRotation.Perform(context.ReadValue<Vector2>());
        }

        private void PerformJump(InputAction.CallbackContext context)
        {
            OnJumpPressed?.Invoke();
        }

        //--------------------------------------------------Camera zoom---------------------------------------------------//

        private void MouseScrolled(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.SetMouseScroll(context.ReadValue<float>());
        }

        private void ReadyToZoomCamera(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.SetEnable();
        }

        private void NotReadyToZoomCamera(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.SetDisable();
        }

        private void StartZoomingIn(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.ZoomIn(true);
        }

        private void StopZoomingIn(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.ZoomIn(false);
        }

        private void StartZommingOut(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.ZoomOut(true);
        }

        private void StopZoomingOut(InputAction.CallbackContext context)
        {
            _gamepadCameraZoom.ZoomOut(false);
        }

        //----------------------------------------------------Interact-----------------------------------------------------//

        private void OpenClosePlayerInventory(InputAction.CallbackContext context)
        {
            OnOpenCloseInventory?.Invoke();
        }

        public void FreezeControlForInteraction(bool mustBeFreezed)
        {
            if (mustBeFreezed)
            {
                _playerInput.PlayerControl.Disable();
                _playerInput.CameraControl.Disable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                _playerInput.PlayerControl.Enable();
                _playerInput.CameraControl.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _playerInput.InventoryControl._1Key1.performed += Inventory1KeyPressed;
                _playerInput.InventoryControl._2Key1.performed += Inventory2KeyPressed;
                _playerInput.InventoryControl._3Key.performed += Inventory3KeyPressed;
                _playerInput.InventoryControl._4Key1.performed += Inventory4KeyPressed;
                _playerInput.InventoryControl._5Key.performed += Inventory5KeyPressed;
                _playerInput.InventoryControl._6Key.performed += Inventory6KeyPressed;
                _playerInput.InventoryControl._7Key.performed += Inventory7KeyPressed;
                _playerInput.InventoryControl._8Key.performed += Inventory8KeyPressed;
            }
        }

        private void Inventory1KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(1);
        }

        private void Inventory2KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(2);
        }

        private void Inventory3KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(3);
        }

        private void Inventory4KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(4);
        }

        private void Inventory5KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(5);
        }

        private void Inventory6KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(6);
        }

        private void Inventory7KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(7);
        }

        private void Inventory8KeyPressed(InputAction.CallbackContext context)
        {
            OnInventoryKeyPressed?.Invoke(8);
        }

        //-------------------------------------------------------UI--------------------------------------------------------//

        private void MousePositionChanged(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }

        private void UILeftClickStared(InputAction.CallbackContext context)
        {
            OnLeftClickStarted?.Invoke();
        }

        private void UILeftClickCanceled(InputAction.CallbackContext context)
        {
            OnLeftClickCanceled?.Invoke();
        }

        //------------------------------------------------------Quit-------------------------------------------------------//

        private void QuitApplication(InputAction.CallbackContext context)
        {
            Application.Quit();
        }

        //-------------------------------------------------------Update---------------------------------------------------------//

        private void FixedUpdate()
        {
            _gamepadCameraRotation.OnUpdate();
            _gamepadCameraZoom.OnUpdate();
        }

        //-------------------------------------------------------Hold Classes-------------------------------------------------------//

        private interface IHoldControl
        {
            public void OnUpdate();
            public void SetEnable();
            public void SetDisable();
            public void Perform(Vector2 vec);
        }

        private class GamepadCameraRotation : IHoldControl
        {
            private bool _rotationEnabled = false;
            private Vector2 _direction = Vector2.zero;
            private readonly float _cameraRotationKoefficient;
            private readonly InputHandler _inputHandler;

            public GamepadCameraRotation(InputHandler inputHandler, float cameraRotationKoefficient)
            {
                _inputHandler = inputHandler;
                _cameraRotationKoefficient = cameraRotationKoefficient;
            }

            public void SetMouseRotationDirection(Vector2 vec)
            {
                _inputHandler.OnCameraMoved?.Invoke(vec);
            }

            public void SetEnable()
            {
                _rotationEnabled = true;
            }

            public void SetDisable()
            {
                _rotationEnabled = false;
            }

            public void Perform(Vector2 direction)
            {
                _direction = direction;
            }

            public void OnUpdate()
            {
                if (_rotationEnabled)
                {
                    _inputHandler.OnCameraMoved?.Invoke(_direction * _cameraRotationKoefficient);
                }
            }
        }

        private class GamepadCameraZoom : IHoldControl
        {
            private bool _zoomEnabled = false;
            private bool _zoomIn = false;
            private bool _zoomOut = false;

            private readonly InputHandler _inputHandler;


            public GamepadCameraZoom(InputHandler inputHandler)
            {
                _inputHandler= inputHandler;
            }

            public void SetEnable()
            {
                _zoomEnabled = true;
            }

            public void SetDisable()
            {
                _zoomEnabled = false;
            }

            public void ZoomIn(bool val)
            {
                _zoomIn = val;
            }

            public void ZoomOut(bool val)
            {
                _zoomOut = val;
            }

            public void SetMouseScroll(float val)
            {
                InvokeMouseScroll(val);
            }

            public void Perform(Vector2 direction)
            {

            }

            public void OnUpdate()
            {
                if(_zoomEnabled && !(_zoomIn && _zoomOut))
                {
                    if(_zoomIn)
                    {
                        InvokeMouseScroll(1);
                    } 
                    else if(_zoomOut)
                    {
                        InvokeMouseScroll(-1);
                    }
                }
            }

            private void InvokeMouseScroll(float val)
            {
#if (UNITY_STANDALONE_LINUX && !UNITY_EDITOR)
                _inputHandler.OnMouseScrolled?.Invoke(-val);
#else
                _inputHandler.OnMouseScrolled?.Invoke(val);
#endif
            }
        }
    }
}

