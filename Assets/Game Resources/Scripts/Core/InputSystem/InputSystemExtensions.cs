using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCore
{
    namespace GameControls
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
}