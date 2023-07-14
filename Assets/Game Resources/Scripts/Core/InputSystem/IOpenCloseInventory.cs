using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface IOpenCloseInventory
        {
            public event Action OnOpenCloseInventory;
        }
    }
}