using System;

namespace GameCore.GameControls
{
    public interface IEscapable
    {
        public event Action OnEscape;
    }
}

