using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.GUI
{
    public sealed class MainScreenUIState : BaseUIState
    {
        [Header("Next state")]
        [SerializeField] private BaseUIState _openInventoryState;

        [Header("Inventory to show")]
        [SerializeField] private GameObject _inventory;
        [SerializeField] private PlayerHoldItem _holdItem;

        private Camera _uiCamera { get => _StateMachine.UiCamera; }

        [SerializeField] private MainScreenViewParameters _mainScreenViewParameters;

        private MainScreenController _mainScreenController;


        protected override void Start()
        {
            base.Start();

            InitializeController();
        }

        private void InitializeController()
        {
            var args = new MainScreenArgs
            {
                MainScreenViewParameters = _mainScreenViewParameters,
                UiCamera = _uiCamera
            };
            var inventory = _inventory.GetComponent<IInventory>();

            _mainScreenController = new(args, inventory, _holdItem);
        }

        private void DeinitializeController()
        {
            _mainScreenController.Deinitialize();
            _mainScreenController = null;
        }

        public override void EnterState()
        {
            _mainScreenController.Activate();

            _InputHandler.OnOpenCloseInventory += OpenPlayerInventory;
        }

        public override void ExitState()
        {
            _mainScreenController.Deactivate();

            _InputHandler.OnOpenCloseInventory -= OpenPlayerInventory;
        }

        private void OpenPlayerInventory()
        {
            SwitchState(_openInventoryState);
        }

        private void OnDestroy()
        {
            DeinitializeController();
        }
    }
}

