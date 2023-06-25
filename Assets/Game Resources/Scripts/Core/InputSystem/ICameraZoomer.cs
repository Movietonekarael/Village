using System;

namespace GameCore.GameControls
{
    public interface ICameraZoomer
    {
        public event Action<float> OnCameraZoomed;
    }
}

