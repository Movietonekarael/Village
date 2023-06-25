using GameCore.GameControls;
using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore.GUI
{
    public sealed class OpenedPlayerInventoryUIState : BaseUIState<OpenedPlayerInventoryViewParameters, IOpenedPlayerInventoryController>
    {
        [Inject] private readonly IOpenCloseInventory _openCloseInventory;

        [Header("Next state")]
        [SerializeField]
        [RequireInterface(typeof(IUIState))]
        private UnityEngine.Object _closeInventoryStateBase;

        private IUIState _closeInventoryState { get => _closeInventoryStateBase as IUIState; }


        protected override void StartState()
        {
            _openCloseInventory.OnOpenCloseInventory += ClosePlayerInventory;
        }

        protected override void EndState()
        {
            _openCloseInventory.OnOpenCloseInventory -= ClosePlayerInventory;
        }

        private void ClosePlayerInventory()
        {
            SwitchState(_closeInventoryState);
        }
    }
}

