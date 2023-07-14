using UnityEngine;


namespace GameCore
{
    namespace GameMovement
    {
        public class ListenJumpEvent : MonoBehaviour
        {
            [SerializeField] private NPCMovementStateMachine _npc;
            public NPCMovementStateMachine NPC
            {
                set
                {
                    _npc = value;
                }
            }

            public void JumpEnded()
            {
                if (_npc != null)
                    _npc.EndJump();
            }

            public void StartJumpingEnd()
            {
                if (_npc != null)
                    _npc.StartJumpingEnd();
            }
        }
    }
}