using System;


namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraAngle
        {
            public event Action<float> OnCameraAngleChanged;
        }
    }
}