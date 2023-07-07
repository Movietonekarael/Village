using System;
using UnityEngine;
using Zenject;
using GameCore.GameControls;
using static GameCore.GameControls.InputHandler;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

namespace GameCore.GUI
{
    public sealed class PauseMenuController : UIController<PauseMenuViewParameters, IPauseMenuController, IPauseMenuView>, IPauseMenuController
    {
        public event Action OnContinueGame;


        protected override void InitializeParameters(PauseMenuViewParameters parameters) { }

        protected override void SubscribeForEvents() { }
        protected override void UnsubscribeForEvents() { }

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

