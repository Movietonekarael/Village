using System;

namespace GameCore
{
    namespace GameControls
    {
        public interface IInteractionPerformer
        {
            public event Action OnInteractionPerformed;
        }
    }
}