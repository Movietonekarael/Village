using GameCore.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GUI
{
    public partial class UIStateMachine
    {
        public class ClosedInventoryUIState : BaseUIState
        {
            private readonly FastInventoryPanelUI _fastInventoryUI = null;
            public ClosedInventoryUIState(UIStateMachine stateMachine, FastInventoryPanelUI fastInventoryUI) : base(stateMachine) 
            { 
                if (fastInventoryUI is not null)
                {
                    _fastInventoryUI = fastInventoryUI;
                    _fastInventoryUI.SetCanvasActive(false);
                }
            }

            public override void EnterState()
            {
                if (_fastInventoryUI is not null)
                    _fastInventoryUI.SetCanvasActive(true);

                StateMachine.OnOpenCloseInventoryEvent += OpenPlayerInventory;
                StateMachine.OnSwitchSelectedItemEvent += SwitchSelectedItem;
            }

            public override void ExitState()
            {
                if (_fastInventoryUI is not null)
                    _fastInventoryUI.SetCanvasActive(false);

                StateMachine.OnOpenCloseInventoryEvent -= OpenPlayerInventory;
                StateMachine.OnSwitchSelectedItemEvent -= SwitchSelectedItem;
            }

            private void OpenPlayerInventory()
            {
                SwitchState(StateMachine._openedPlayerInventoryUIState);
            }

            private void SwitchSelectedItem(int index)
            {
                if (_fastInventoryUI is not null)
                    _fastInventoryUI.SwitchSelectedItem(index);
            }
        }
    }
}

