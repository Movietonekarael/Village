using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GameCore.Network;
using GameCore.SceneManagement;
using UnityEngine.SceneManagement;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenuController : UIController<MainMenuViewParameters,
                                                              IMainMenuController,
                                                              MainMenuView,
                                                              IMainMenuView>,
                                                 IMainMenuController
        {
            public event Action OnStartMultiplayer;

            private AssetReference _singlePlayerSceneReference;


            protected override void InitializeParameters(MainMenuViewParameters parameters) { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void SubscribeForPermanentEvents() { }
            protected override void SubscribeForTemporaryEvents() { }
            protected override void UnsubscribeForPermanentEvents() { }
            protected override void UnsubscribeForTemporaryEvents() { }

            public void SetSinglePlayerSceneReference(AssetReference singlePlayerSceneReference)
            {
                _singlePlayerSceneReference = singlePlayerSceneReference;
            }

            public async void StartSinglePlayer()
            {
                await NetworkConnectionService.StartHost();
                await AddressablesSceneManager.Singleton.LoadSceneServerAsync(_singlePlayerSceneReference, LoadSceneMode.Single);
            }

            public void StartMultiPlayer()
            {
                OnStartMultiplayer?.Invoke();
            }

            public void QuitApplication()
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }

            public void SetStartupAnimationAvailability(bool allowed)
            {
                _View.SetStartupAnimationAvailability(allowed);
            }
        }
    }
}