using GameCore.GameControls;
using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryUIState : BaseUIState
    {
        [Header("Next state")]
        [SerializeField] private BaseUIState _closeInventoryState;

        [Header("Inventory to show")]
        [SerializeField] private GameObject _inventory;

        private EventSystem _eventSystem { get => _StateMachine.EventSystem; }
        private Camera _uiCamera { get => _StateMachine.UiCamera; }

        [SerializeField] private OpenedPlayerInventoryViewParameters _openedPlayerInventoryViewParameters;

        private OpenedPlayerInventoryController _openedPlayerInventoryController;

        protected override void Start()
        {
            base.Start();

            InitializeController();
        }

        private void InitializeController()
        {
            var args = new OpenedPlayerInventoryArgs
            {
                OpenedPlayerInventoryViewParameters = _openedPlayerInventoryViewParameters,
                ItemsNumber = _inventory.GetComponent<PlayerInventory>().GetInventorySize(),
                EventSystem = _eventSystem,
                UiCamera = _uiCamera
            };
            var inventory = _inventory.GetComponent<PlayerInventory>();

            _openedPlayerInventoryController = new(args, inventory);
        }

        private void DeinitializeController()
        {
            _openedPlayerInventoryController.Deinitialize();
            _openedPlayerInventoryController = null;
        }

        public override void EnterState()
        {
            _openedPlayerInventoryController.Activate();

            _InputHandler.OnOpenCloseInventory += ClosePlayerInventory;
        }

        public override void ExitState()
        {
            _openedPlayerInventoryController.Deactivate();

            _InputHandler.OnOpenCloseInventory -= ClosePlayerInventory;
        }

        private void ClosePlayerInventory()
        {
            SwitchState(_closeInventoryState);
        }

        private void OnDestroy()
        {
            DeinitializeController();
        }
    }
}

