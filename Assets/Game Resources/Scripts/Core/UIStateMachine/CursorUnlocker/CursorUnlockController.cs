using GameCore.GameControls;
using UnityEngine;
using static GameCore.GameControls.InputHandler;
using UnityEngine.InputSystem;
using Zenject;
using System.Threading.Tasks;


namespace GameCore
{
    namespace GUI
    {
        public sealed class CursorUnlockController : UIController<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockController
        {
            [Inject] private readonly InputHandler _inputHandler;
            private VirtualMouseHandler _virtualMouseHandler => _inputHandler.VirtualMouse;
            private ControlScheme _currentControlScheme => _inputHandler.CurrentControlScheme;

            private bool _virtualPointerAllowed;
            private bool _controlsDisabled = false;


            protected override void InitializeParameters(CursorUnlockViewParameters parameters) { }

            public void SetVirtualMouseAvailability(bool allowed)
            {
                _virtualPointerAllowed = allowed;
            }

            protected override void OnActivate()
            {
                if (!_virtualPointerAllowed)
                    DisableCursor();
                else
                    EnableVirtualPointer();

                _inputHandler.DisableFreezableInputActionMaps();

                if (_inputHandler.CurrentControlScheme == ControlScheme.Keyboard)
                    _SpecificView.DisableSelection();
            }

            protected override void OnDeactivate()
            {
                if (_virtualPointerAllowed == false)
                    EnableCursor();
                else
                    DisableVirtualPointer();

                _inputHandler.EnableFreezableInputActionMaps();

                _SpecificView.EnableSelection();
            }

            protected override void SubscribeForEvents()
            {
                _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
            }

            protected override void UnsubscribeForEvents()
            {
                _inputHandler.OnControlSchemeChanged -= ControlSchemeChanged;
            }

            private void EnableCursor()
            {
                if (_currentControlScheme == ControlScheme.Keyboard)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            private void DisableCursor()
            {
                if (_currentControlScheme == ControlScheme.Keyboard)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            private void EnableVirtualPointer()
            {
                _controlsDisabled = true;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (_currentControlScheme == ControlScheme.Gamepad)
                {
                    _virtualMouseHandler.EnableMouse();
                }
            }

            private void DisableVirtualPointer()
            {
                _controlsDisabled = false;

                if (_currentControlScheme == ControlScheme.Gamepad)
                {
                    _virtualMouseHandler.DisableMouse();
                    Cursor.lockState = CursorLockMode.Locked;
                    _inputHandler.ChangeControlSchemeInOneFrame(_currentControlScheme);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                Cursor.visible = false;
            }

            private void ControlSchemeChanged(ControlScheme controlScheme)
            {
                if (!_virtualPointerAllowed)
                {
                    HandleChangeForCursor(controlScheme);
                }
                else
                {
                    HandleChangeForVirtualPointer(controlScheme);
                }
            }

            private void HandleChangeForCursor(ControlScheme controlScheme)
            {
                if (controlScheme == ControlScheme.Keyboard)
                {
                    _SpecificView.DisableSelection();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (controlScheme == ControlScheme.Gamepad)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    _SpecificView.EnableSelection();
                    _inputHandler.ChangeControlSchemeInOneFrame(controlScheme);
                }
            }

            private void HandleChangeForVirtualPointer(ControlScheme controlScheme)
            {
                if (controlScheme == ControlScheme.Keyboard)
                {
                    if (_controlsDisabled)
                    {
                        _virtualMouseHandler.DisableMouse();
                    }
                }
                else if (controlScheme == ControlScheme.Gamepad)
                {
                    if (_controlsDisabled)
                    {
                        _virtualMouseHandler.EnableMouse();
                    }
                }
            }
        }
    }
}