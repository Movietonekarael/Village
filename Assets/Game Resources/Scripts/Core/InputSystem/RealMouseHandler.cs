using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


namespace GameCore.GameControls
{
    public partial class InputHandler
    {
        public sealed class RealMouseHandler : IMouseEnablable
        {
            private const string _DEVICE_NAME = "Mouse";

            private Mouse _realMouse;

            public RealMouseHandler(InputHandler inputHandler) 
            { 
                CacheRealMouse();
            }

            private void CacheRealMouse()
            {
                _realMouse = InputSystem.GetDevice(_DEVICE_NAME) as Mouse;
            }

            public void EnableMouse()
            {
                InputSystem.EnableDevice(_realMouse);
            }

            public void DisableMouse()
            {
                InputSystem.DisableDevice(_realMouse);
            }

            public void SetPosition(Vector2 position)
            {
                _realMouse.WarpCursorPosition(position);
            }

            public void SetDelta(Vector2 delta)
            {
                InputState.Change(_realMouse.delta, delta);
            }

            public Vector2 GetPosition()
            {
                return _realMouse.position.ReadValue();
            }
        }
    }
}