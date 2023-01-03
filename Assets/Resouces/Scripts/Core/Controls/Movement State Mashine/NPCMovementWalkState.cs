using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GameMovement
{
    public abstract partial class NPCMovementStateMachine
    {
        public class NPCMovementWalkState : NPCMovementAnyMoveState
        {
            public NPCMovementWalkState(NPCMovementStateMachine currentStateMachine) 
            : base(currentStateMachine) {}

            public override void UpdateState()
            {
                base.UpdateState();
                if (StateMachine._isRunning)
                    SetRunState();
            }

            protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                return vec.normalized * StateMachine._walkSpeedLimit;
            }

            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void SetRunState()
            {
                SwitchState(StateMachine._runState);
            }
        }
    }
}

