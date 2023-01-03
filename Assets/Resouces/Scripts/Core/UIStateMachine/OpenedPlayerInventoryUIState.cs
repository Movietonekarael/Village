using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GUI
{
    public partial class UIStateMachine
    {
        public class OpenedPlayerInventoryUIState : BaseUIState
        {
            private readonly PlayerInventoryPanelUI _playerInventoryUI;
            public OpenedPlayerInventoryUIState(UIStateMachine stateMachine, PlayerInventoryPanelUI playerInventoryUI) : base(stateMachine) 
            {
                if (playerInventoryUI is not null)
                {
                    _playerInventoryUI = playerInventoryUI;
                    _playerInventoryUI._playerInventoryCanvas.SetActive(false);
                    StateMachine._inputHandler.FreezeControlForInteraction(false);
                }
            }

            public override void EnterState()
            {
                if (_playerInventoryUI is not null)
                {
                    _playerInventoryUI._playerInventoryCanvas.SetActive(true);
                    StateMachine._inputHandler.FreezeControlForInteraction(true);
                }
                
                StateMachine.OnOpenCloseInventoryEvent += ClosePlayerInventory;
            }

            public override void ExitState()
            {
                if (_playerInventoryUI is not null)
                {
                    _playerInventoryUI._playerInventoryCanvas.SetActive(false);
                    StateMachine._inputHandler.FreezeControlForInteraction(false);
                }
                
                StateMachine.OnOpenCloseInventoryEvent -= ClosePlayerInventory;
            }

            private void ClosePlayerInventory()
            {
                SwitchState(StateMachine._closedInventoryUIState);
            }
        }
    }
}

