using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraZoomer
        {
            public event Action<float> OnCameraZoomed;
        }
    }
}