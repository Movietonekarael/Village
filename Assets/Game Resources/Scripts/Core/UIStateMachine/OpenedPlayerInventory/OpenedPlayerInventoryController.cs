using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.InputSystem;
using static GameCore.GameControls.InputHandler;
using Zenject;
using System.Threading.Tasks;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryController : IOpenedPlayerInventoryController, ISubscribable, IDeinitializable, IActivatable
    {
        [Inject] private readonly IOpenedPlayerInventoryView _openedPlayerInventoryView;
        [Inject(Id = typeof(OpenedPlayerInventoryView))] private readonly IDeinitializable _openedPlayerInventoryViewDeinitializator;
        [Inject(Id = typeof(OpenedPlayerInventoryView))] private readonly IActivatable _openedPlayerInventoryViewActivator;

        [Inject] private readonly InputHandler _inputHandler;
        [Inject] private readonly PlayerInventory _inventory;
        [Inject] private readonly UnityEngine.InputSystem.PlayerInput _playerInput;
        private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
        private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;

        private bool _openedInventoryControlsActive = false;

        public void Init(OpenedPlayerInventoryViewParameters parameters) 
        {
            InitializeView(parameters);
            Subscribe();
        }

        private void InitializeView(OpenedPlayerInventoryViewParameters parameters)
        {
            _openedPlayerInventoryView.Init(parameters, _inventory.GetInventorySize(), this);
        }

        public void Deinitialize()
        {
            Unsubscribe();
            _openedPlayerInventoryViewDeinitializator.Deinitialize();
        }

        public void Subscribe()
        {
            _inventory.OnItemChanged += ChangeItemInformation;
            _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
        }

        public void Unsubscribe()
        {
            _inventory.OnItemChanged -= ChangeItemInformation;
            _inputHandler.OnControlSchemeChanged -= ControlSchemeChanged;
        }

        private void ChangeItemInformation(int position)
        {
            var item = _inventory.GetGameItem(position);
            _openedPlayerInventoryView.SetItemInformation(position, item);
        }

        public void Activate()
        {
            _openedPlayerInventoryViewActivator.Activate();
            SetOpenedInventoryControlsActive();
        }

        public void Deactivate()
        {
            _openedPlayerInventoryViewActivator.Deactivate();
            SetOpenedInventoryControlsInactive();
        }

        public void ChangeItemsInInventory(int itemNumber1, int itemNumber2)
        {
            _inventory.MoveItem(itemNumber1, itemNumber2);
        }

        public void DropInventoryItem(int itemNumber)
        {
            _inventory.DropItem(itemNumber);
        }

        private void ControlSchemeChanged(ControlScheme controlScheme)
        {
            if (controlScheme == ControlScheme.Keyboard) 
            {
                if (_openedInventoryControlsActive)
                {
                    _virtualMouseHandler.DisableMouse();
                }
            }
            else if (controlScheme == ControlScheme.Gamepad)
            {
                if (_openedInventoryControlsActive)
                {
                    _virtualMouseHandler.EnableMouse();
                }
            }
        }

        public void SetOpenedInventoryControlsInactive()
        {
            _openedInventoryControlsActive = false;
            _inputHandler.EnableFreezableInputActionMaps();

            if (_currentControlScheme == ControlScheme.Gamepad)
            {
                _virtualMouseHandler.DisableMouse();
                Cursor.lockState = CursorLockMode.Locked;
                _inputHandler.canUiChange = false;
                EnableUiChangeInOneFrame();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            Cursor.visible = false;
        }

        /// <summary>
        /// CursorLockMode.Locked вызывает движение курсора в т.ч. Mouse девайса в течение текущего кадра. 
        /// Отключение мыши лишь отодвигает вызов движения до повторного включения мыши.
        /// Поэтому, чтобы избежать изменения интерфейса в рантайме при смене схемы контроля из-за движения мыши, что игрок
        /// не ожидает, было решено возвращать нужную схему контроля на следующем кадре.
        /// </summary>
        private async void EnableUiChangeInOneFrame()
        {
            for (var i = 0; i < 2; i++)
                await Task.Yield();

            _inputHandler.canUiChange = true;
            _playerInput.SwitchCurrentControlScheme(_virtualMouseHandler.VirtualMouse, Gamepad.current);
        }

        public void SetOpenedInventoryControlsActive()
        {
            _openedInventoryControlsActive = true;
            _inputHandler.DisableFreezableInputActionMaps();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if (_currentControlScheme == ControlScheme.Gamepad)
            {
                _virtualMouseHandler.EnableMouse();
            }
        }
    }
}