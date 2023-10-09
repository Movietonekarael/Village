using System;
using UnityEngine;


namespace GameCore
{
    namespace GameControls
    {
        public interface IMovement
        {
            public event Action OnMovementStart;
            public event Action OnMovementFinish;
            public event Action<Vector2> OnMovement;
            public event Action OnRunningChanged;
            public event Action OnDashed;
            public event Action OnJumped;
        }
    }
}