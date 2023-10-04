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
                Time.timeScale = 0;
            }

            protected override void OnDeactivate()
            {
                Time.timeScale = 1;
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
                await LoadAndActivateMainMenu();
                NetworkConnectionService.ShutdownConnection();
                NetworkManagerPrefabs.Singleton.DestroyNetworkManager();
                AddressablesSceneManager.Singleton.UnloadAll();


                async Task LoadAndActivateMainMenu()
                {
                    var sceneLoadHandle = Addressables.LoadSceneAsync(_mainMenuSceneReference, LoadSceneMode.Single, false);
                    await sceneLoadHandle.Task;
                    var sceneInstance = sceneLoadHandle.Result;
                    sceneInstance.ActivateAsync();
                }
            }
        }
    }
}