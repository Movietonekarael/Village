using GameCore.GameControls;
using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryUIState : BaseUIState
    {
        [Header("Next state")]
        [SerializeField] private BaseUIState _closeInventoryState;

        [SerializeField] private OpenedPlayerInventoryViewParameters _openedPlayerInventoryViewParameters;

        [Inject] private readonly IOpenedPlayerInventoryController _openedPlayerInventoryController;
        [Inject(Id = typeof(OpenedPlayerInventoryController))] private readonly IDeinitializable _openedPlayerInventoryControllerDeinitializator;
        [Inject(Id = typeof(OpenedPlayerInventoryController))] private readonly IActivatable _openedPlayerInventoryControllerActivator;

        protected override void OnAwake()
        {
            InitializeController();
        }

        private void InitializeController()
        {
            _openedPlayerInventoryController.Init(_openedPlayerInventoryViewParameters);
        }

        private void DeinitializeController()
        {
            _openedPlayerInventoryControllerDeinitializator.Deinitialize();
        }

        public override void EnterState()
        {
            _openedPlayerInventoryControllerActivator.Activate();

            _InputHandler.OnOpenCloseInventory += ClosePlayerInventory;
        }

        protected override void ExitState()
        {
            _openedPlayerInventoryControllerActivator.Deactivate();

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

