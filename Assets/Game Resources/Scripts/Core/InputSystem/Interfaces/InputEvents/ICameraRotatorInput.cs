using System;
using UnityEngine;


namespace GameCore
{
    namespace GameControls
    {
        public interface ICameraRotatorInput
        {
            public event Action<Vector2, bool> OnCameraRotated;
        }
    }
}