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
    public sealed class MainScreenUIState : BaseUIState<MainScreenViewParameters, IMainScreenController>
    {
        [Inject] private readonly IOpenCloseInventory _openCloseInventory;

        [Header("Next state")]
        [SerializeField]
        [RequireInterface(typeof(IUIState))]
        private UnityEngine.Object _openInventoryStateBase;

        private IUIState _openInventoryState { get => _openInventoryStateBase as IUIState; }


        protected override void StartState()
        {
            _openCloseInventory.OnOpenCloseInventory += OpenPlayerInventory;
        }

        protected override void EndState()
        {
            _openCloseInventory.OnOpenCloseInventory -= OpenPlayerInventory;
        }

        private void OpenPlayerInventory()
        {
            SwitchState(_openInventoryState);
        }
    }
}

