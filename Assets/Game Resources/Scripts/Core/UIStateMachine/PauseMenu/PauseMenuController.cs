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
        [Inject] private readonly InputHandler _inputHandler;
        [Inject] private readonly UnityEngine.InputSystem.PlayerInput _playerInput;
        private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
        private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;

        protected override void InitializeParameters(PauseMenuViewParameters parameters)
        {

        }

        protected override void OnActivate()
        {
            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
            Time.timeScale = 0;
            _inputHandler.DisableFreezableInputActionMaps();
        }

        protected override void OnDeactivate()
        {
            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Time.timeScale = 1;
            _inputHandler.EnableFreezableInputActionMaps();
        }

        protected override void SubscribeForEvents()
        {
            _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
        }

        protected override void UnsubscribeForEvents()
        {
            _inputHandler.OnControlSchemeChanged -= ControlSchemeChanged;
        }

        private void ControlSchemeChanged(ControlScheme controlScheme)
        {
            if (controlScheme == ControlScheme.Keyboard)
            {
                _SpecificView.RememberSubmitButton();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (controlScheme == ControlScheme.Gamepad)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _SpecificView.SetLastSubmitButton();
                EnableUiChangeInOneFrame();
            }
        }

        private async void EnableUiChangeInOneFrame()
        {
            for (var i = 0; i < 2; i++)
                await Task.Yield();

            _playerInput.SwitchCurrentControlScheme(_virtualMouseHandler.VirtualMouse, Gamepad.current);
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

