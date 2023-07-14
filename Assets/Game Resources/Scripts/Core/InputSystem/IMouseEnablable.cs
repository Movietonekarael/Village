using UnityEngine;


namespace GameCore
{
    namespace GameControls
    {
        public partial class InputHandler
        {
            public interface IMouseEnablable
            {
                public void EnableMouse();
                public void DisableMouse();
                public void SetPosition(Vector2 position);
                public void SetDelta(Vector2 delta);
                public Vector2 GetPosition();
            }
        }
    }
}