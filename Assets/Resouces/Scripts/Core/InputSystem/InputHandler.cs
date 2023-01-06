using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    public sealed partial class InputHandler : MonoBehaviour
    {
        private static InputHandler _instance;

        private PlayerInputScheme _inputScheme;

        public event Action OnMovementStart;
        public event Action OnMovementFinish;
        public event Action<Vector2> OnMovement;
        public event Action<Vector2, bool> OnCameraRotated;
        public event Action<float> OnCameraZoomed;
        public event Action OnRunningChanged;
        public event Action OnDashed;
        public event Action OnJumped;

        public event Action OnOpenCloseInventory;
        public event Action<int> OnInventoryKeyPressed;

        public event Action OnLeftMouseButtonPressed;
        public event Action OnLeftMouseButtonReleased;

        private InputActionMap[] _generalInputActionMaps;
        private InputActionMap[] _freezableInputActionMaps;
        private IHoldControl[] _holdableInputActions;

        private CameraRotationInputHandler _cameraRotationInputHandler;
        private CameraZoomInputHandler _cameraZoomInputHandler;
        private PlayerMovementInputHandler _playerMovementInputHandler;
        private InventoryInputHandler _inventoryInputHandler;
        private UIInputHandler _uiInputHandler;
        private ApplicationInputHandler _applicationInputHandler;


        private Vector2 _mousePosition = Vector2.zero;
        public Vector2 MousePosition { get { return _mousePosition; } }


        private void Awake()
        {
            CheckForInstanceExistance();
            _inputScheme = new PlayerInputScheme();
            CacheActionMaps();
            InitializeSubInputHandlers();
            CacheHoldableInputActions();
        }

        private void CheckForInstanceExistance()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("There are more than one InputHandler in the scene.");
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        private void CacheActionMaps()
        {
            CacheGeneralActionMaps();
            CacheFreezableActionMaps();
        }

        private void CacheGeneralActionMaps()
        {
            _generalInputActionMaps = new InputActionMap[]
            { 
                _inputScheme.PlayerControl, 
                _inputScheme.CameraControl,
                _inputScheme.InventoryControl,
                _inputScheme.UI,
                _inputScheme.ApplicationControl
            };
        }

        private void CacheFreezableActionMaps()
        {
            _freezableInputActionMaps = new InputActionMap[]
            {
                _inputScheme.PlayerControl, 
                _inputScheme.CameraControl
            };
        }

        private void InitializeSubInputHandlers()
        {
            _cameraRotationInputHandler = new(this);
            _cameraZoomInputHandler = new(this);
            _playerMovementInputHandler = new(this);
            _inventoryInputHandler = new(this);
            _uiInputHandler = new(this);
            _applicationInputHandler = new(this);
        }

        private void CacheHoldableInputActions()
        {
            _holdableInputActions = new IHoldControl[]
            {
                _cameraRotationInputHandler,
                _cameraZoomInputHandler
            };
        }
 
        private void OnEnable()
        {
            _generalInputActionMaps.EnableAll();
        }

        private void OnDisable()
        {
            _generalInputActionMaps.DisableAll();
        }

        private void Update()
        {
            _holdableInputActions.UpdateAll();
        }

        public bool IfInteractWasPerformed()
        {
            return _inputScheme.PlayerControl.Intaract.WasPerformedThisFrame();
        }

        public void FreezeControlForInteraction(bool mustBeFreezed)//must be deleted soon
        {
            if (mustBeFreezed)
            {
                _inputScheme.PlayerControl.Disable();
                _inputScheme.CameraControl.Disable();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                _inputScheme.PlayerControl.Enable();
                _inputScheme.CameraControl.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public static InputHandler GetInstance(string senderName)
        {
            if (_instance is null)
                throw new Exception($"There is no InputHandler in the scene to attach to {senderName} script.");

            return _instance;
        }
    }
}

