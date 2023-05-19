using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using GameCore.GameMovement;
using Lightbug.CharacterControllerPro.Implementation;
using System.Runtime.CompilerServices;

namespace GameCore.GameControls
{
    partial class InputHandler
    {
        private abstract class SubInputHandler
        {
            protected readonly InputHandler _InputHandler;

            public SubInputHandler(InputHandler inputHandler)
            {
                _InputHandler = inputHandler;
                RegisterForInputEvents();
            }

            public void DestroyAllReferences()
            {
                UnregisterForInputEvents();
            }
            
            protected abstract void RegisterForInputEvents();
            protected abstract void UnregisterForInputEvents();

            protected void CheckForInputHandler(string handlerName)
            {
                if (_InputHandler == null)
                {
                    throw new Exception("InputHandler reference not found in " +
                                        handlerName + ".");
                }
            }
        }
    }
}