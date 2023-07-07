using GameCore.GameControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCore.GameControls.InputHandler;
using UnityEngine.InputSystem;
using Zenject;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace GameCore.GUI
{
    public sealed class CursorUnlockUIState : BaseUIState<CursorUnlockViewParameters, ICursorUnlockController>
    {
        protected override void StartState()
        {

        }

        protected override void EndState()
        {

        }
    }

    public sealed class CursorUnlockController : UIController<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockController
    {
        [Inject] private readonly InputHandler _inputHandler;
        [Inject] private readonly UnityEngine.InputSystem.PlayerInput _playerInput;
        private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
        private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;


        protected override void InitializeParameters(CursorUnlockViewParameters parameters)
        {

        }

        protected override void OnActivate()
        {
            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            _inputHandler.DisableFreezableInputActionMaps();
        }

        protected override void OnDeactivate()
        {
            if (_currentControlScheme == ControlScheme.Keyboard)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

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

        /// <summary>
        /// CursorLockMode.Locked вызывает движение курсора в т.ч. Mouse девайса в течение текущего кадра. 
        /// Отключение мыши лишь отодвигает вызов движения до повторного включения мыши.
        /// Поэтому, чтобы избежать изменения интерфейса в рантайме при смене схемы контроля из-за движения мыши, что игрок
        /// не ожидает, было решено возвращать нужную схему контроля на следующем кадре.
        /// </summary>
        private async void EnableUiChangeInOneFrame()
        {
            for (var i = 0; i < 2; i++)
                await Task.Yield();
            _playerInput.SwitchCurrentControlScheme(_virtualMouseHandler.VirtualMouse, Gamepad.current);
        }
    }

    public sealed class CursorUnlockView : UIView<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockView
    {
        [Inject] private readonly EventSystem _eventSystem;

        private GameObject _lastSubmitObject;


        public override void Activate()
        {
            _lastSubmitObject = _eventSystem.currentSelectedGameObject;
        }

        public override void Deactivate()
        {

        }

        public override void Deinitialize()
        {

        }

        public void RememberSubmitButton()
        {
            _lastSubmitObject = _eventSystem.currentSelectedGameObject;
        }

        public void SetLastSubmitButton()
        {
            _eventSystem.SetSelectedGameObject(_lastSubmitObject);
        }

        protected override void InstantiateViewElements()
        {

        }
    }
}