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
    public enum ControlScheme
    {
        Keyboard,
        Gamepad
    }


    public sealed partial class InputHandler : MonoBehaviour, IInteractionPerformer
    {
        [SerializeField] private RectTransform _pointerTransform;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;

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
        public event Action<int> OnInventoryArrowPressed;

        public event Action OnLeftMouseButtonPressed;
        public event Action OnLeftMouseButtonReleased;

        public event Action<ControlScheme> OnControlSchemeChanged;

        private InputActionMap[] _generalInputActionMaps;
        private InputActionMap[] _freezableInputActionMaps;
        private IHoldControl[] _holdableInputActions;

        private CameraRotationInputHandler _cameraRotationInputHandler;
        private CameraZoomInputHandler _cameraZoomInputHandler;
        private PlayerMovementInputHandler _playerMovementInputHandler;
        private InventoryInputHandler _inventoryInputHandler;
        private UIInputHandler _uiInputHandler;
        private ApplicationInputHandler _applicationInputHandler;

        [HideInInspector] public VirtualMouseHandler VirtualMouse;

        private Vector2 _mousePosition = Vector2.zero;
        public Vector2 MousePosition { get { return _mousePosition; } }
        
        [SerializeField] private float _virtualMouseSpeed = 150f;        

        private ControlScheme _currentControlScheme = ControlScheme.Keyboard;
        public ControlScheme CurrentControlScheme { get { return _currentControlScheme; } }

        private const string _GAMEPAD_SCHEME = "Gamepad";
        private const string _KEYBOADR_SCHEME = "Keyboard";

        private void Awake()
        {
            InitializeSingleton();
            _inputScheme = new PlayerInputScheme();
            CacheActionMaps();
            InitializeVirtualMouseHandler();
            SubscribeForSchemeChange();
            InitializeSubInputHandlers();
            CacheHoldableInputActions();
        }

        private void InitializeSingleton()
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

        private bool _controlsFreezed = false;

        public void EnableFreezableInputActionMaps()
        {
            _freezableInputActionMaps.EnableAll();
            _controlsFreezed = false;
        }

        public void DisableFreezableInputActionMaps()
        {
            _freezableInputActionMaps.DisableAll();
            _controlsFreezed = true;
        }

        private void InitializeVirtualMouseHandler()
        {
            VirtualMouse = new VirtualMouseHandler(this);
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
                _cameraZoomInputHandler,
                _uiInputHandler
            };
        }

        private void SubscribeForSchemeChange()
        {
            _playerInput.onControlsChanged += OnControlsChanged;
        }

        private void UnsubscribeForSchemeChange()
        {
            _playerInput.onControlsChanged -= OnControlsChanged;
        }

        private void OnEnable()
        {
            SubscribeForSchemeChange();
            _generalInputActionMaps.EnableAll();
        }

        private void OnDisable()
        {
            UnsubscribeForSchemeChange();
            _generalInputActionMaps.DisableAll();
        }

        private void Update()
        {
            _holdableInputActions.UpdateAll();
        }

        private void OnDestroy()
        {
            DestroyAllReferencesInSubHandlers();
        }

        private void DestroyAllReferencesInSubHandlers()
        {
            _cameraRotationInputHandler.DestroyAllReferences();
            _cameraZoomInputHandler.DestroyAllReferences();
            _playerMovementInputHandler.DestroyAllReferences();
            _inventoryInputHandler.DestroyAllReferences();
            _uiInputHandler.DestroyAllReferences();
            _applicationInputHandler.DestroyAllReferences();
        }

        public bool IfInteractionWasPerformed()
        {
            return _inputScheme.PlayerControl.Intaract.WasPerformedThisFrame();
        }

        public static InputHandler GetInstance(string senderName)
        {
            if (_instance == null)
                throw new Exception($"There is no InputHandler in the scene to attach to {senderName} script.");

            return _instance;
        }

        private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
        {
            if (_playerInput.currentControlScheme == _KEYBOADR_SCHEME && _currentControlScheme != ControlScheme.Keyboard)
            {
                _currentControlScheme = ControlScheme.Keyboard;
            }
            else if (_playerInput.currentControlScheme == _GAMEPAD_SCHEME && _currentControlScheme != ControlScheme.Gamepad)
            {
                _currentControlScheme = ControlScheme.Gamepad;
            }
            else
            {
                return;
            }
            OnControlSchemeChanged?.Invoke(_currentControlScheme);
        }

        public Vector2 AnchorPosition(Vector2 position)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform,
                                        position,
                                        _mainCamera,
                                        out var anchoredPosition);

            return anchoredPosition;
        }
    }
}

