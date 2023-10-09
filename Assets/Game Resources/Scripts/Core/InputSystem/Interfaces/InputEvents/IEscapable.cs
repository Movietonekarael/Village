using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface IEscapable
        {
            public event Action OnEscape;
        }
    }
}