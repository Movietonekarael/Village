using System;

namespace GameCore
{
    namespace GameControls
    {
        public interface IEnterable
        {
            public event Action OnEnter;
        }
    }
}

