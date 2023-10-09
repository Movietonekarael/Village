using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GameControls
    {
        public class HintChanger : MonoBehaviour, ISubscribable
        {
            public ControlScheme CurrentControlScheme = ControlScheme.Keyboard;

            public GameObject KeyboardActiveGameObject;
            public GameObject GamepadActiveGameObject;

            [Inject] private readonly InputHandler _inputHandler;

            private void Start()
            {
                Subscribe();
                SetCurrentActiveObject();
            }

            public void Subscribe()
            {
                _inputHandler.OnControlSchemeChanged += ControlSchemeChanged;
            }

            public void Unsubscribe()
            {
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
}