using System;
using UnityEngine;
using UnityEditor;


namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenuController : UIController<PauseMenuViewParameters, IPauseMenuController, PauseMenuView, IPauseMenuView>, 
                                                  IPauseMenuController
        {
            public event Action OnContinueGame;


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

            public void ContinueGame()
            {
                OnContinueGame?.Invoke();
            }

            public void QuitGame()
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
        }
    }
}