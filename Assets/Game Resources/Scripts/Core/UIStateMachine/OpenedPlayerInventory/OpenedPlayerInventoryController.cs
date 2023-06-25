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
    public sealed class OpenedPlayerInventoryController : UIController<OpenedPlayerInventoryViewParameters, 
                                                                       IOpenedPlayerInventoryController, 
                                                                       IOpenedPlayerInventoryView>, 
                                                          IOpenedPlayerInventoryController
    {
        [Inject] private readonly InputHandler _inputHandler;
        [Inject] private readonly PlayerInventory _inventory;
        [Inject] private readonly UnityEngine.InputSystem.PlayerInput _playerInput;
        private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
        private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;

        private bool _openedInventoryControlsActive = false;

        protected override void InitializeParameters(OpenedPlayerInventoryViewParameters parameters)
        {
            parameters.ItemsNumber = _inventory.GetInventorySize();
        }

        protected override void SubscribeForEvents()
        {
            _inventory.OnItemChanged += ChangeItemInformation;
            _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
        }

        protected override void UnsubscribeForEvents()
        {
            _inventory.OnItemChanged -= ChangeItemInformation;
            _inputHandler.OnControlSchemeChanged -= ControlSchemeChanged;
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

        private void ChangeItemInformation(int position)
        {
            var item = _inventory.GetGameItem(position);
            _SpecificView.SetItemInformation(position, item);
        }

        protected override void OnActivate()
        {
            SetOpenedInventoryControlsActive();
        }

        private void SetOpenedInventoryControlsActive()
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

        protected override void OnDeactivate()
        {
            SetOpenedInventoryControlsInactive();
        }

        private void SetOpenedInventoryControlsInactive()
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

        public void ChangeItemsInInventory(int itemNumber1, int itemNumber2)
        {
            _inventory.MoveItem(itemNumber1, itemNumber2);
        }

        public void DropInventoryItem(int itemNumber)
        {
            _inventory.DropItem(itemNumber);
        }
    }
}