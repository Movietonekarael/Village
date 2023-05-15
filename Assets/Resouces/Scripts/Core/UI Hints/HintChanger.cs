using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore.GameControls
{
    public class HintChanger : MonoBehaviour, ISubscribable
    {
        public ControlScheme CurrentControlScheme = ControlScheme.Keyboard;

        public GameObject KeyboardActiveGameObject;
        public GameObject GamepadActiveGameObject;

        private InputHandler _inputHandler;

        private void Start()
        {
            CacheInputHandler();
            Subscribe();
            SetCurrentActiveObject();
        }

        private void CacheInputHandler()
        {
            _inputHandler = InputHandler.GetInstance(this.GetType().Name);
        }

        public void Subscribe()
        {
            _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
        }

        public void Unsubscribe()
        {
            if (_inputHandler != null)
                _inputHandler.OnControlSchemeChanged -= ControlSchemeChanged;
        }

        private void ControlSchemeChanged(ControlScheme controlScheme)
        {
            CurrentControlScheme = controlScheme;
            SetCurrentActiveObject();
        }

        private void SetCurrentActiveObject()
        {
            if (CurrentControlScheme == ControlScheme.Keyboard)
            {
                KeyboardActiveGameObject.SetActive(true);
                GamepadActiveGameObject.SetActive(false);
            }
            else if (CurrentControlScheme == ControlScheme.Gamepad)
            {
                GamepadActiveGameObject.SetActive(true);
                KeyboardActiveGameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }

}
