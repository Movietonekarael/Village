using GameCore.GameControls;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class OpenedPlayerInventoryUIState : BaseUIState<OpenedPlayerInventoryViewParameters, 
                                                                       OpenedPlayerInventoryController,
                                                                       IOpenedPlayerInventoryController>
        {
            [Inject] private readonly IOpenCloseInventory _openCloseInventory;
            [Inject] private readonly IEscapable _escape;

            [Header("Next state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _closeInventoryStateBase;

            private IUIState _closeInventoryState { get => _closeInventoryStateBase as IUIState; }


            protected override void StartState(params bool[] args)
            {
                _openCloseInventory.OnOpenCloseInventory += ClosePlayerInventory;
                _escape.OnEscape += ClosePlayerInventory;
            }

            protected override void EndState()
            {
                _openCloseInventory.OnOpenCloseInventory -= ClosePlayerInventory;
                _escape.OnEscape -= ClosePlayerInventory;
            }

            private void ClosePlayerInventory()
            {
                SwitchState(_closeInventoryState);
            }
        }
    }
}