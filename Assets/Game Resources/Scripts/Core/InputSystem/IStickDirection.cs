using UnityEngine;


namespace GameCore.GameControls
{
    public partial class InputHandler
    {
        private interface IStickDirection
        {
            public void Perform(Vector2 vec);
        }
    }
}