using UnityEngine;


namespace GameCore
{
    namespace GameControls
    {
        public partial class InputHandler
        {
            private interface IStickDirection
            {
                public void Perform(Vector2 vec);
            }
        }
    }
}