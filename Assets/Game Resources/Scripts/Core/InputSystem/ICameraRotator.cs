using System;
using UnityEngine;


namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraRotator
        {
            public event Action<Vector2, bool> OnCameraRotated;
        }
    }
}