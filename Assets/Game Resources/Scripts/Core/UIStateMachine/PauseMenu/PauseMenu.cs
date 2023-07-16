using System;
using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenu : MonoBehaviour
        {
            public event Action OnContinuePressed;
            public event Action OnQuitPressed;

            public GameObject ContinueButton;
            public GameObject QuitButton;

            [HideInInspector] public UiSelecter ContinueUiSelecter;
            [HideInInspector] public UiSelecter QuitUiSelecter;


            private void Awake()
            {
                SetUiSelecters();
            }

            private void SetUiSelecters()
            {
                ContinueUiSelecter = ContinueButton.GetComponent<UiSelecter>();
                QuitUiSelecter = QuitButton.GetComponent<UiSelecter>();
            }

            public void ContinuePressed()
            {
                OnContinuePressed?.Invoke();
            }

            public void QuitPressed()
            {
                OnQuitPressed?.Invoke();
            }
        }
    }
}