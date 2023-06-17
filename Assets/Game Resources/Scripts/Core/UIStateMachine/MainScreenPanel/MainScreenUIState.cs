using GameCore.GameControls;
using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore.GUI
{
    public sealed class MainScreenUIState : BaseUIState
    {
        [Header("Next state")]
        [SerializeField] private BaseUIState _openInventoryState;

        [SerializeField] private MainScreenViewParameters _mainScreenViewParameters;

        [Inject] private readonly IMainScreenController _mainScreenController;
        [Inject(Id = typeof(MainScreenController))] private readonly IDeinitializable _mainScreenControllerDeinitializator;
        [Inject(Id = typeof(MainScreenController))] private readonly IActivatable _mainScreenControllerActivator;

        protected override void OnAwake()
        {
            InitializeController();
        }

        private void InitializeController()
        {          
            _mainScreenController.Init(_mainScreenViewParameters);
        }

        private void DeinitializeController()
        {
            _mainScreenControllerDeinitializator.Deinitialize();
        }

        public override void EnterState()
        {
            _mainScreenControllerActivator.Activate();

            _InputHandler.OnOpenCloseInventory += OpenPlayerInventory;
        }

        protected override void ExitState()
        {
            _mainScreenControllerActivator.Deactivate();

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

