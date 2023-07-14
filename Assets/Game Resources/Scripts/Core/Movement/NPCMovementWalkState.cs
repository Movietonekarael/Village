using UnityEngine;


namespace GameCore
{
    namespace GameMovement
    {
        public abstract partial class NPCMovementStateMachine
        {
            public class NPCMovementWalkState : NPCMovementAnyMoveState
            {
                public NPCMovementWalkState(NPCMovementStateMachine currentStateMachine)
                : base(currentStateMachine) { }

                public override void UpdateState()
                {
                    base.UpdateState();
                    if (_StateMachine._isRunning)
                        SetRunState();
                }

                protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
                {
                    return vec.normalized * _StateMachine._walkVelocityLimit;
                }

                //----------------------------------------------------------Local methods------------------------------------------------------//

                private void SetRunState()
                {
                    SwitchState(_StateMachine._runState);
                }
            }
        }
    }
}