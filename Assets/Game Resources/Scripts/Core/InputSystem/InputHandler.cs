using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using Zenject;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

namespace GameCore.GameControls
{
    public sealed partial class InputHandler : MonoBehaviour, 
                                               IInteractionPerformer, 
                                               ICameraRotator, 
                                               ICameraZoomer,
                                               IMovement,
                                               IInventoryPress,
                                               IOpenCloseInventory,
                                               IEscapable
    {
        private const string _GAMEPAD_SCHEME = "Gamepad";
        private const string _KEYBOADR_SCHEME = "Keyboard";

        [Inject(Id = "VirtualPointer")] private readonly RectTransform _pointerTransform;
        [Inject(Id = "HintsCanvas")] private readonly RectTransform _canvasTransform;
        [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;
        [Inject] private readonly UnityEngine.InputSystem.PlayerInput _playerInput;

        private PlayerInputScheme _inputScheme;

        public event Action<Vector2, bool> OnCameraRotated;
        public event Action<float> OnCameraZoomed;
        public event Action OnMovementStart;
        public event Action OnMovementFinish;
        public event Action<Vector2> OnMovement;
        public event Action OnRunningChanged;
        public event Action OnDashed;
        public event Action OnJumped;

        public event Action OnOpenCloseInventory;
        public event Action<int> OnInventoryKeyPressed;
        public event Action<int> OnInventoryArrowPressed;

        public event Action OnEscape;

        public event Action OnLeftMouseButtonPressed;
        public event Action OnLeftMouseButtonReleased;

        public event Action<ControlScheme> OnControlSchemeChanged;

        private InputActionMap[] _generalInputActionMaps;
        private InputActionMap[] _freezableInputActionMaps;
        private bool _controlsFreezed = false;
        private IHoldControl[] _holdableInputActions;

        private CameraRotationInputHandler _cameraRotationInputHandler;
        private CameraZoomInputHandler _cameraZoomInputHandler;
        private PlayerMovementInputHandler _playerMovementInputHandler;
        private InventoryInputHandler _inventoryInputHandler;
        private UIInputHandler _uiInputHandler;
        private ApplicationInputHandler _applicationInputHandler;



        [Tooltip("1/1920 part of screen width per second * gamepad stick input value.")]
        [SerializeField] private float _virtualMouseSpeed = 150f;
        [HideInInspector] public VirtualMouseHandler VirtualMouse;
        [HideInInspector] public RealMouseHandler RealMouse;

        private Vector2 _mousePosition = Vector2.zero;
        public Vector2 MousePosition { get { return _mousePosition; } }

        private ControlScheme _currentControlScheme = ControlScheme.Keyboard;
        public ControlScheme CurrentControlScheme { get { return _currentControlScheme; } }

        private void Awake()
        {
            _inputScheme = new PlayerInputScheme();
            CacheActionMaps();
            InitializeVirtualMouseHandler();
            SubscribeForSchemeChange();
            InitializeSubInputHandlers();
            CacheHoldableInputActions();
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
            RealMouse = new RealMouseHandler(this);
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

        private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
        {
            if (input.currentControlScheme == _KEYBOADR_SCHEME && _currentControlScheme != ControlScheme.Keyboard)
            {
                _currentControlScheme = ControlScheme.Keyboard;
            }
            else if (input.currentControlScheme == _GAMEPAD_SCHEME && _currentControlScheme != ControlScheme.Gamepad)
            {
                _currentControlScheme = ControlScheme.Gamepad;
            }

            OnControlSchemeChanged?.Invoke(_currentControlScheme);
        }

        public Vector2 AnchorPosition(Vector2 position)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform,
                                        position,
                                        _uiCamera,
                                        out var anchoredPosition);

            return anchoredPosition;
        }
    }
}

