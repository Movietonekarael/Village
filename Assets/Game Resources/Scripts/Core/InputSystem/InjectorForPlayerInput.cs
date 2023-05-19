using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Zenject;


namespace GameCore.Injectors
{
    [RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
    public class InjectorForPlayerInput : MonoBehaviour
    {
        [Inject(Id = "UiCamera")] private Camera _uiCamera;

        private void Awake()
        {
            var playerInput = GetPlayerInput();
            InjectUiCamera(playerInput);
            InjectUiInputModule(playerInput);
        }

        private UnityEngine.InputSystem.PlayerInput GetPlayerInput()
        {
            return GetComponent<UnityEngine.InputSystem.PlayerInput>();
        }

        private void InjectUiCamera(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            playerInput.camera = _uiCamera;
        }

        private void InjectUiInputModule(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            var uiInputModule = FindFirstObjectByType<InputSystemUIInputModule>();
            playerInput.uiInputModule = uiInputModule;
        }
    }
}

