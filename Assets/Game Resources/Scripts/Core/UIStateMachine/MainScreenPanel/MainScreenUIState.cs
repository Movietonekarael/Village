using GameCore.GameControls;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MainScreenUIState : BaseUIState<MainScreenViewParameters, IMainScreenController>
        {
            [Inject] private readonly IOpenCloseInventory _openCloseInventory;
            [Inject] private readonly IEscapable _escape;

            [Header("Inventory state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _openInventoryStateBase;
            private IUIState _openInventoryState { get => _openInventoryStateBase as IUIState; }

            [Header("Pause state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _pauseStateBase;
            private IUIState _pauseState { get => _pauseStateBase as IUIState; }

            protected override void StartState(params bool[] args)
            {
                _openCloseInventory.OnOpenCloseInventory += OpenPlayerInventory;
                _escape.OnEscape += PauseGame;
            }

            protected override void EndState()
            {
                _openCloseInventory.OnOpenCloseInventory -= OpenPlayerInventory;
                _escape.OnEscape -= PauseGame;
            }

            private void OpenPlayerInventory()
            {
                SwitchState(_openInventoryState);
            }

            private void PauseGame()
            {
                SwitchState(_pauseState);
            }
        }
    }
}