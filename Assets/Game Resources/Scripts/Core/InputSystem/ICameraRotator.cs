using System;
using UnityEngine;

namespace GameCore.GameControls
{
    public interface ICameraRotator
    {
        public event Action<Vector2, bool> OnCameraRotated;
    }
}

