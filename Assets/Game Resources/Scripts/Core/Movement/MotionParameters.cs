using Unity;
using UnityEngine;


namespace GameCore.GameMovement
{
    public abstract partial class NPCMovementStateMachine
    {
        [System.Serializable]
        public struct MotionParameters
        {
            [SerializeField] public float Acceleration;
            [SerializeField] public float Deceleration;

            public MotionParameters(float acceleration = 0, float deceleration = 0)
            {
                Acceleration = acceleration;
                Deceleration = deceleration;
            }
        }

    }
}

