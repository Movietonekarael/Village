using System;


namespace GameCore
{
    namespace GameControls
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
}