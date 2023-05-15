using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.InputSystem;
using static GameCore.GameControls.InputHandler;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryController : ISubscribable, IDeinitializable, IActivatable
    {
        private OpenedPlayerInventoryView _openedPlayerInventoryView;
        private InputHandler _inputHandler;
        private PlayerInventory _inventory;

        private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
        private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;

        private bool _openedInventoryControlsActive = false;

        public OpenedPlayerInventoryController(OpenedPlayerInventoryArgs args, PlayerInventory inventory) 
        {
            CacheInputHandler();
            CacheInventory(inventory);
            InitializeView(args);
            Subscribe();
        }

        private void CacheInputHandler()
        {
            _inputHandler = InputHandler.GetInstance(this.GetType().Name);
        }

        private void CacheInventory(PlayerInventory inventory)
        {
            _inventory = inventory;
        }

        private void InitializeView(OpenedPlayerInventoryArgs args)
        {
            _openedPlayerInventoryView = new(args, this);
        }

        public void Deinitialize()
        {
            Unsubscribe();
            _openedPlayerInventoryView.Deinitialize();
            _openedPlayerInventoryView = null;
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
            _openedPlayerInventoryView.Activate();
            SetOpenedInventoryControlsActive();
        }

        public void Deactivate()
        {
            _openedPlayerInventoryView.Deactivate();
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
                    _virtualMouseHandler.DeactivateVirtualPointer();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                _virtualMouseHandler.DisableVirtualMouse();
            }
            else if (controlScheme == ControlScheme.Gamepad)
            {
                _virtualMouseHandler.EnableVirtualMouse();
                if (_openedInventoryControlsActive)
                {
                    _virtualMouseHandler.ActivateVirtualPointer();
                }
            }
        }

        public void SetOpenedInventoryControlsInactive()
        {
            _openedInventoryControlsActive = false;
            _inputHandler.EnableFreezableInputActionMaps();

            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (_currentControlScheme == ControlScheme.Gamepad)
            {
                _virtualMouseHandler.DeactivateVirtualPointer();
                Cursor.visible = false;
            }
        }

        public void SetOpenedInventoryControlsActive()
        {
            _openedInventoryControlsActive = true;
            _inputHandler.DisableFreezableInputActionMaps();

            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (_currentControlScheme == ControlScheme.Gamepad)
            {
                _virtualMouseHandler.ActivateVirtualPointer();
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}