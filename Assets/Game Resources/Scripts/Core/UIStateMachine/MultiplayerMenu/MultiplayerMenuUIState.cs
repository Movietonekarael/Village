using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;


namespace GameCore
{
    namespace GUI
    {
        public sealed class MuiltiplayerMenuUIState : BaseUIState<MultiplayerMenuViewParameters,
                                                                  MultiplayerMenuController,
                                                                  IMultiplayerMenuController>
        {
            [Header("Main menu state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _mainMenuStateBase;
            private IUIState _mainMenuState { get => _mainMenuStateBase as IUIState; }


            protected override void StartState(params bool[] args)
            {
                _Controller.OnBackToMainMenu += LoadMainMenu;
            }

            protected override void EndState()
            {
                _Controller.OnBackToMainMenu -= LoadMainMenu;
            }

            private void LoadMainMenu()
            {
                SwitchState(_mainMenuState);
            }
        }
    }
}