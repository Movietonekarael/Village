using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface IInventoryPress
        {
            public event Action<int> OnInventoryKeyPressed;
            public event Action<int> OnInventoryArrowPressed;
        }
    }
}