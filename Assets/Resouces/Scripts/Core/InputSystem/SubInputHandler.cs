using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using UnityEngine.InputSystem;
using GameCore.GameMovement;


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

            protected abstract void RegisterForInputEvents();

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