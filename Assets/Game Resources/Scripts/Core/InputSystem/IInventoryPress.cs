using System;

namespace GameCore.GameControls
{
    public interface IInventoryPress
    {
        public event Action<int> OnInventoryKeyPressed;
        public event Action<int> OnInventoryArrowPressed;
    }
}

