using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;


namespace GameCore.GameControls
{
    public static class InputSystemExtensions
    {
        public static void EnableAll(this InputActionMap[] array)
        {
            foreach (var inputActionMap in array)
            {
                inputActionMap.Enable();
            }
        }

        public static void DisableAll(this InputActionMap[] array)
        {
            foreach (var inputActionMap in array)
            {
                inputActionMap.Disable();
            }
        }
    }
}
