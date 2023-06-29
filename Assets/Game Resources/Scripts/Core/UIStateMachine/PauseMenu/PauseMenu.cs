using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
