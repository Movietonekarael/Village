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