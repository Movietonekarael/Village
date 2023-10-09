using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GameCore.SceneManagement;
using UnityEngine.SceneManagement;
using GameCore.Network;
using System.Threading.Tasks;


namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenuController : UIController<PauseMenuViewParameters, IPauseMenuController, PauseMenuView, IPauseMenuView>, 
                                                  IPauseMenuController
        {
            public event Action OnContinueGame;
            private AssetReference _mainMenuSceneReference;


            protected override void InitializeParameters(PauseMenuViewParameters parameters) { }

            protected override void SubscribeForPermanentEvents() { }
            protected override void UnsubscribeForPermanentEvents() { }
            protected override void SubscribeForTemporaryEvents() { }
            protected override void UnsubscribeForTemporaryEvents() { }

            protected override void OnActivate()
            {
                if (NetworkConnectionService.GameType == GameType.SinglePlayer)
                {
                    Time.timeScale = 0;
                }
            }

            protected override void OnDeactivate()
            {
                if (NetworkConnectionService.GameType == GameType.SinglePlayer)
                {
                    Time.timeScale = 1;
                }
            }

            public void SetMainMenuSceneReference(AssetReference mainMenuSceneReference)
            {
                _mainMenuSceneReference = mainMenuSceneReference;
            }

            public void ContinueGame()
            {
                OnContinueGame?.Invoke();
            }

            public async void QuitGame()
            {
                _View.DeactivateButtons();
                var loadSceneHandle = Addressables.LoadSceneAsync(_mainMenuSceneReference, LoadSceneMode.Single);
                await loadSceneHandle.Task;
                NetworkConnectionService.ShutdownConnection();
            }
        }
    }
}