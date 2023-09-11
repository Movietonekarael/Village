using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.AddressableAssets;


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

            [SerializeField] private AssetReference _multiplayerPlayerScene;


            protected override void StartState(params bool[] args)
            {
                _Controller.OnBackToMainMenu += LoadMainMenu;
                _Controller.SetMultiplayerPlayerSceneReference(_multiplayerPlayerScene);
            }

            protected override void EndState()
            {
                _Controller.OnBackToMainMenu -= LoadMainMenu;
            }

            private void LoadMainMenu()
            {
                var mainMenuState = _mainMenuStateBase as MainMenuUIState;
                mainMenuState.StartupAnimationAllowed = false;
                SwitchState(_mainMenuState);
            }
        }
    }
}