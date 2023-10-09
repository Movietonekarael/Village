using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenu : MonoBehaviour
        {
            public event Action OnContinuePressed;
            public event Action OnQuitPressed;

            public GameObject ContinueButtonObject;
            public GameObject QuitButtonObject;

            private Button _continueButton;
            private Button _quitButton;

            [HideInInspector] public UiSelecter ContinueUiSelecter;
            [HideInInspector] public UiSelecter QuitUiSelecter;


            private void Awake()
            {
                SetUiSelecters();
                SetButtons();
            }

            private void SetUiSelecters()
            {
                ContinueUiSelecter = ContinueButtonObject.GetComponent<UiSelecter>();
                QuitUiSelecter = QuitButtonObject.GetComponent<UiSelecter>();
            }

            private void SetButtons()
            {
                _continueButton = ContinueButtonObject.GetComponent<Button>();
                _quitButton = QuitButtonObject.GetComponent<Button>();
            }

            public void ContinuePressed()
            {
                OnContinuePressed?.Invoke();
            }

            public void QuitPressed()
            {
                OnQuitPressed?.Invoke();
            }

            public void ActivateButtons()
            {
                _continueButton.interactable = true;
                _quitButton.interactable = true;
            }

            public void DeactivateButtons()
            {
                _continueButton.interactable = false;
                _quitButton.interactable = false;
            }
        }
    }
}