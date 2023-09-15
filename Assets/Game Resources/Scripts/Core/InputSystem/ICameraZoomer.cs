using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraZoomerInput
        {
            public event Action<float> OnCameraZoomed;
        }
    }
}